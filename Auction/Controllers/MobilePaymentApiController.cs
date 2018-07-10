using AuctionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Auction.Controllers
{
    public class MobilePaymentApiController : ApiController
    {

        private AuctionsModelDB db = new AuctionsModelDB();

        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        public string Get(int id)
        {
            return "value";
        }

        public string Get(string clientid, string status)
        {
            TokenOrder tokenOrder;

            tokenOrder = db.TokenOrders.Find(Guid.Parse(clientid));
            tokenOrder.State = "clientID";
            db.SaveChanges();

            return clientid;

        }
        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
}