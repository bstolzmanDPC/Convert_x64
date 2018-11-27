using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Convert_x64
{
    
    public partial class Dialog : Form
    {
        File f;

        public Dialog()
        {
            InitializeComponent();
            f = new File();
        }

        public File GetFileInfo()
        {
            return f;
        }

        private void ChooseConvertPath(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txtConvertPath.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void ChooseLogPath(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {
                txtLogPath.Text = folderBrowserDialog2.SelectedPath;
            }
        }

        private void Dialog_Load(object sender, EventArgs e)
        {
            btnConvertPath.Click += new System.EventHandler(ChooseConvertPath);
            btnLogPath.Click += new System.EventHandler(ChooseLogPath);
        }
        
        private void btnConvert_Click(object sender, EventArgs e)
        {
            if(CheckEntries())
            {
                f.Path = txtConvertPath.Text;
                f.LogPath = txtLogPath.Text;
                DialogResult = DialogResult.OK;
            }
        }

        private bool CheckEntries()
        {
            bool flag = true;

            if (string.IsNullOrWhiteSpace(txtConvertPath.Text) || string.IsNullOrWhiteSpace(txtLogPath.Text))
                flag = false;

            return flag;
        }
    }
}
