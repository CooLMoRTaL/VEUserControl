namespace VEarth
{
    partial class ucVEarth
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.vEarthBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // vEarthBrowser
            // 
            this.vEarthBrowser.AllowNavigation = false;
            this.vEarthBrowser.AllowWebBrowserDrop = false;
            this.vEarthBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.vEarthBrowser.IsWebBrowserContextMenuEnabled = false;
            this.vEarthBrowser.Location = new System.Drawing.Point(0, 0);
            this.vEarthBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.vEarthBrowser.Name = "vEarthBrowser";
            this.vEarthBrowser.ScrollBarsEnabled = false;
            this.vEarthBrowser.Size = new System.Drawing.Size(345, 326);
            this.vEarthBrowser.TabIndex = 0;
            this.vEarthBrowser.WebBrowserShortcutsEnabled = false;
            this.vEarthBrowser.Resize += new System.EventHandler(this.vEarthBrowser_Resize);
            this.vEarthBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this.vEarthBrowser_DocumentCompleted);
            // 
            // ucVEarth
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.vEarthBrowser);
            this.Name = "ucVEarth";
            this.Size = new System.Drawing.Size(345, 326);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser vEarthBrowser;
    }
}
