namespace Egel_Mods_Installer
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.install = new System.Windows.Forms.Button();
            this.progress = new System.Windows.Forms.Label();
            this.uninstall = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.error = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // install
            // 
            this.install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.install.Location = new System.Drawing.Point(48, 100);
            this.install.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.install.Name = "install";
            this.install.Size = new System.Drawing.Size(150, 75);
            this.install.TabIndex = 0;
            this.install.Text = "Installeren";
            this.install.UseVisualStyleBackColor = false;
            this.install.Click += new System.EventHandler(this.install_Click);
            // 
            // progress
            // 
            this.progress.AutoSize = true;
            this.progress.Location = new System.Drawing.Point(32, 203);
            this.progress.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.progress.MaximumSize = new System.Drawing.Size(400, 300);
            this.progress.MinimumSize = new System.Drawing.Size(400, 0);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(400, 18);
            this.progress.TabIndex = 1;
            this.progress.Text = "Klik op installeren om te beginnen";
            this.progress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // uninstall
            // 
            this.uninstall.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.uninstall.Location = new System.Drawing.Point(266, 100);
            this.uninstall.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.uninstall.Name = "uninstall";
            this.uninstall.Size = new System.Drawing.Size(150, 75);
            this.uninstall.TabIndex = 2;
            this.uninstall.Text = "Deïnstalleren";
            this.uninstall.UseVisualStyleBackColor = false;
            this.uninstall.Click += new System.EventHandler(this.uninstall_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(464, 72);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(104, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Egel Mods Installer";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // error
            // 
            this.error.AutoSize = true;
            this.error.ForeColor = System.Drawing.Color.Red;
            this.error.Location = new System.Drawing.Point(32, 249);
            this.error.MaximumSize = new System.Drawing.Size(400, 300);
            this.error.MinimumSize = new System.Drawing.Size(400, 0);
            this.error.Name = "error";
            this.error.Size = new System.Drawing.Size(400, 18);
            this.error.TabIndex = 4;
            this.error.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 321);
            this.Controls.Add(this.error);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.uninstall);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.install);
            this.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Form1";
            this.Text = "Egel Mods Installer";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button install;
        private System.Windows.Forms.Label progress;
        private System.Windows.Forms.Button uninstall;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label error;
    }
}

