using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.Settings
{
    public class GlobalSettings
    {
        static GlobalSettings()
        {
            // default values
            N = 10;
            D = 60;
            S = 10;
            G = 20;
            P = 40;
            C = "RSD";
            T = 1;
        }

        // number of auctions to show
        public static int N { get; set; }

        // default auction Duration
        public static int D { get; set; }

        // silver package
        public static int S { get; set; }

        // gold [ackage
        public static int G { get; set; }

        // platinum package
        public static int P { get; set; }

        // currency
        public static string C { get; set; }

        // token value
        public static double T { get; set; }
    }
}