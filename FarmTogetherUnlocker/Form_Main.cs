using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmTogetherUnlocker
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();
        }

        private const string STEAM_INSTALL_NOT_FOUND = "Auto detect failed.";

        private Boolean steamInstallCorrect = false;
        private Boolean steamIdCorrect = false;

        private void Form_Main_Load(object sender, EventArgs e)
        {
            GetSteamPathAndDisplay();
        }

        private void GetSteamPathAndDisplay()
        {
            textBox1.Text = 
                (string)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\WOW6432Node\\Valve\\Steam", 
                "InstallPath", STEAM_INSTALL_NOT_FOUND);
        }

        private void Check()
        {
            steamInstallCorrect = textBox1.Text != string.Empty && Directory.Exists(textBox1.Text + "\\userdata");
            steamIdCorrect = textBox2.Text != string.Empty && Directory.Exists(textBox1.Text + "\\userdata\\" + textBox2.Text);

            pictureBox1.Image = steamInstallCorrect ? Properties.Resources.passed : Properties.Resources.failed;
            pictureBox2.Image = steamIdCorrect ? Properties.Resources.passed : Properties.Resources.failed;

            button2.Enabled = steamInstallCorrect && steamIdCorrect;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Check();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string gamePath = textBox1.Text + "\\userdata\\" + textBox2.Text + "\\673950\\";

            string tempPath = Path.GetTempPath() + "FarmTogetherUnlock\\";

            // make backup
            if (!File.Exists(gamePath + "remote\\farms.data.backup"))
            {
                File.Copy(gamePath + "remote\\farms.data", gamePath + "remote\\farms.data.backup");
            }
            // create temp directory
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }

            // decompress farms.data to farms.json
            GZipUtils.DecompressGZip(gamePath + "remote\\farms.data", tempPath + "farms.json");

            // replace rewards with embedded data
            var origin = JObject.Parse(File.ReadAllText(tempPath + "farms.json"));
            var newRewards = JObject.Parse(Properties.Resources.Rewards);

            origin["Rewards"] = newRewards["Rewards"];

            // save file to json
            File.WriteAllText(tempPath + "farms_new.json", origin.ToString(Newtonsoft.Json.Formatting.None));

            // compress farms_new.json to farms_new.data
            GZipUtils.CompressGZip(tempPath + "farms_new.json", tempPath + "farms_new.data");

            // remove old farms.data
            if (File.Exists(gamePath + "remote\\farms.data"))
            {
                File.Delete(gamePath + "remote\\farms.data");
            }

            // copy farms_new.data to game folder
            File.Copy(tempPath + "farms_new.data", gamePath + "remote\\farms.data");

            // remove temp directory
            //Directory.Delete(tempPath, true);

            MessageBox.Show("Unlock event items successful!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
