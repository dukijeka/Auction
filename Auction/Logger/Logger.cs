using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Auction.Logger
{
    public class Logger
    {
        

        private static string path = HttpContext.Current.Server.MapPath("~/App_Data/log.txt");

        private Logger() { }
        

        public static void log(string message)
        {
            File.AppendAllText(path, DateTime.UtcNow + " ----> " + message + "\n");
        }

        public static void newLog()
        {
            File.AppendAllText(path,  " ------------------------------------ " + "\n");
        }
        
        public static void clearLog()
        {
            File.WriteAllText(path, "");
        }
    }
}