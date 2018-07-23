using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace weaReader
{
    public partial class Form1 : Form
    {
        string directory;
        
        List<LocationData> locdata;
        public Form1()
        {
            InitializeComponent();
            
        }
        

        private string[] getFiles()
        {

            //get projectfolder
            string[] files = null;
            folderBrowserDialog1.Description = "Folder with weather files";
            folderBrowserDialog1.SelectedPath = "D:\\WORK\\piloto\\Climate";
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK) // Test result.
            {

                directory = folderBrowserDialog1.SelectedPath;
                files = Directory.GetFiles(directory);

            }
                return files;
        }

        private void readWEA(object sender, EventArgs e)
        {
            //get the file to read or loop a load of files
            string[] files = getFiles();
            //should check there are wea
            WEAReader wea = new WEAReader(files);
            this.locdata = wea.getData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach(LocationData ld in this.locdata)
            {
                EPWWriter epw = new EPWWriter(ld, this.directory);
            }
        }
        private void writeJSON(object sender, EventArgs e)
        {
            foreach (LocationData ld in this.locdata)
            {
                ClimaJSONWriter writeCJ = new ClimaJSONWriter(ld, this.directory);
            }
        }
        private void readEPW(object sender, EventArgs e)
        {
            
            string[] files = getFiles();
            EPWReader epwr = new EPWReader(files);
            this.locdata = epwr.getData();
        }

        private void epwDwnBtn_Click(object sender, EventArgs e)
        {
            EPWDownload epwdown = new EPWDownload();
        }

        private void cityCountryBtn_Click(object sender, EventArgs e)
        {
            EPWDownload.makeCountriesCitiesObj();
        }
    }
}
