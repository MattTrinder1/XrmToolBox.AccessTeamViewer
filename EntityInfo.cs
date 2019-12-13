using AccessTeamViewer;
using Microsoft.Xrm.Sdk.Metadata;

namespace MsCrmTools.AuditCenter
{
    public class EntityInfo
    {
        public EntityInfo(EntityMetadata emd)
        {
            Emd = emd;

            LogicalName = emd.LogicalName;
            DisplayName = emd.DisplayName?.UserLocalizedLabel?.Label ?? "N/A";
            PrimaryAttribute = emd.PrimaryNameAttribute;
        }

        public string DisplayName { get; private set; }
        public string LogicalName { get; private set; }
        public string PrimaryAttribute { get; private set; }

        public MyPluginControl.ActionState Action { get; set; }
        public EntityMetadata Emd { get; set; }
        public bool InitialState { get; set; }
    }
}