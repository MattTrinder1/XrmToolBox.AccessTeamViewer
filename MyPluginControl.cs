using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using XrmToolBox.Extensibility;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk;
using McTools.Xrm.Connection;
using MsCrmTools.AuditCenter;
using Microsoft.Xrm.Sdk.Metadata;
using MoD.CAMS.Plugins.Common;
using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata.Query;
using Microsoft.Xrm.Sdk.Messages;

namespace AccessTeamViewer
{
    public partial class MyPluginControl : PluginControlBase
    {
        private Settings mySettings;
        public enum ActionState
        {
            None,
            Added,
            Removed
        }

        private List<AttributeInfo> attributeInfos;
        private List<EntityMetadata> emds;
        private List<EntityInfo> entityInfos;
        private List<SortingConfiguration> sortingConfigurations;

        public MyPluginControl()
        {
            InitializeComponent();
        }

        private void MyPluginControl_Load(object sender, EventArgs e)
        {
            
            // Loads or creates the settings for the plugin
            if (!SettingsManager.Instance.TryLoad(GetType(), out mySettings))
            {
                mySettings = new Settings();

                LogWarning("Settings not found => a new settings file has been created!");
            }
            else
            {
                LogInfo("Settings found and loaded");
            }
        }

        private void tsbClose_Click(object sender, EventArgs e)
        {
            CloseTool();
        }

       

      
        /// <summary>
        /// This event occurs when the plugin is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyPluginControl_OnCloseTool(object sender, EventArgs e)
        {
            // Before leaving, save the settings
            SettingsManager.Instance.Save(GetType(), mySettings);
        }

