using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CSCron
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            CronService.Logging("program started");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
            notifyIcon1.ShowBalloonTip(3000, string.Empty, "service is running in background", ToolTipIcon.Info);
        }

        private void editJobsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CronService.OpenCronFile();
        }

        private void viewLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CronService.OpenLogFile();
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CronService.ClearLogFile();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CronService.Logging("program terminated");
        }
    }
}
