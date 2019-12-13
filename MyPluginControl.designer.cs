namespace AccessTeamViewer
{
    partial class MyPluginControl
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.toolStripMenu = new System.Windows.Forms.ToolStrip();
            this.tsbClose = new System.Windows.Forms.ToolStripButton();
            this.tssSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.gbTeams = new System.Windows.Forms.GroupBox();
            this.lvTeams = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.gbMembers = new System.Windows.Forms.GroupBox();
            this.lvTeamMembers = new System.Windows.Forms.ListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.gbEntity = new System.Windows.Forms.GroupBox();
            this.btnGetTeams = new System.Windows.Forms.Button();
            this.txtEntityId = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cBoxEntities = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.toolStripMenu.SuspendLayout();
            this.gbTeams.SuspendLayout();
            this.gbMembers.SuspendLayout();
            this.gbEntity.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenu
            // 
            this.toolStripMenu.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStripMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbClose,
            this.tssSeparator1});
            this.toolStripMenu.Location = new System.Drawing.Point(0, 0);
            this.toolStripMenu.Name = "toolStripMenu";
            this.toolStripMenu.Size = new System.Drawing.Size(900, 25);
            this.toolStripMenu.TabIndex = 4;
            this.toolStripMenu.Text = "toolStrip1";
            // 
            // tsbClose
            // 
            this.tsbClose.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbClose.Name = "tsbClose";
            this.tsbClose.Size = new System.Drawing.Size(86, 22);
            this.tsbClose.Text = "Close this tool";
            this.tsbClose.Click += new System.EventHandler(this.tsbClose_Click);
            // 
            // tssSeparator1
            // 
            this.tssSeparator1.Name = "tssSeparator1";
            this.tssSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // gbTeams
            // 
            this.gbTeams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbTeams.Controls.Add(this.lvTeams);
            this.gbTeams.Location = new System.Drawing.Point(3, 147);
            this.gbTeams.Name = "gbTeams";
            this.gbTeams.Size = new System.Drawing.Size(894, 100);
            this.gbTeams.TabIndex = 6;
            this.gbTeams.TabStop = false;
            this.gbTeams.Text = "Access Teams";
            // 
            // lvTeams
            // 
            this.lvTeams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTeams.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            this.lvTeams.HideSelection = false;
            this.lvTeams.Location = new System.Drawing.Point(6, 19);
            this.lvTeams.Name = "lvTeams";
            this.lvTeams.Size = new System.Drawing.Size(874, 75);
            this.lvTeams.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvTeams.TabIndex = 0;
            this.lvTeams.UseCompatibleStateImageBehavior = false;
            this.lvTeams.View = System.Windows.Forms.View.Details;
            this.lvTeams.SelectedIndexChanged += new System.EventHandler(this.lvTeams_SelectedIndexChanged);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Team Name";
            this.columnHeader3.Width = 346;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Access Level";
            this.columnHeader4.Width = 474;
            // 
            // gbMembers
            // 
            this.gbMembers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbMembers.Controls.Add(this.lvTeamMembers);
            this.gbMembers.Location = new System.Drawing.Point(3, 253);
            this.gbMembers.Name = "gbMembers";
            this.gbMembers.Size = new System.Drawing.Size(894, 189);
            this.gbMembers.TabIndex = 7;
            this.gbMembers.TabStop = false;
            this.gbMembers.Text = "Team Members ";
            // 
            // lvTeamMembers
            // 
            this.lvTeamMembers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvTeamMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5});
            this.lvTeamMembers.HideSelection = false;
            this.lvTeamMembers.Location = new System.Drawing.Point(6, 19);
            this.lvTeamMembers.Name = "lvTeamMembers";
            this.lvTeamMembers.Size = new System.Drawing.Size(874, 164);
            this.lvTeamMembers.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lvTeamMembers.TabIndex = 0;
            this.lvTeamMembers.UseCompatibleStateImageBehavior = false;
            this.lvTeamMembers.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "User";
            this.columnHeader5.Width = 348;
            // 
            // gbEntity
            // 
            this.gbEntity.Controls.Add(this.button1);
            this.gbEntity.Controls.Add(this.label2);
            this.gbEntity.Controls.Add(this.cBoxEntities);
            this.gbEntity.Controls.Add(this.btnGetTeams);
            this.gbEntity.Controls.Add(this.txtEntityId);
            this.gbEntity.Controls.Add(this.label1);
            this.gbEntity.Location = new System.Drawing.Point(3, 34);
            this.gbEntity.Name = "gbEntity";
            this.gbEntity.Size = new System.Drawing.Size(554, 107);
            this.gbEntity.TabIndex = 8;
            this.gbEntity.TabStop = false;
            this.gbEntity.Text = "Entity";
            // 
            // btnGetTeams
            // 
            this.btnGetTeams.Enabled = false;
            this.btnGetTeams.Location = new System.Drawing.Point(467, 65);
            this.btnGetTeams.Name = "btnGetTeams";
            this.btnGetTeams.Size = new System.Drawing.Size(75, 23);
            this.btnGetTeams.TabIndex = 2;
            this.btnGetTeams.Text = "Get Teams";
            this.btnGetTeams.UseVisualStyleBackColor = true;
            this.btnGetTeams.Click += new System.EventHandler(this.btnGetTeams_Click);
            // 
            // txtEntityId
            // 
            this.txtEntityId.Location = new System.Drawing.Point(80, 67);
            this.txtEntityId.Name = "txtEntityId";
            this.txtEntityId.Size = new System.Drawing.Size(381, 20);
            this.txtEntityId.TabIndex = 1;
            this.txtEntityId.Text = "92A7A695-1015-EA11-A811-000D3A86D756";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 67);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Entity Id";
            // 
            // cBoxEntities
            // 
            this.cBoxEntities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cBoxEntities.FormattingEnabled = true;
            this.cBoxEntities.Location = new System.Drawing.Point(80, 30);
            this.cBoxEntities.Name = "cBoxEntities";
            this.cBoxEntities.Size = new System.Drawing.Size(381, 21);
            this.cBoxEntities.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Entity Type";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(467, 28);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Get Entities";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MyPluginControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbEntity);
            this.Controls.Add(this.gbMembers);
            this.Controls.Add(this.gbTeams);
            this.Controls.Add(this.toolStripMenu);
            this.Name = "MyPluginControl";
            this.Size = new System.Drawing.Size(900, 445);
            this.Load += new System.EventHandler(this.MyPluginControl_Load);
            this.toolStripMenu.ResumeLayout(false);
            this.toolStripMenu.PerformLayout();
            this.gbTeams.ResumeLayout(false);
            this.gbMembers.ResumeLayout(false);
            this.gbEntity.ResumeLayout(false);
            this.gbEntity.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStripMenu;
        private System.Windows.Forms.ToolStripButton tsbClose;
        private System.Windows.Forms.ToolStripSeparator tssSeparator1;
        private System.Windows.Forms.GroupBox gbTeams;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.GroupBox gbMembers;
        private System.Windows.Forms.GroupBox gbEntity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGetTeams;
        private System.Windows.Forms.TextBox txtEntityId;
        private System.Windows.Forms.ListView lvTeams;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ListView lvTeamMembers;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cBoxEntities;
        private System.Windows.Forms.Button button1;
    }
}