        /// <summary>
        /// This event occurs when the connection has been updated in XrmToolBox
        /// </summary>
        public override void UpdateConnection(IOrganizationService newService, ConnectionDetail detail, string actionName, object parameter)
        {
            base.UpdateConnection(newService, detail, actionName, parameter);

            if (mySettings != null && detail != null)
            {
                mySettings.LastUsedOrganizationWebappUrl = detail.WebApplicationUrl;
                LogInfo("Connection has changed to: {0}", detail.WebApplicationUrl);
            }
        }

              
        private void btnGetTeams_Click(object sender, EventArgs e)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Access Teams",
                Work = (worker, args) =>
                {
                    var q = new QueryExpression("team");
                    q.ColumnSet = new ColumnSet(true);
                    q.AddCriteria("teamtype", 1);
                    //q.AddCriteria("teamtemplateid", teamTemplateId);
                    q.AddCriteria("regardingobjectid", new Guid(txtEntityId.Text));
                    var l = q.AddLink("teamtemplate", "teamtemplateid", "teamtemplateid");
                    l.Columns = new ColumnSet("teamtemplatename");
                    l.EntityAlias = "tt";
                    
                    args.Result =  Service.GetMultiple(q);

                    foreach (var team in (args.Result as List<Entity>))
                    {
                        
                        var accessRequest = new RetrieveSharedPrincipalsAndAccessRequest
                        {
                            Target = new EntityReference((cBoxEntities.SelectedItem as EntityInfo).LogicalName, new Guid(txtEntityId.Text))
                        };

                        var accessResponse = (RetrieveSharedPrincipalsAndAccessResponse)Service.Execute(accessRequest);

                        var p = accessResponse.PrincipalAccesses.SingleOrDefault(x => x.Principal.Id == team.Id);
                        if (p != null)
                        {
                            team["accessrights"] = p.AccessMask;
                        }
                    }
                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    lvTeams.Items.Clear();
                    lvTeamMembers.Items.Clear();

                    var result = args.Result as List<Entity>;
                    if (result != null)
                    {
                        foreach (var team in result)
                        {
                            var item = new ListViewItem { Text = team.GetValue<string>("tt.teamtemplatename"),Tag = team };
                            item.SubItems.Add(team["accessrights"].ToString());
                            lvTeams.Items.Add(item);
                        }
                    }
                }
            });

        }

        private void lvTeams_SelectedIndexChanged(object sender, EventArgs e)
        {
            WorkAsync(new WorkAsyncInfo
            {
                Message = "Loading Team Members",
                Work = (worker, args) =>
                {
                    var fetchXml = $@"
                    <fetch>
                      <entity name='teammembership'>
                        <attribute name='systemuserid' />
                        <link-entity name='systemuser' from='systemuserid' to='systemuserid'>
                          <attribute name='fullname' />
                        </link-entity>
                        <link-entity name='team' from='teamid' to='teamid'>
                          <attribute name='name' />
                          <attribute name='regardingobjectid' />
                          <filter type='and'>
                            <condition attribute='regardingobjectid' operator='eq' value='{txtEntityId.Text}'/>
                          </filter>
                          <link-entity name='teamtemplate' from='teamtemplateid' to='teamtemplateid'>
                            <attribute name='teamtemplatename' />
                            <filter type='and'>
                              <condition attribute='teamtemplateid' operator='eq' value='{(lvTeams.SelectedItems[0].Tag as Entity).GetAttributeValue<EntityReference>("teamtemplateid").Id.ToString()}'/>
                            </filter>
                          </link-entity>
                        </link-entity>
                      </entity>
                      <!---->
                    </fetch>";

                    LogInfo(fetchXml);

                    args.Result = DynamicsServiceHelper.GetMultiple(Service, fetchXml);

                    

                },
                PostWorkCallBack = (args) =>
                {
                    if (args.Error != null)
                    {
                        MessageBox.Show(args.Error.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    var result = args.Result as List<Entity>;
                    LogInfo(result.Count.ToString());
                    if (result != null)
                    {
                        lvTeamMembers.Items.Clear();
                        foreach (var member in result)
                        {
                            var item = new ListViewItem { Text = member.GetValue<string>("systemuser1.fullname"), Tag = member };
                            lvTeamMembers.Items.Add(item);
                        }
                    }
                }
            });
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExecuteMethod(ProcessRetrieveEntities);
        }

        private void ProcessRetrieveEntities()
        {
            cBoxEntities.Items.Clear();

            WorkAsync(new WorkAsyncInfo
            {
                Message = "Retrieving Entities...",
                AsyncArgument = null,
                Work = (bw, e) =>
                {
                    var entityQueryExpression = new EntityQueryExpression
                    {
                        Properties = new MetadataPropertiesExpression("LogicalName", "DisplayName","ObjectTypeCode", "PrimaryNameAttribute", "PrimaryIdAttribute")
                    };
                    var retrieveMetadataChangesRequest = new RetrieveMetadataChangesRequest
                    {
                        Query = entityQueryExpression,
                        ClientVersionStamp = null
                    };

                    var emd = ((RetrieveMetadataChangesResponse)Service.Execute(retrieveMetadataChangesRequest)).EntityMetadata;
                    var e1 = new List<EntityMetadata>();
                    e1.AddRange(emd);
                    e.Result = e1;
                    
                },
                PostWorkCallBack = e =>
                {
                    if (e.Error != null)
                    {
                        MessageBox.Show(this, "An error occured: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        var emds = (List<EntityMetadata>)e.Result;

                        emds = emds.OrderBy(x => x.LogicalName).ToList();

                        foreach (var emd in emds)
                        {
                            cBoxEntities.Items.Add(new EntityInfo(emd));
                        }

                        cBoxEntities.DrawMode = DrawMode.OwnerDrawFixed;
                        cBoxEntities.DrawItem += cbbEntity_DrawItem;

                        cBoxEntities.SelectedIndex = 0;
                        btnGetTeams.Enabled = true;
                    }
                },
                ProgressChanged = e => { SetWorkingMessage(e.UserState.ToString()); }
            });
        }

        private void cbbEntity_DrawItem(object sender, DrawItemEventArgs e)
        {
            // Draw the default background
            e.DrawBackground();
            e.DrawFocusRectangle();

            if (e.Index == -1) return;

            // The ComboBox is bound to a DataTable,
            // so the items are DataRowView objects.
            var attr = (EntityInfo)((ComboBox)sender).Items[e.Index];

            // Retrieve the value of each column.
            string displayName = attr.DisplayName;
            string logicalName = attr.LogicalName;

            // Get the bounds for the first column
            Rectangle r1 = e.Bounds;
            r1.Width /= 2;

            // Draw the text on the first column
            using (SolidBrush sb = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(displayName, e.Font, sb, r1);
            }

            // Get the bounds for the second column
            Rectangle r2 = e.Bounds;
            r2.X = e.Bounds.Width / 2;
            r2.Width /= 2;

            // Draw the text on the second column
            using (SolidBrush sb = new SolidBrush(e.ForeColor))
            {
                e.Graphics.DrawString(logicalName, e.Font, sb, r2);
            }
        }
    }
}