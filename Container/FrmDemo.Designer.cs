namespace Container
{
    partial class FrmDemo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDisplayAddresses = new System.Windows.Forms.Button();
            this.txtAddresses = new System.Windows.Forms.TextBox();
            this.btnHideLayer = new System.Windows.Forms.Button();
            this.btnShowLayer = new System.Windows.Forms.Button();
            this.btnGetDirections = new System.Windows.Forms.Button();
            this.btnDisplayLine = new System.Windows.Forms.Button();
            this.btnDisplayPolygon = new System.Windows.Forms.Button();
            this.lvDirections = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ucVEarth = new VEarth.ucVEarth();
            this.SuspendLayout();
            // 
            // btnDisplayAddresses
            // 
            this.btnDisplayAddresses.Location = new System.Drawing.Point(12, 117);
            this.btnDisplayAddresses.Name = "btnDisplayAddresses";
            this.btnDisplayAddresses.Size = new System.Drawing.Size(168, 23);
            this.btnDisplayAddresses.TabIndex = 1;
            this.btnDisplayAddresses.Text = "Display adresses";
            this.btnDisplayAddresses.UseVisualStyleBackColor = true;
            this.btnDisplayAddresses.Click += new System.EventHandler(this.btnDisplayAddresses_Click);
            // 
            // txtAddresses
            // 
            this.txtAddresses.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::Container.Properties.Settings.Default, "Search", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
            this.txtAddresses.Location = new System.Drawing.Point(12, 0);
            this.txtAddresses.Multiline = true;
            this.txtAddresses.Name = "txtAddresses";
            this.txtAddresses.Size = new System.Drawing.Size(168, 111);
            this.txtAddresses.TabIndex = 2;
            this.txtAddresses.Text = global::Container.Properties.Settings.Default.Search;
            // 
            // btnHideLayer
            // 
            this.btnHideLayer.Location = new System.Drawing.Point(12, 191);
            this.btnHideLayer.Name = "btnHideLayer";
            this.btnHideLayer.Size = new System.Drawing.Size(168, 23);
            this.btnHideLayer.TabIndex = 3;
            this.btnHideLayer.Text = "Hide Layer";
            this.btnHideLayer.UseVisualStyleBackColor = true;
            this.btnHideLayer.Click += new System.EventHandler(this.btnHideLayer_Click);
            // 
            // btnShowLayer
            // 
            this.btnShowLayer.Location = new System.Drawing.Point(12, 220);
            this.btnShowLayer.Name = "btnShowLayer";
            this.btnShowLayer.Size = new System.Drawing.Size(168, 23);
            this.btnShowLayer.TabIndex = 4;
            this.btnShowLayer.Text = "Show Layer";
            this.btnShowLayer.UseVisualStyleBackColor = true;
            this.btnShowLayer.Click += new System.EventHandler(this.btnShowLayer_Click);
            // 
            // btnGetDirections
            // 
            this.btnGetDirections.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetDirections.Location = new System.Drawing.Point(185, 325);
            this.btnGetDirections.Name = "btnGetDirections";
            this.btnGetDirections.Size = new System.Drawing.Size(174, 23);
            this.btnGetDirections.TabIndex = 5;
            this.btnGetDirections.Text = "Display driving directions";
            this.btnGetDirections.UseVisualStyleBackColor = true;
            this.btnGetDirections.Click += new System.EventHandler(this.btnGetDirections_Click);
            // 
            // btnDisplayLine
            // 
            this.btnDisplayLine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisplayLine.Location = new System.Drawing.Point(365, 325);
            this.btnDisplayLine.Name = "btnDisplayLine";
            this.btnDisplayLine.Size = new System.Drawing.Size(174, 23);
            this.btnDisplayLine.TabIndex = 7;
            this.btnDisplayLine.Text = "Display line";
            this.btnDisplayLine.UseVisualStyleBackColor = true;
            this.btnDisplayLine.Click += new System.EventHandler(this.btnDisplayLine_Click);
            // 
            // btnDisplayPolygon
            // 
            this.btnDisplayPolygon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDisplayPolygon.Location = new System.Drawing.Point(545, 325);
            this.btnDisplayPolygon.Name = "btnDisplayPolygon";
            this.btnDisplayPolygon.Size = new System.Drawing.Size(174, 23);
            this.btnDisplayPolygon.TabIndex = 8;
            this.btnDisplayPolygon.Text = "Display polygon";
            this.btnDisplayPolygon.UseVisualStyleBackColor = true;
            this.btnDisplayPolygon.Click += new System.EventHandler(this.btnDisplayPolygon_Click);
            // 
            // lvDirections
            // 
            this.lvDirections.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvDirections.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.lvDirections.Location = new System.Drawing.Point(186, 351);
            this.lvDirections.Name = "lvDirections";
            this.lvDirections.Size = new System.Drawing.Size(670, 159);
            this.lvDirections.TabIndex = 9;
            this.lvDirections.UseCompatibleStateImageBehavior = false;
            this.lvDirections.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Description";
            this.columnHeader1.Width = 133;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Distance";
            this.columnHeader2.Width = 133;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Duration";
            this.columnHeader3.Width = 133;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Distance so far";
            this.columnHeader4.Width = 133;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Duration so far";
            this.columnHeader5.Width = 133;
            // 
            // ucVEarth
            // 
            this.ucVEarth.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ucVEarth.AutoSize = true;
            this.ucVEarth.DashboardStyle = VEarth.ucVEarth.DashboardStyleEnum.Tiny;
            this.ucVEarth.DisambiguationMode = VEarth.ucVEarth.DisambiguationEnum.Ignore;
            this.ucVEarth.HTMLLocation = "C:\\VEUSerControl\\WindowsFormsApplication2\\1.jpg";
            this.ucVEarth.Location = new System.Drawing.Point(186, 0);
            this.ucVEarth.MapLocation = "The Netherlands";
            this.ucVEarth.MapStyle = VEarth.ucVEarth.MapStyleEnum.Aerial;
            this.ucVEarth.Name = "ucVEarth";
            this.ucVEarth.Size = new System.Drawing.Size(680, 316);
            this.ucVEarth.TabIndex = 0;
            this.ucVEarth.Load += new System.EventHandler(this.ucVEarth_Load);
            // 
            // FrmDemo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(868, 520);
            this.Controls.Add(this.lvDirections);
            this.Controls.Add(this.btnDisplayPolygon);
            this.Controls.Add(this.btnDisplayLine);
            this.Controls.Add(this.btnGetDirections);
            this.Controls.Add(this.btnShowLayer);
            this.Controls.Add(this.btnHideLayer);
            this.Controls.Add(this.txtAddresses);
            this.Controls.Add(this.btnDisplayAddresses);
            this.Controls.Add(this.ucVEarth);
            this.Name = "FrmDemo";
            this.Text = "Demo VEarth Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmDemo_FormClosing);
            this.Load += new System.EventHandler(this.FrmDemo_Load);
            this.Shown += new System.EventHandler(this.FrmDemo_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private VEarth.ucVEarth ucVEarth;
        private System.Windows.Forms.Button btnDisplayAddresses;
        private System.Windows.Forms.TextBox txtAddresses;
        private System.Windows.Forms.Button btnHideLayer;
        private System.Windows.Forms.Button btnShowLayer;
        private System.Windows.Forms.Button btnGetDirections;
        private System.Windows.Forms.Button btnDisplayLine;
        private System.Windows.Forms.Button btnDisplayPolygon;
        private System.Windows.Forms.ListView lvDirections;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}

