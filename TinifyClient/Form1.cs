using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TinifyAPI;
using TinifyClient.FileUtilities;

namespace TinifyClient
{
    public partial class FormTinifyClient : Form
    {
        public FormTinifyClient()
        {
            InitializeComponent();
        }
        List<string> _files = new List<string>();
        List<string> _largeFiles = new List<string>();
        List<FileInfo> _fileinfoes = new List<FileInfo>();
        private NetTraffic _netTraffic = new NetTraffic();
        string _conversionLog;

        private void FormTinifyClient_Load(object sender, EventArgs e)
        {
            _netTraffic.DataReceiving += _netTraffic_DataReceiving;

        }
        private void _netTraffic_DataReceiving(object sender, List<NetTrafficEventArg> e)
        {
            if (e.Count == 0) //no live network
            {
                
                return;

            }

            string msg = ""; // $"Down: {FileSizeFormatter.FormatSize(e[0].DownloadSpeed)}/S  , UP: {FileSizeFormatter.FormatSize(e[0].UploadSpeed)}/S";
            foreach (NetTrafficEventArg arg in e)
            {
                msg += $"Network: {arg.NetworkInterface.Name}:   Down: {FileSizeFormatter.FormatSize(arg.DownloadSpeed)}/s  , UP: {FileSizeFormatter.FormatSize(arg.UploadSpeed)}/s\r\n";
            }
                lblNetSpeed.Text = msg;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    txtFolderPath.Text = dialog.SelectedPath;
                    btnCalculate.Enabled = true;
                    btnCalculate_Click(null, null);
                }

            }
            catch (Exception)
            {

            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            string msg = "";
            _files = new Utilities().GetAllImages(txtFolderPath.Text, true);
           
            lblInfo.Text = $"Total Files: {_files.Count}";
            btnComAll.Enabled = _files.Count > 0;
            //ShowMsg("Getting files info.....");
            var totalFile = _files.Count;
            long totalSize = 0;
            long MAX_SIZE = 2005 * 1024 * 1024;//5MB//no from API
            _fileinfoes = new List<FileInfo>();
            foreach (string file in _files)
            {
                var fi = new FileInfo(file);
                if (fi.Length > MAX_SIZE)
                {
                    _largeFiles.Add(file);
                }
                else
                {
                    _fileinfoes.Add(fi);
                       totalSize += fi.Length;
                }
            }
            msg = $"Total files {totalFile}, Total size {FileSizeFormatter.FormatSize(totalSize)}. Large files ( can't compress files) {_largeFiles.Count}";
            lblSummary.Text = msg;

            _fileinfoes = _fileinfoes.OrderByDescending(f => f.Length).ToList();
        }

        private async void btnComAll_Click(object sender, EventArgs e)
        {
            string workingOnFileOriginalName = "";
            string workingOnFile = "";
            _conversionLog ="SUCCESS_"+ DateTime.Now.ToString("yy.MM.dd.HH.mm.ss")+".log";
            if (string.IsNullOrEmpty(txtAPIKEY.Text))
            {
                MessageBox.Show("Please give a API key.");
                txtAPIKEY.Focus();
                return;
            }
            Tinify.Key = txtAPIKEY.Text;
            string msg = "";
            string selectedPx = comboBox1.Text;
            await Task.Run(async () =>
            {
                try
                {
                    int count = 0;

                    foreach (FileInfo fileInfo in _fileinfoes)
                    {
                        count++;

                        msg = $"Compressing {count} of {_fileinfoes.Count}";
                        ShowMsg(msg);
                        var destPath = fileInfo.FullName.Replace(txtFolderPath.Text, txtDest.Text);

                        var destpathforcheck = Path.GetDirectoryName(destPath);
                        Utilities.CreateFolderIfNotExists(destpathforcheck);
                        workingOnFileOriginalName = fileInfo.FullName;
                        workingOnFile = fileInfo.FullName;

                        //File.Copy(fileInfo.FullName, destPath,true);// to check the logic work perfectly before go live
                        int px = 0;
                        int.TryParse(selectedPx.Replace("px",""), out px);
                        if (px > 0)
                        {
                            workingOnFile = Utilities.ResizeImage(workingOnFile, px);
                        }
                        if (chkUseTinify.Checked)
                        {
                            var source = await Tinify.FromFile(workingOnFile);
                            await source.ToFile(destPath);
                        }
                        File.AppendAllText(_conversionLog, fileInfo.FullName + "\r\n");//logging 
                        if (chkMove.Checked)
                        {
                            var donePath = txtFolderPath.Text + "\\Completed"; 
                            var doneFullPath = Path.GetDirectoryName(fileInfo.FullName.Replace(txtFolderPath.Text, donePath));
                            Utilities.CreateFolderIfNotExists(doneFullPath);
                            File.Move(fileInfo.FullName, doneFullPath+"\\"+ fileInfo.Name, true);
                        }
                        msg = $"{count} files are compressed of {_fileinfoes.Count} files. Remaining {_fileinfoes.Count - count} files ";
                        ShowMsg(msg);
                    }
                    msg = $"{_fileinfoes.Count} files are compressed.";
                    ShowMsg(msg);
                    File.AppendAllText(_conversionLog, msg + "\r\n");
                    MessageBox.Show("Done");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message+"  \r\n"+ workingOnFileOriginalName);
                }
            });
        }
        private void ShowMsg(string msg)
        {
            this.InvokeOnUiThreadIfRequired(() =>
            {
                lblInfo.Text = msg;
            });
        }
        //test a single file compression
        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                //Tinify.Key = txtAPIKEY.Text;

                var imgPath = @"C:\Users\iqbal\Desktop\Image rnd\20210224_185348.jpg";

                int px = 0;
                int.TryParse(comboBox1.Text.Replace("px", ""), out px);
                var zx=  Utilities.ResizeImage(imgPath,px);

                //if (comboBox1.Text == "1080px")
                //{
                    
                //}
                //var source = await Tinify.FromFile(imgPath);
                //await source.ToFile(imgPath + ".compressed.jpg");
                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDest_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                    txtDest.Text = dialog.SelectedPath; 
                }

            }
            catch (Exception)
            {

            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                this.toolTip1.SetToolTip(this.lblres, $"if the resolution is greater than {comboBox1.Text.ToString()} then reduce it to {comboBox1.Text.ToString()} then convert by tinypng.com\r\n");
            }
            catch (Exception ex)
            { 
            }
        }
    }
}
