using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;

namespace Auction.Hubs
{
    public class AuctionNotificaionsHub : Hub
    {
        public void Hello()
        {
            Clients.All.hello();
        }

        public static void UpdateClientAuctions(string auctionID, int newBidAmount, string userPosted)
        {
            UpdateData updateData = new UpdateData();
            updateData.AuctionID = auctionID;
            updateData.NewBidAmount = newBidAmount;
            updateData.UserPosted = userPosted;

            //Clients.All.updateAuction(updateData);

            var hubContext = GlobalHost.ConnectionManager.GetHubContext<AuctionNotificaionsHub>();
            hubContext.Clients.All.updateAuction(updateData);
        }
        
    }

    public class UpdateData
    {
        [JsonProperty("auctionID")]
        public string AuctionID { get; set; }

        [JsonProperty("newBidAmount")]
        public int NewBidAmount { get; set; }

        [JsonProperty("userPosted")]
        public string UserPosted { get; set; }
    }
}