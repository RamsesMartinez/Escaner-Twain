namespace Escaner_Twain
{
    partial class frmEscaner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEscaner));
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.btnSaveImage = new System.Windows.Forms.Button();
            this.menu = new System.Windows.Forms.MenuStrip();
            this.archivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.elegirDispositivoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.salirToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.pictureBoxEscaner = new System.Windows.Forms.PictureBox();
            this.buttonZoomMas = new System.Windows.Forms.Button();
            this.buttonZoomMenos = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.menu.SuspendLayout();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEscaner)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadImage
            // 
            resources.ApplyResources(this.btnLoadImage, "btnLoadImage");
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // btnSaveImage
            // 
            resources.ApplyResources(this.btnSaveImage, "btnSaveImage");
            this.btnSaveImage.Name = "btnSaveImage";
            this.btnSaveImage.UseVisualStyleBackColor = true;
            this.btnSaveImage.Click += new System.EventHandler(this.btnSaveImage_Click);
            // 
            // menu
            // 
            this.menu.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.menu.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
            this.menu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.archivoToolStripMenuItem});
            resources.ApplyResources(this.menu, "menu");
            this.menu.Name = "menu";
            this.menu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            // 
            // archivoToolStripMenuItem
            // 
            this.archivoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.elegirDispositivoToolStripMenuItem,
            this.salirToolStripMenuItem});
            this.archivoToolStripMenuItem.Name = "archivoToolStripMenuItem";
            resources.ApplyResources(this.archivoToolStripMenuItem, "archivoToolStripMenuItem");
            // 
            // elegirDispositivoToolStripMenuItem
            // 
            this.elegirDispositivoToolStripMenuItem.Name = "elegirDispositivoToolStripMenuItem";
            resources.ApplyResources(this.elegirDispositivoToolStripMenuItem, "elegirDispositivoToolStripMenuItem");
            this.elegirDispositivoToolStripMenuItem.Click += new System.EventHandler(this.elegirDispositivoToolStripMenuItem_Click);
            // 
            // salirToolStripMenuItem
            // 
            this.salirToolStripMenuItem.Name = "salirToolStripMenuItem";
            resources.ApplyResources(this.salirToolStripMenuItem, "salirToolStripMenuItem");
            this.salirToolStripMenuItem.Click += new System.EventHandler(this.salirToolStripMenuItem_Click);
            // 
            // Panel1
            // 
            resources.ApplyResources(this.Panel1, "Panel1");
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.pictureBoxEscaner);
            this.Panel1.Name = "Panel1";
            // 
            // pictureBoxEscaner
            // 
            resources.ApplyResources(this.pictureBoxEscaner, "pictureBoxEscaner");
            this.pictureBoxEscaner.BackColor = System.Drawing.SystemColors.ControlDark;
            this.pictureBoxEscaner.BackgroundImage = global::Escaner_Twain.Properties.Resources.doc1;
            this.pictureBoxEscaner.Name = "pictureBoxEscaner";
            this.pictureBoxEscaner.TabStop = false;
            // 
            // buttonZoomMas
            // 
            resources.ApplyResources(this.buttonZoomMas, "buttonZoomMas");
            this.buttonZoomMas.ForeColor = System.Drawing.SystemColors.InfoText;
            this.buttonZoomMas.Name = "buttonZoomMas";
            this.buttonZoomMas.UseVisualStyleBackColor = true;
            this.buttonZoomMas.Click += new System.EventHandler(this.buttonZoomMas_Click);
            // 
            // buttonZoomMenos
            // 
            resources.ApplyResources(this.buttonZoomMenos, "buttonZoomMenos");
            this.buttonZoomMenos.ForeColor = System.Drawing.SystemColors.InfoText;
            this.buttonZoomMenos.Name = "buttonZoomMenos";
            this.buttonZoomMenos.UseVisualStyleBackColor = true;
            this.buttonZoomMenos.Click += new System.EventHandler(this.buttonZoomMenos_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // frmEscaner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonZoomMenos);
            this.Controls.Add(this.buttonZoomMas);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.btnSaveImage);
            this.Controls.Add(this.btnLoadImage);
            this.Controls.Add(this.menu);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.Name = "frmEscaner";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmEscaner_Load);
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxEscaner)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxEscaner;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.Button btnSaveImage;
        private System.Windows.Forms.MenuStrip menu;
        private System.Windows.Forms.ToolStripMenuItem archivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem elegirDispositivoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem salirToolStripMenuItem;
        private System.Windows.Forms.Panel Panel1;
        private System.Windows.Forms.Button buttonZoomMas;
        private System.Windows.Forms.Button buttonZoomMenos;
        private System.Windows.Forms.Label label1;
    }
}