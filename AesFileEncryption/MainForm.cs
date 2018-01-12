using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Media;
using System.Text.RegularExpressions;

namespace AesFileEncryption
{
    public partial class MainForm : Form
    {
        private string path;
        private string pwd;
        private bool IsDirectory;

        private int completedFiles = 0;
        private int allFilesInt = 0;

        public MainForm()
        {
            InitializeComponent();
        }
        
        private void MainForm_Load(object sender, EventArgs e)
        {
            LogAppend("Select path to the file or folder.");
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
                        IsDirectory = true;
                        encDecGb.Enabled = true;

                        progressBar.Value = 0;
                        percentLabel.Text = progressBar.Value.ToString() + " %";

                        completedFiles = 0;
                        allFilesInt = 0;
                        fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);

                        log.Clear();
                        LogAppend(string.Format("Folder \"{0}\" is now selected.", path));
                        LogAppend("Now enter a password. That password will be used to encrypt/decrypt all files in the folder.");
                    }
                    else if (result == DialogResult.OK && !Directory.Exists(fbd.SelectedPath))
                    {
                        LogAppend("Selected folder doesn't exist!");
                    }
                } 
            }
            else if (!IsFolder.Checked)
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    DialogResult result = ofd.ShowDialog();

                    if (result == DialogResult.OK && File.Exists(ofd.FileName))
                    {
                        path = ofd.FileName;
                        pathTxt.Text = path;
                        IsDirectory = false;
                        encDecGb.Enabled = true;

                        progressBar.Value = 0;
                        percentLabel.Text = progressBar.Value.ToString() + " %";

                        completedFiles = 0;
                        allFilesInt = 1;
                        fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);

                        log.Clear();
                        LogAppend(string.Format("File \"{0}\" is now selected.", path));
                        LogAppend("Now enter a password. That password will be used to encrypt/decrypt the selected file.");
                    }
                    else if (result == DialogResult.OK && !File.Exists(ofd.FileName))
                    {
                        LogAppend("Selected file doesn't exist!");
                    }
                }
            }
        }

        private void encBtn_Click(object sender, EventArgs e)
        {
            pwd = pwdTxt.Text;
            path = pathTxt.Text;

            progressBar.Value = 0;
            percentLabel.Text = progressBar.Value.ToString() + " %";

            string[] args = new string[2];
            args[0] = path;
            args[1] = "encrypt";

            if (!string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(path) && IsDirectory == false)
            {
                if (!File.Exists(path))
                {
                    LogAppend("File was not found.");
                    return;
                }

                if (path.Contains(".encrypted"))
                {
                    LogAppend("File is already encrypted.");
                    return;
                }

                string fileName = new FileInfo(path).Name;
                LogAppend(string.Format("Encrypting file {0}...", fileName));

                completedFiles = 0;
                allFilesInt = 1;
                fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);

                Gui(false);
                bgWorker.RunWorkerAsync(args);
            }
            else if (!string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(path) && IsDirectory == true)
            {
                EncryptFolder();
            }
        }

        private void decBtn_Click(object sender, EventArgs e)
        {
            pwd = pwdTxt.Text;
            path = pathTxt.Text;

            progressBar.Value = 0;
            percentLabel.Text = progressBar.Value.ToString() + " %";

            string[] args = new string[2];
            args[0] = path;
            args[1] = "decrypt";

            if (!string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(path) && IsDirectory == false)
            {
                if (!File.Exists(path))
                {
                    LogAppend("File was not found.");
                    return;
                }

                if (!path.Contains(".encrypted"))
                {
                    LogAppend("File is already decrypted.");
                    return;
                }

                string fileName = new FileInfo(path).Name;
                LogAppend(string.Format("Decrypting file {0}...", fileName));

                completedFiles = 0;
                allFilesInt = 1;
                fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);

                Gui(false);
                bgWorker.RunWorkerAsync(args);
            }
            else if (!string.IsNullOrEmpty(pwd) && !string.IsNullOrEmpty(path) && IsDirectory == true)
            {
                DecryptFolder();
            }
        }

        private void EncryptFolder()
        {
            Gui(false);

            completedFiles = 0;
            allFilesInt = 0;

            string[] allFiles = Directory.GetFiles(path);

            for (int i = 0; i < allFiles.Count(); i++)
            {
                string fileExt = new FileInfo(allFiles[i]).Extension;

                if (fileExt != ".encrypted")
                {
                    allFilesInt++;
                }
            }

            fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);

            if (allFilesInt == 0)
            {
                LogAppend("No files found.");
                Gui(true);
                return;
            }

            for (int i = 0; i < allFiles.Count(); i++)
            {
                string file = allFiles[i];

                string[] args = new string[2];
                args[0] = file;
                args[1] = "encrypt";

                string fileName = new FileInfo(file).Name;
                string fileExt = new FileInfo(file).Extension;

                if (fileExt == ".encrypted" && i == allFilesInt - 1)
                {
                    LogAppend("No suitable files found.");
                    Gui(true);
                    continue;
                }
                else if (fileExt == ".encrypted")
                {
                    continue;
                }

                if (bgWorker.IsBusy)
                {
                    while (bgWorker.IsBusy)
                    {
                        Application.DoEvents();

                        if (!bgWorker.IsBusy)
                        {
                            LogAppend(string.Format("Encrypting file {0}...", fileName));
                            bgWorker.RunWorkerAsync(args);
                            break;
                        }
                    }
                }
                else if (!bgWorker.IsBusy)
                {
                    LogAppend(string.Format("Encrypting file {0}...", fileName));
                    bgWorker.RunWorkerAsync(args);
                }
            }
        }

        private void DecryptFolder()
        {
            Gui(false);

            completedFiles = 0;
            allFilesInt = 0;

            string[] allFiles = Directory.GetFiles(path);

            for (int i = 0; i < allFiles.Count(); i++)
            {
                string fileExt = new FileInfo(allFiles[i]).Extension;

                if (fileExt == ".encrypted")
                {
                    allFilesInt++;
                }
            }

            fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);

            if (allFilesInt == 0)
            {
                LogAppend("No files found.");
                Gui(true);
                return;
            }

            for (int i = 0; i < allFiles.Count(); i++)
            {
                string file = allFiles[i];

                string[] args = new string[2];
                args[0] = file;
                args[1] = "decrypt";

                string fileExt = new FileInfo(file).Extension;
                string fileName = new FileInfo(file).Name;

                if (fileExt != ".encrypted" && i == allFilesInt - 1)
                {
                    LogAppend("No suitable files found.");
                    Gui(true);
                    continue;
                }
                else if (fileExt != ".encrypted")
                {
                    continue;
                }

                if (bgWorker.IsBusy)
                {
                    while (bgWorker.IsBusy)
                    {
                        Application.DoEvents();

                        if (!bgWorker.IsBusy)
                        {
                            LogAppend(string.Format("Decrypting file {0}...", fileName));
                            bgWorker.RunWorkerAsync(args);
                            break;
                        }
                    }
                }
                else if (!bgWorker.IsBusy)
                {
                    LogAppend(string.Format("Decrypting file {0}...", fileName));
                    bgWorker.RunWorkerAsync(args);
                }
            }
        }

        public void LogAppend(string text)
        {
            log.AppendText(text + "\n");
        }

        private void Gui(bool enabled)
        {
            encDecGb.Enabled = enabled;
            pathTxt.Enabled = enabled;
            browseBtn.Enabled = enabled;
            IsFolder.Enabled = enabled;
            pathLabel.Enabled = enabled;
            menuStrip.Enabled = enabled;
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

            completedFiles = 0;
            allFilesInt = 0;
            fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);

            log.Clear();
            LogAppend("Select path to the file or folder.");
        }

        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string[] args = (string[])e.Argument;
            string filePath = args[0];
            string mode = args[1];

            if (mode == "encrypt")
            {
                if (deleteFile.Checked == false)
                {
                    AesCrypt.Encrypt(filePath, pwd, true, sender as BackgroundWorker);
                }
                else if (deleteFile.Checked == true)
                {
                    AesCrypt.Encrypt(filePath, pwd, false, sender as BackgroundWorker);
                }

                e.Result = args;
            }
            else if (mode == "decrypt")
            {
                if (deleteFile.Checked == false)
                {
                    AesCrypt.Decrypt(filePath, pwd, true, sender as BackgroundWorker);
                }
                else if (deleteFile.Checked == true)
                {
                    AesCrypt.Decrypt(filePath, pwd, false, sender as BackgroundWorker);
                }

                e.Result = args;
            }
        }

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            percentLabel.Text = progressBar.Value.ToString() + " %";

            if (progressBar.Value == 100)
            {
                completedFiles++;
                fileProgress.Text = string.Format("{0} / {1}", completedFiles, allFilesInt);
            }
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string[] results = (string[])e.Result;
            string filePath = results[0];
            string mode = results[1];

            if (e.Error == null && progressBar.Value == 100 && completedFiles == allFilesInt)
            {
                Gui(true);
            }
            if (e.Error == null && progressBar.Value == 100)
            {
                string fileName = new FileInfo(filePath).Name;
                LogAppend(string.Format("Successfully {0}ed file {1}!", mode, fileName));

                if (completedFiles == allFilesInt)
                {
                    SystemSounds.Asterisk.Play();
                }
            }
            else if (e.Error != null)
            {
                Gui(true);
                LogAppend(e.Error.Message);
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
