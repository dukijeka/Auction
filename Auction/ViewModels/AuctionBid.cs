using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Auction.ViewModels
{
    public class AuctionBid
    {
        public Guid AuctionID { get; set; }

        public string AuctionName { get; set; }

        public int Duration { get; set; }

        public int price { get; set; }

        public DateTime OppenedOn { get; set; }

        public string State { get; set; }

        public string UserID { get; set; }

        public string Name { get; set; }


    }
}