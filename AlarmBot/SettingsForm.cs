using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AlarmBot
{
    public partial class SettingsForm : Form
    {
        string currentChange = "0000";
        string formatString = "({0}; {1})";

        string pilotName;
        Point startPos;
        Point takeControlPosition;
        Point beginView;
        Point endView;

        public SettingsForm()
        {
            InitializeComponent();
            DownloadSettings();
            ShowData();
            pictureBox1.Image = Screen.getScreen(0, 0, 1024, 768);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Screen.getScreen(0, 0, 1024, 768);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            currentChange = "1000";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            currentChange = "0100";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            currentChange = "0010";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            currentChange = "0001";
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            switch (currentChange)
            {
                case "1000":
                    {
                        button3.Text = String.Format(formatString, e.X, e.Y);
                        startPos = e.Location;
                        currentChange = "0000";
                        break;
                    }
                case "0100":
                    {
                        button4.Text = String.Format(formatString, e.X, e.Y);
                        takeControlPosition = e.Location;
                        currentChange = "0000";
                        break;
                    }
                case "0010":
                    {
                        button5.Text = String.Format(formatString, e.X, e.Y);
                        beginView = e.Location;
                        currentChange = "0000";
                        break;
                    }
                case "0001":
                    {
                        button6.Text = String.Format(formatString, e.X, e.Y);
                        endView = e.Location;
                        currentChange = "0000";
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void DownloadSettings()
        {
            startPos = Properties.Settings.Default.StartPosition;
            pilotName = Properties.Settings.Default.PilotName;
            takeControlPosition = Properties.Settings.Default.TakeControlPosition;
            beginView = Properties.Settings.Default.BeginView;
            endView = Properties.Settings.Default.EndView;
        }

        private void UploadSettings()
        {
            Properties.Settings.Default.StartPosition = startPos;
            Properties.Settings.Default.PilotName = pilotName;
            Properties.Settings.Default.TakeControlPosition = takeControlPosition;
            Properties.Settings.Default.BeginView = beginView;
            Properties.Settings.Default.EndView = endView;
            Properties.Settings.Default.Save();
        }

        private void ShowData()
        {
            button3.Text = String.Format(formatString, startPos.X, startPos.Y);
            button4.Text = String.Format(formatString, takeControlPosition.X, takeControlPosition.Y);
            button5.Text = String.Format(formatString, beginView.X, beginView.Y);
            button6.Text = String.Format(formatString, endView.X, endView.Y);
            textBox1.Text = pilotName;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            pilotName = textBox1.Text;
            UploadSettings();
            this.Close();
        }
    }
}
