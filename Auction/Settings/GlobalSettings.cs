using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Auction.Settings
{
    public class GlobalSettings
    {
        class JSONSettings
        {
            // number of auctions to show
            public int _N { get; set; }

            // default auction Duration
            public int _D { get; set; }

            // silver package
            public int _S { get; set; }

            // gold package
            public int _G { get; set; }

            // platinum package
            public int _P { get; set; }

            // currency
            public string _C { get; set; }

            // token value
            public double _T { get; set; }
        }

        static GlobalSettings()
        {
            JSONSettings savedSettings = JsonConvert.DeserializeObject<JSONSettings>(File.ReadAllText((HttpContext.Current.Server.MapPath("~/App_Data/settings.json"))));
            N = savedSettings._N;
            D = savedSettings._D;
            S = savedSettings._S;
            G = savedSettings._G;
            P = savedSettings._P;
            C = savedSettings._C;
            T = savedSettings._T;
            //// default values
            //N = 10;
            //D = 60;
            //S = 10;
            //G = 20;
            //P = 40;
            //C = "RSD";
            //T = 1;
        }

        public static void SaveToFile()
        {
            // serialize JSON to a string and then write string to a file
            File.WriteAllText(HttpContext.Current.Server.MapPath("~/App_Data/settings.json"), JsonConvert.SerializeObject(new JSONSettings {
                _N = N,
                _D = D,
                _S = S,
                _G = G, 
                _P = P,
                _C = C, 
                _T = T

        }));
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