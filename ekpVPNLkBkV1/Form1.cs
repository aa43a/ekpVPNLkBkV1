using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ekpVPNLkBkV1
{
    public partial class Form1 : Form
    {
        private Thread thd, th,tr,tmem;
        public Form1()
        {
           // Console.WriteLine("hello????");
            InitializeComponent();
            Thread tk = new Thread(auv.KillProcess);
            Thread tsm = new Thread(MemGet.StartMemCa);
            Thread tri = new Thread(auv.GetIP);
            tk.Start();tsm.Start();tri.Start();
           // MemGet.MemcacheSet(Global.outip + "_siteip", Global.nip);
            textBox1.Text = Global.nip;
            auv.SetCount();
            //  auv.IsVirExist();
            //   Console.WriteLine(Environment.Is64BitOperatingSystem);
            this.Hide();
            //this.WindowState = FormWindowState.Minimized;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal && this.Visible == true)
            {
                this.notifyIcon1.Visible = true;//在通知区显示Form的Icon
                this.WindowState = FormWindowState.Minimized;
                this.Visible = false;
                this.ShowInTaskbar = false;//使Form不在任务栏上显示
            }
            if (this.WindowState == FormWindowState.Normal)
            {
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.WindowState = FormWindowState.Normal;
                this.Show() ;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)//当用户点击窗体右上角X按钮或(Alt + F4)时 发生           
            {
                e.Cancel = true;
                this.ShowInTaskbar = false;
                this.notifyIcon1.Icon = this.Icon;
                this.Hide();
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                myMenu.Show(MousePosition.X, MousePosition.Y);
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("你确定要退出程序吗？", "确认", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                notifyIcon1.Visible = false;
                this.Close();
                this.Dispose();
                Application.Exit();
                auv.KillProcess();
            }
           
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void timer4_Tick(object sender, EventArgs e)
        {
            textBox1.Text = auv.showMessage();
            if (auv.showMessage().Length > 4000) {
                Global.s = "";
            }
            textBox2.Text = Global.oip;
            textBox3.Text = Global.nip;
            textBox4.Text = Global.outip;
            textBox5.Text = Global.statu1;
            textBox6.Text = Global.statu2;
            textBox7.Text = Global.currentCount;
            textBox8.Text = Global.routeTxt;
            textBox9.Text = Global.isLink + " _____ " + Global.isRoute;
             //textBox5.Text = MemGet.MemcacheGet()
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer5_Tick(object sender, EventArgs e)
        {
            Thread tri = new Thread(auv.GetIP);
            tri.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            tmem= new Thread(auv.WaitToGetip);
            tmem.Start();
            if (Global.getip) {
                if (Global.isLink && !Global.isRoute && Global.statu1 == "平台已连接")
                {
                   // auv.ChangeRouteIP(Global.nip);
                    textBox1.Text = "";
                    Global.s = "";
                    tr = new Thread(auv.OpenVpnToRoute);
                    tr.Start();
                    Global.getip = false;
                }
                else if (Global.isLink && Global.isRoute) {
                   // tr.Abort();
                    auv.KillProcess();                           
                    //Global.getip = false;                  
                }
            }
        }

       

        private void timer3_Tick(object sender, EventArgs e)
        {
            Global.isLink = false;
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.ProcessName == "openvpn")
                {
                    Global.isLink = true;
                    return;
                }
            }
            if (!Global.isLink) {
                auv.KillProcess();
                textBox1.Text = "";
                Global.s = "";
                th = new Thread(auv.CheckAndAddVir);
                th.Start();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!Global.virExist) {
                 th = new Thread(auv.CheckAndAddVir);
                th.Start();
             //   Console.WriteLine("hello???");
            }

         
            if (!Global.isLink && Global.virExist && th.ThreadState == System.Threading.ThreadState.Stopped)
            {
               // textBox1.Text = "";
                Global.s = "";
                thd = new Thread(auv.OpenVpnToServer);
                thd.Start();
                if (thd.ThreadState == System.Threading.ThreadState.Stopped)
                {
                    Console.WriteLine("线程结束");
                }
                Global.isLink = true;               
            }
            //else {
            //    timer2.Stop();
            //}
            //if (thd != null)
            //    Console.WriteLine("thd:" + thd.ThreadState);
            //if (th != null)
            //{
            //    Console.WriteLine("th:" + th.ThreadState);
            //}
        }
    }
}
