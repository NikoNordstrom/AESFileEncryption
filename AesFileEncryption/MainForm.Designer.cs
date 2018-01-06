namespace AesFileEncryption
{
    partial class MainForm
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
            this.pathTxt = new System.Windows.Forms.TextBox();
            this.pathLabel = new System.Windows.Forms.Label();
            this.browseBtn = new System.Windows.Forms.Button();
            this.log = new System.Windows.Forms.RichTextBox();
            this.encBtn = new System.Windows.Forms.Button();
            this.decBtn = new System.Windows.Forms.Button();
            this.pwdTxt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.encDecGb = new System.Windows.Forms.GroupBox();
            this.showBtn = new System.Windows.Forms.Button();
            this.deleteFile = new System.Windows.Forms.CheckBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IsFolder = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.percentLabel = new System.Windows.Forms.Label();
            this.bgWorker = new System.ComponentModel.BackgroundWorker();
            this.encDecGb.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // pathTxt
            // 
            this.pathTxt.BackColor = System.Drawing.SystemColors.Window;
            this.pathTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathTxt.Location = new System.Drawing.Point(50, 29);
            this.pathTxt.Name = "pathTxt";
            this.pathTxt.ReadOnly = true;
            this.pathTxt.Size = new System.Drawing.Size(436, 20);
            this.pathTxt.TabIndex = 0;
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pathLabel.Location = new System.Drawing.Point(12, 32);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(32, 13);
            this.pathLabel.TabIndex = 1;
            this.pathLabel.Text = "Path:";
            // 
            // browseBtn
            // 
            this.browseBtn.BackColor = System.Drawing.SystemColors.Control;
            this.browseBtn.Location = new System.Drawing.Point(492, 27);
            this.browseBtn.Name = "browseBtn";
            this.browseBtn.Size = new System.Drawing.Size(80, 23);
            this.browseBtn.TabIndex = 2;
            this.browseBtn.Text = "Browse";
            this.browseBtn.UseVisualStyleBackColor = true;
            this.browseBtn.Click += new System.EventHandler(this.browseBtn_Click);
            // 
            // log
            // 
            this.log.DetectUrls = false;
            this.log.Location = new System.Drawing.Point(12, 220);
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.Size = new System.Drawing.Size(474, 150);
            this.log.TabIndex = 3;
            this.log.Text = "";
            // 
            // encBtn
            // 
            this.encBtn.BackColor = System.Drawing.SystemColors.Control;
            this.encBtn.Enabled = false;
            this.encBtn.Location = new System.Drawing.Point(191, 76);
            this.encBtn.Name = "encBtn";
            this.encBtn.Size = new System.Drawing.Size(80, 35);
            this.encBtn.TabIndex = 4;
            this.encBtn.Text = "Encrypt";
            this.encBtn.UseVisualStyleBackColor = true;
            this.encBtn.Click += new System.EventHandler(this.encBtn_Click);
            // 
            // decBtn
            // 
            this.decBtn.BackColor = System.Drawing.SystemColors.Control;
            this.decBtn.Enabled = false;
            this.decBtn.Location = new System.Drawing.Point(277, 76);
            this.decBtn.Name = "decBtn";
            this.decBtn.Size = new System.Drawing.Size(80, 35);
            this.decBtn.TabIndex = 4;
            this.decBtn.Text = "Decrypt";
            this.decBtn.UseVisualStyleBackColor = true;
            this.decBtn.Click += new System.EventHandler(this.decBtn_Click);
            // 
            // pwdTxt
            // 
            this.pwdTxt.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pwdTxt.Location = new System.Drawing.Point(135, 44);
            this.pwdTxt.Name = "pwdTxt";
            this.pwdTxt.PasswordChar = '•';
            this.pwdTxt.Size = new System.Drawing.Size(222, 26);
            this.pwdTxt.TabIndex = 5;
            this.pwdTxt.TextChanged += new System.EventHandler(this.pwdTxt_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Password:";
            // 
            // encDecGb
            // 
            this.encDecGb.Controls.Add(this.showBtn);
            this.encDecGb.Controls.Add(this.deleteFile);
            this.encDecGb.Controls.Add(this.pwdTxt);
            this.encDecGb.Controls.Add(this.label2);
            this.encDecGb.Controls.Add(this.encBtn);
            this.encDecGb.Controls.Add(this.decBtn);
            this.encDecGb.Enabled = false;
            this.encDecGb.Location = new System.Drawing.Point(12, 69);
            this.encDecGb.Name = "encDecGb";
            this.encDecGb.Size = new System.Drawing.Size(474, 145);
            this.encDecGb.TabIndex = 7;
            this.encDecGb.TabStop = false;
            this.encDecGb.Text = "Encryption/Decryption";
            // 
            // showBtn
            // 
            this.showBtn.Location = new System.Drawing.Point(363, 43);
            this.showBtn.Name = "showBtn";
            this.showBtn.Size = new System.Drawing.Size(45, 28);
            this.showBtn.TabIndex = 8;
            this.showBtn.Text = "Show";
            this.showBtn.UseVisualStyleBackColor = true;
            this.showBtn.Click += new System.EventHandler(this.showBtn_Click);
            // 
            // deleteFile
            // 
            this.deleteFile.AutoSize = true;
            this.deleteFile.Location = new System.Drawing.Point(76, 86);
            this.deleteFile.Name = "deleteFile";
            this.deleteFile.Size = new System.Drawing.Size(109, 17);
            this.deleteFile.TabIndex = 7;
            this.deleteFile.Text = "Delete original file";
            this.deleteFile.UseVisualStyleBackColor = true;
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(584, 24);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetAllToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // resetAllToolStripMenuItem
            // 
            this.resetAllToolStripMenuItem.Name = "resetAllToolStripMenuItem";
            this.resetAllToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.resetAllToolStripMenuItem.Text = "Reset all";
            this.resetAllToolStripMenuItem.Click += new System.EventHandler(this.resetAllToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            // 
            // IsFolder
            // 
            this.IsFolder.AutoSize = true;
            this.IsFolder.Location = new System.Drawing.Point(492, 56);
            this.IsFolder.Name = "IsFolder";
            this.IsFolder.Size = new System.Drawing.Size(55, 17);
            this.IsFolder.TabIndex = 9;
            this.IsFolder.Text = "Folder";
            this.IsFolder.UseVisualStyleBackColor = true;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 376);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(474, 23);
            this.progressBar.TabIndex = 10;
            // 
            // percentLabel
            // 
            this.percentLabel.AutoSize = true;
            this.percentLabel.Location = new System.Drawing.Point(492, 382);
            this.percentLabel.Name = "percentLabel";
            this.percentLabel.Size = new System.Drawing.Size(24, 13);
            this.percentLabel.TabIndex = 11;
            this.percentLabel.Text = "0 %";
            // 
            // bgWorker
            // 
            this.bgWorker.WorkerReportsProgress = true;
            this.bgWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgWorker_DoWork);
            this.bgWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgWorker_ProgressChanged);
            this.bgWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgWorker_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(584, 411);
            this.Controls.Add(this.percentLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.IsFolder);
            this.Controls.Add(this.encDecGb);
            this.Controls.Add(this.log);
            this.Controls.Add(this.browseBtn);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.pathTxt);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "AES File Encryption";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.encDecGb.ResumeLayout(false);
            this.encDecGb.PerformLayout();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox pathTxt;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Button browseBtn;
        private System.Windows.Forms.RichTextBox log;
        private System.Windows.Forms.Button encBtn;
        private System.Windows.Forms.Button decBtn;
        private System.Windows.Forms.TextBox pwdTxt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox encDecGb;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.CheckBox IsFolder;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label percentLabel;
        private System.Windows.Forms.ToolStripMenuItem resetAllToolStripMenuItem;
        private System.ComponentModel.BackgroundWorker bgWorker;
        private System.Windows.Forms.CheckBox deleteFile;
        private System.Windows.Forms.Button showBtn;
    }
}

