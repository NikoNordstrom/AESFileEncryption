using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Threading;

namespace AesFileEncryption
{
    public partial class MainForm : Form
    {
        private string path;
        private string pwd;

        public MainForm()
        {
            InitializeComponent();
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            log.AppendText("Select path to the file or folder.\n");
        }

        private void browseBtn_Click(object sender, EventArgs e)
        {
            if (IsFolder.Checked)
            {
                using (FolderBrowserDialog fbd = new FolderBrowserDialog())
                {
                    DialogResult result = fbd.ShowDialog();

                    if (result == DialogResult.OK && Directory.Exists(fbd.SelectedPath))
                    {
                        path = fbd.SelectedPath;
                        pathTxt.Text = path;
                        encDecGb.Enabled = true;

                        log.Clear();
                        log.AppendText(string.Format("Folder \"{0}\" is now selected.\n", path));
                        log.AppendText("Now enter a password. That password will be used to encrypt/decrypt all the files in the folder.\n");
                    }
                    else if (result == DialogResult.OK && !Directory.Exists(fbd.SelectedPath))
                    {
                        log.AppendText("Selected folder doesn't exist!\n");
                    }
                } 
            }
            else
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    DialogResult result = ofd.ShowDialog();

                    if (result == DialogResult.OK && File.Exists(ofd.FileName))
                    {
                        path = ofd.FileName;
                        pathTxt.Text = path;
                        encDecGb.Enabled = true;

                        if (path.Contains(".encrypted"))
                        {
                            decBtn.Enabled = true;
                            encBtn.Enabled = false;
                        }
                        else
                        {
                            decBtn.Enabled = false;
                            encBtn.Enabled = true;
                        }

                        log.Clear();
                        log.AppendText(string.Format("File \"{0}\" is now selected.\n", path));
                        log.AppendText("Now enter a password. That password will be used to encrypt/decrypt the selected file.\n");
                    }
                    else if (result == DialogResult.OK && !File.Exists(ofd.FileName))
                    {
                        log.AppendText("Selected file doesn't exist!\n");
                    }
                }
            }
        }

        private void encBtn_Click(object sender, EventArgs e)
        {
            pwd = pwdTxt.Text;

            if (!string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(path))
            {
                string fileName = new FileInfo(path).Name;
                LogAppend(string.Format("Encrypting file {0}...", fileName));

                Gui(false);
                bgWorker.RunWorkerAsync(0);
            }
        }

        private void decBtn_Click(object sender, EventArgs e)
        {
            pwd = pwdTxt.Text;

            if (!string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(path))
            {
                string fileName = new FileInfo(path).Name;
                LogAppend(string.Format("Decrypting file {0}...", fileName));

                Gui(false);
                bgWorker.RunWorkerAsync(1);
            }
        }

        private void EncryptFolder()
        {
            
        }

        private void DecryptFolder()
        {

        }

        public void LogAppend(string text)
        {
            log.AppendText(text + "\n");
        }

        private void Gui(bool enabled)
        {
            if (enabled == true)
            {
                encDecGb.Enabled = true;
                pathTxt.Enabled = true;
                browseBtn.Enabled = true;
                IsFolder.Enabled = true;
                pathLabel.Enabled = true;
                menuStrip.Enabled = true;
            }
            else if (enabled == false)
            {
                encDecGb.Enabled = false;
                pathTxt.Enabled = false;
                browseBtn.Enabled = false;
                IsFolder.Enabled = false;
                pathLabel.Enabled = false;
                menuStrip.Enabled = false;
            }
        }

        private void pwdTxt_TextChanged(object sender, EventArgs e)
        {
            if (pwdTxt.Text != string.Empty && !path.Contains(".encrypted"))
            {
                encBtn.Enabled = true;
            }
            else if (pwdTxt.Text != string.Empty && path.Contains(".encrypted"))
            {
                encBtn.Enabled = false;
                decBtn.Enabled = true;
            }
            else
            {
                encBtn.Enabled = false;
                decBtn.Enabled = false;
            }
        }

        private void resetAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            path = null;
            pwd = null;

            pathTxt.Text = string.Empty;
            pwdTxt.Text = string.Empty;
            encDecGb.Enabled = false;
            IsFolder.Checked = false;
            percentLabel.Text = "0 %";
            progressBar.Value = 0;

            log.Clear();
            log.AppendText("Select path to the file or folder.\n");
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if ((int)e.Argument == 0)
            {
                if (deleteFile.Checked == false)
                {
                    AesCrypt.Encrypt(path, pwd, true, sender as BackgroundWorker);
                }
                else if (deleteFile.Checked == true)
                {
                    AesCrypt.Encrypt(path, pwd, false, sender as BackgroundWorker);
                }

                e.Result = 0;
            }
            else if ((int)e.Argument == 1)
            {
                if (deleteFile.Checked == false)
                {
                    AesCrypt.Decrypt(path, pwd, true, sender as BackgroundWorker);
                }
                else if (deleteFile.Checked == true)
                {
                    AesCrypt.Decrypt(path, pwd, false, sender as BackgroundWorker);
                }

                e.Result = 1;
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            percentLabel.Text = progressBar.Value.ToString() + " %";
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null && progressBar.Value == 100)
            {
                if ((int)e.Result == 0)
                {
                    string fileName = new FileInfo(path).Name;
                    log.AppendText(string.Format("Successfully encrypted file {0}!\n", fileName));
                }
                else if ((int)e.Result == 1)
                {
                    string fileName = new FileInfo(path).Name;
                    log.AppendText(string.Format("Successfully decrypted file {0}!\n", fileName));
                }

                Gui(true);
                SystemSounds.Asterisk.Play();
            }
            else if (e.Error != null)
            {
                Gui(true);
                log.AppendText(e.Error.Message + "\n");
            }
        }

        private void showBtn_Click(object sender, EventArgs e)
        {
            if (pwdTxt.PasswordChar == '\u2022')
            {
                pwdTxt.PasswordChar = '\0';
            }
            else if (pwdTxt.PasswordChar == '\0')
            {
                pwdTxt.PasswordChar = '\u2022';
            }
        }
    }
}
