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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            nowStatus = CurrentStatus.Wait;
            alarm = new Alarm();
        }

        public enum CurrentStatus { Invite, Write, Launch, Wait }

        Alarm alarm;

        public CurrentStatus nowStatus = CurrentStatus.Wait;

        private void button1_Click(object sender, EventArgs e)
        {
            SettingsForm form2 = new SettingsForm();
            form2.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DiscordMessager.MainAsync().GetAwaiter();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime a = DateTime.Now;
            if (a.Hour == 13 && a.Minute > 45)
            {
                DiscordMessager.s?.Invoke();
                nowStatus = CurrentStatus.Wait;
            }
            if (DiscordMessager.IsLaunchPilotMode)
            {
                return;
            }
            if (DiscordMessager.IsInviterMode && nowStatus == CurrentStatus.Wait)
            {
                alarm.Start();
                nowStatus = CurrentStatus.Invite;
            }
            if (!DiscordMessager.IsLaunchPilotMode && !DiscordMessager.IsInviterMode)
            {
                DiscordMessager.s?.Invoke();
                nowStatus = CurrentStatus.Wait;
            }
        }
    }
}
