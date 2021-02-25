using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        List<string> _files;
        private void FormTinifyClient_Load(object sender, EventArgs e)
        {
            //MessageBox.Show("hi rohan");
            _files = new List<string>();
            Tinify.Key = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxx";
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var imgPath = @"C:\Users\iqbal\Desktop\Image rnd\2000-4K-2\4k-3840-x-2160-wallpapers-themefoxx (366).jpg";
                
                var source = await Tinify.FromFile(imgPath);
                await source.ToFile(imgPath + ".compressed.jpg");
                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
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

                }

            }
            catch (Exception)
            {

            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            _files = new Utilities().GetAllImages(txtFolderPath.Text, true);
           
            lblInfo.Text = $"Total Files: {_files.Count}";
        }

        private async void btnComAll_Click(object sender, EventArgs e)
        {

            try
            {
                foreach (string file in _files)
                { 
                    var source = await Tinify.FromFile(file);
                    await source.ToFile(file + $".compressed{Path.GetExtension(file)}");
                }
                MessageBox.Show("Done");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
