using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeIT.MemCached;

namespace ekpVPNLkBkV1
{
    class MemGet
    {
        private static MemcachedClient cache;

        public static void StartMemCa() {

            MemcachedClient.Setup("MyCache", new string[] { "115.29.172.236:11211" });

           cache = MemcachedClient.GetInstance("MyCache");

            //MemcachedClient configFileCache = MemcachedClient.GetInstance("MyConfigFileCache");

            cache.SendReceiveTimeout = 5000;
            cache.ConnectTimeout = 5000;
            cache.MinPoolSize = 1;
            cache.MaxPoolSize = 5;
           
        }

        public static string MemcacheGet(string value) {
            string[] h = new string[4];
            try
            {
                //ulong unique;
                Console.WriteLine(cache.Get(value) as string + "11");
                string s = cache.Get(value) as string;
                if (s.Split('.').Length == 4)
                {
                    h[0] = s.Split('.')[0].Split('"')[1];
                    h[1] = s.Split('.')[1];
                    h[2] = s.Split('.')[2];
                    h[3] = s.Split('.')[3].Split('"')[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return cache.Get(value) as string;
            }
            return h[0] + "." + h[1] + "." + h[2] + "." + h[3];

        }

        public static void MemcacheSet(string key,string value)
        {
            try
            {
               cache.Set(key,value);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
