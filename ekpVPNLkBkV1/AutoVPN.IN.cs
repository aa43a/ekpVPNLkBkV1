using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ekpVPNLkBkV1
{
    partial class AutoVPN
    {

        public void RemoveTAPVir()
        {
            Process cmd = new Process();
            StartProcess(cmd);
            cmd.StandardInput.WriteLine("c:");
            cmd.StandardInput.WriteLine(@"cd " + Global.path + @"\openvpn\driver");
            if (Environment.Is64BitOperatingSystem)
                cmd.StandardInput.WriteLine("devcon /r remove tap0901");
            else
            {
                cmd.StandardInput.WriteLine("devcon_1 /r remove tap0901");
                FileStream fs = new FileStream(Global.path + @"\openvpn\driver\count.out", FileMode.Open);
                //FileStream fs = new FileStream(@"C:\Users\zhangsf\Desktop\OpenVPN\config\route.ovpn", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                StreamWriter sw = new StreamWriter(fs);
                string strline;
                strline = sr.ReadLine();
                fs.SetLength(0);
                sw.WriteLine("0");
                sw.Close();
                sr.Close();
                fs.Close();
                return;
            }             
            EndProcess(cmd);
        }


        private void StartProcess(Process cmd)
        {
            cmd.StartInfo.FileName = "cmd.exe";

            cmd.StartInfo.UseShellExecute = false;

            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.RedirectStandardError = false;
            cmd.StartInfo.CreateNoWindow = true;

            cmd.Start();
        }

        private void EndProcess(Process cmd)
        {
            cmd.StandardInput.Flush();

            cmd.StandardInput.Close();
            cmd.WaitForExit();
            cmd.Close();
        }

        private void  IsVirExist() {
            try
            {
                Process cmd = new Process();
                Global.virExist = false;
                StartProcess(cmd);
                cmd.StandardInput.WriteLine("c:");
                cmd.StandardInput.WriteLine(@"cd " + Global.path + @"\openvpn\driver");
                //cmd.StandardInput.WriteLine(@"cd C:\Users\zhangsf\Desktop\tap-windows\driver");
                if (Environment.Is64BitOperatingSystem)
                {
                    cmd.StandardInput.WriteLine("devcon hwids tap0901");

                    StreamReader reader = cmd.StandardOutput;
                    string line = reader.ReadLine();//每次读取一行
                    while (line != null)
                    {
                        line = reader.ReadLine();
                        Console.WriteLine(line);
                        if (line == "2 matching device(s) found.")
                        {
                            Global.virExist = true;
                            reader.Close();
                            EndProcess(cmd);
                            Console.WriteLine(Global.virExist);
                            return;
                        }
                        if (line == "No matching devices found.")
                        {
                            Global.virExist = false;
                            reader.Close();
                            EndProcess(cmd);
                            Console.WriteLine(Global.virExist);
                            return;
                        }
                    }
                    // Console.WriteLine("hello?????");
                    // Console.WriteLine(line);
                    reader.Close();
                    EndProcess(cmd);
                }
                else
                {
                    //cmd.StandardInput.WriteLine("devcon_1 hwids tap0901");
                    string strline;
                    FileStream fs = new FileStream(Global.path + @"\openvpn\driver\count.out", FileMode.Open);
                    //FileStream fs = new FileStream(@"C:\Users\zhangsf\Desktop\OpenVPN\config\route.ovpn", FileMode.Open);
                    StreamReader sr = new StreamReader(fs);
                    StreamWriter sw = new StreamWriter(fs);
                    strline = sr.ReadLine();
                    if (strline == "0")
                    {
                        Global.virExist = false;
                        sw.Close();
                        sr.Close();
                        fs.Close();
                        return;
                    }
                    if (strline == "1")
                    {
                        Global.virExist = true;
                        sw.Close();
                        sr.Close();
                        fs.Close();
                        return;
                    }
                    sw.Close();
                    sr.Close();
                    fs.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            // return false;
        }

        public void finishOther()
        {
            int con = 0;
            Process pro = new Process();


                foreach (Process p in Process.GetProcesses())
                {
                    if (p.ProcessName == "openvpn")
                    {
                        con++;
                        Global.isLink = true;
                    }
                }
                Console.WriteLine(Global.isLink + "  hello.");
                foreach (Process p in Process.GetProcesses())
                {

                    if (con != 0)
                    {
                        //Console.WriteLine(p.ProcessName);
                        if (p.ProcessName == "conhost")
                        {
                            p.Kill();
                            con++;
                        }
                        if (p.ProcessName == "cmd")
                        {
                            p.Kill();
                            con++;
                        }
                    }
                }
        }

        public void KillProcess()
        {
            Process pro = new Process();
            foreach (Process p in Process.GetProcesses())
            {
                //    Console.WriteLine(p.ProcessName);
                if (p.ProcessName == "openvpn")
                {
                    p.Kill();
                }
            }
            Global.isRoute = false;
            Global.isLink = false;
            Global.getip = true;
                     
            Global.statu1 = "";
            Global.statu2 = "";
            finishOther();
        }

        public void ChangeRouteIP(string ip) {
            try
            {
                string strline;
                List<string> srw = new List<string>();
                // ip = "10.8.0.49";
                FileStream fs = new FileStream(Global.path + @"\openvpn\route.ovpn", FileMode.Open);
                //FileStream fs = new FileStream(@"C:\Users\zhangsf\Desktop\OpenVPN\config\route.ovpn", FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                StreamWriter sw = new StreamWriter(fs);
                strline = sr.ReadLine();
                Global.routeTxt = "";
                while (strline != null)
                {
                    Console.WriteLine(strline);
                    if (strline.StartsWith("remote"))
                    {
                        srw.Add("remote " + ip + " 10088");
                    }
                    else
                    {
                        srw.Add(strline);
                    }
                    strline = sr.ReadLine();
                }
                fs.SetLength(0);
                foreach (string sinw in srw)
                {
                    sw.WriteLine(sinw);
                    Global.routeTxt += sinw + "_______";
                }
                sw.Close();
                sr.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
           
            }
        }

        public void GetIP() {
            //string tempip = "";
            //try
            //{
            //    WebRequest wr = WebRequest.Create("http://www.ip138.com/ip2city.asp");
            //    Stream s = wr.GetResponse().GetResponseStream();
            //    StreamReader sr = new StreamReader(s, Encoding.Default);
            //    string all = sr.ReadToEnd(); //读取网站的数据

            //    int start = all.IndexOf("[") + 1;
            //    int end = all.IndexOf("]", start);
            //    tempip = all.Substring(start, end - start);
            //    sr.Close();
            //    s.Close();
            //}
            //catch
            //{
            //}
            string tempip = "";
            try
            {
                WebRequest wr = WebRequest.Create("http://www.ip138.com/ips138.asp");
                Stream s = wr.GetResponse().GetResponseStream();
                StreamReader sr = new StreamReader(s, Encoding.Default);
                string all = sr.ReadToEnd(); //读取网站的数据
                int start = all.IndexOf("您的IP地址是：[") + 9;
                int end = all.IndexOf("]", start);
                tempip = all.Substring(start, end - start);
                sr.Close();
                s.Close();
            }
            catch
            {
            }
            //return tempip;
            Global.outip = tempip;

        }

        public  string showMessage()
        {
            return Global.s;
        }

        public void SetCount() {
            try
            {
                FileStream fs = new FileStream(Global.path + @"\openvpn\ss.txt", FileMode.Open);
                //FileStream fs = new FileStream(@"C:\Users\zhangsf\Desktop\OpenVPN\config\route.ovpn", FileMode.Open);
               // StreamReader sr = new StreamReader(fs);
                StreamWriter sw = new StreamWriter(fs);
                fs.SetLength(0);
                Random rd = new Random();
                int rdd = rd.Next(1, 200);
                sw.WriteLine("test" + rdd);
                sw.WriteLine("test" + rdd);
                Global.currentCount = "当前账号为:test" + rdd;
                Console.WriteLine("test" + rdd);

               // sr.Close();
                sw.Close();
                fs.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
