using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FTPUpload
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtAddress.Text = Properties.Settings.Default["FTPAddress"].ToString();
            txtUsername.Text = Properties.Settings.Default["FTPUsername"].ToString();
            txtPassword.Text = Properties.Settings.Default["FTPPassword"].ToString();
            txtLink.Text = Properties.Settings.Default["FTPLink"].ToString();
        }

        private bool exit = false;

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;

            if (!exit)
            {
                e.Cancel = true;
                Hide();
            }
        }

        //ftp://aktheknight.co.uk/public_html/stuff/

        private void btnUpload_Click(object sender, EventArgs e)
        {
            Debug.Print("Kappa");
            Stream myStream = null;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = false;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {
                            // Insert code to read the stream here
                            label1.Text = "DONE";

                            // Get the object used to communicate with the server.
                            FtpWebRequest request =
                                (FtpWebRequest)
                                WebRequest.Create(Properties.Settings.Default["FTPAddress"].ToString() + "/" +
                                                  openFileDialog1.SafeFileName);
                            request.Method = WebRequestMethods.Ftp.UploadFile;

                            // This example assumes the FTP site uses anonymous logon.
                            request.Credentials =
                                new NetworkCredential(Properties.Settings.Default["FTPUsername"].ToString(),
                                    Properties.Settings.Default["FTPPassword"].ToString());

                            // Copy the contents of the file to the request stream.
                            StreamReader sourceStream = new StreamReader(openFileDialog1.OpenFile());
                            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
                            sourceStream.Close();
                            request.ContentLength = fileContents.Length;

                            Stream requestStream = request.GetRequestStream();
                            requestStream.Write(fileContents, 0, fileContents.Length);
                            requestStream.Close();

                            FtpWebResponse response = (FtpWebResponse) request.GetResponse();

                            label1.Text = String.Format("Upload File Complete, status {0}", response.StatusDescription);

                            response.Close();

                            Clipboard.SetText(Properties.Settings.Default["FTPLink"] + openFileDialog1.SafeFileName);

                            Hide();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }

        }

        public void Upload()
        {
            OpenFileDialog dialog = GetFile(new OpenFileDialog());
            if (dialog.ShowDialog() == DialogResult.OK)
            {

            }

        }

        public OpenFileDialog GetFile(OpenFileDialog dialog)
        {
            dialog.InitialDirectory = "c:\\";
            dialog.RestoreDirectory = true;
            dialog.Multiselect = false;
            return dialog;
        }

        public void Transfer(OpenFileDialog dialog)
        {
            
        }

        public void MakeVisible()
        {
            Show();
            WindowState = FormWindowState.Normal;
        }
        

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["FTPAddress"] = txtAddress.Text;
            Properties.Settings.Default["FTPUsername"] = txtUsername.Text;
            Properties.Settings.Default["FTPPassword"] = txtPassword.Text;
            Properties.Settings.Default["FTPLink"] = txtLink.Text;
            Properties.Settings.Default.Save();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            tabControl1.SelectedTab = tabUpload;
            MakeVisible();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == WindowState)
                Hide();
        }

        private void uploadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Upload();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabSettings;
            MakeVisible();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, "Are you sure you want to close?", "Closing", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {
                exit = true;
                Application.Exit();
            }
        }
    }
}
