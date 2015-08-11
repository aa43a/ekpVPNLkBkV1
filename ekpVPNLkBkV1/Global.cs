using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ekpVPNLkBkV1
{
    class Global
    {
        public static bool virExist = false;
        public static bool isLink = false;
        public static bool getip = false;
        public static bool isRoute = false;
        public static bool isRouted = false;

        public static int countserver = 0;
        public static int countroute = 0;
        public static int getipCount = 0;

        public static string oip = "10.8.0.0";
        public static string nip = "";
        public static string outip = "";
        public static string statu1 = "";
        public static string statu2 = "";
        public static string currentCount = "";
        public static string routeTxt = "";

        public static string path = System.AppDomain.CurrentDomain.BaseDirectory;
        public static string s = "";

        //public static Process p1;
       // public static Process p2;
    }
}
