using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ekpVPNLkBkV1
{
    public partial class AutoVPN
    {

        public void CheckAndAddVir()
        {          
            IsVirExist();
            if (!Global.virExist)
            {
                Thread th = new Thread(TAPVirINstall);
                Thread ts = new Thread(TAPVirINstall);
                th.Start();
                ts.Start();               
                Global.virExist = true;
            }
            //OpenVpnToRoute();
        }


        public void TAPVirINstall()
        {
            // Global.virExist = false;
            Process cmd = new Process();

            StartProcess(cmd);
            cmd.StandardInput.WriteLine("c:");
            cmd.StandardInput.WriteLine(@"cd " + Global.path + @"\openvpn\driver");
            //cmd.StandardInput.WriteLine(@"cd C:\Users\zhangsf\Desktop\tap-windows\driver");
            if (Environment.Is64BitOperatingSystem)
                cmd.StandardInput.WriteLine("devcon -r install OemWin2k.inf tap0901");
            else
            {
                cmd.StandardInput.WriteLine("devcon_1 -r install OemWin2k_1.inf tap0901");
                FileStream fs = new FileStream(Global.path + @"\openvpn\driver\count.out", FileMode.Open);
                //FileStream fs = new FileStream(@"C:\Users\zhangsf\Desktop\OpenVPN\config\route.ovpn", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                StreamWriter sw = new StreamWriter(fs);
                string strline;
                strline = sr.ReadLine();
                fs.SetLength(0);
                sw.WriteLine("1");
                sw.Close();
                sr.Close();
                fs.Close();
                return;

            }
            StreamReader reader = cmd.StandardOutput;
            string line = reader.ReadLine();//每次读取一行
            while (!reader.EndOfStream)
            {
                Console.WriteLine(line);
                
                line = reader.ReadLine();
                Global.s += line + "\n";
            }
            finishOther();
            EndProcess(cmd);
        }

        

        public void OpenVpnToServer()
        {

            if (Global.virExist)
            {
               // Thread.Sleep(8000);
                Global.isLink = false;
                foreach (Process prc in Process.GetProcesses())
                {
                    if (prc.ProcessName == "openvpn")
                    {
                        Global.isLink = true;
                        return;
                    }
                }
                //if (Global.statu1 == "平台已连接")
                //{
                //    Global.isLink = true;
                //    return;
                //}
                if (!Global.isLink)
                {
                    Global.countserver++;
                    Process cmd = new Process();
                    StartProcess(cmd);
                    cmd.StandardInput.WriteLine("c:");
                    cmd.StandardInput.WriteLine(@"cd " + Global.path + @"\openvpn");
                   // cmd.StandardInput.WriteLine(@"cd C:\Users\zhangsf\Desktop\OpenVPN\config");
                    cmd.StandardInput.WriteLine("openvpn --config web.ovpn --auth-user-pass ss.txt");
                   
                    
                    StreamReader reader = cmd.StandardOutput;
                    string line = reader.ReadLine();//每次读取一行
                    while (!reader.EndOfStream)
                    {
                        Console.WriteLine(line);
                       
                        line = reader.ReadLine();
                        if (line.Split(' ').Length == 8 &&
                            line.Split(' ')[5] == "Initialization" &&
                            line.Split(' ')[6] == "Sequence" &&
                            line.Split(' ')[7] == "Completed")
                        {
                            Global.isLink = true;
                            Global.statu1 = "平台已连接";
                            if(Global.nip!="")
                            ChangeRouteIP(Global.nip);
                        }
                        Global.s += line + "\n";
                    }

                    finishOther();
                    reader.Close();
                    EndProcess(cmd);
                    return;
                }
            }
        }

        public void WaitToGetip()
        {
            try
            {
                Global.nip = MemGet.MemcacheGet(Global.outip + "_siteip");

                if (Global.nip.Split('.').Length == 4 && !Global.nip.Equals(Global.oip) &&
                    !Global.nip.Split('.')[1].Equals("") && !Global.nip.Split('.')[1].Equals(null))
                {
                    Global.oip = Global.nip;
                    ChangeRouteIP(Global.nip);
                    Global.getip = true;
                  //  Global.isgetip = true;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }           
        }

        public void OpenVpnToRoute()
        {
            if (Global.virExist && Global.isLink)
            {
                // Thread.Sleep(8000);
                Global.isRoute = false;
                int count = 0;
                foreach (Process prc in Process.GetProcesses())
                {
                    if (prc.ProcessName == "openvpn")
                    {
                        count++;
                    }
                }
                if (count == 2)
                {
                    if (Global.statu1 == Global.nip + "已连接")
                    {
                        Global.isRoute = true;
                        return;
                    }
                }
                
                if (!Global.isRoute)
                {
                    Global.countroute++;
                    Process cmd = new Process();
                    StartProcess(cmd);
                    cmd.StandardInput.WriteLine("c:");
                    cmd.StandardInput.WriteLine(@"cd " + Global.path + @"\openvpn");
                   // cmd.StandardInput.WriteLine(@"cd C:\Users\zhangsf\Desktop\OpenVPN\config");
                    cmd.StandardInput.WriteLine("openvpn --config route.ovpn");
                    
                    Global.isRoute = true;
                    StreamReader reader = cmd.StandardOutput;
                    string line = reader.ReadLine();//每次读取一行
                    while (!reader.EndOfStream)
                    {                      
                        Console.WriteLine(line);
                       
                        line = reader.ReadLine();
                        if (line.Split(' ').Length == 8 &&
                           line.Split(' ')[5] == "Initialization" &&
                           line.Split(' ')[6] == "Sequence" &&
                           line.Split(' ')[7] == "Completed")
                        {
                            Global.isRouted = true;
                            Global.statu2 = Global.nip + "已连接";                         
                        }
                        Global.s += line + "\n";
                    }
                    finishOther();
                    reader.Close();
                    EndProcess(cmd);
                    return;
                }
            }
        }
    }    
}
