using AuctionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
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


            try
            {
                tokenOrder = db.TokenOrders.Find(Guid.Parse(clientid));
            }
            catch (Exception)
            {

                return "failed";
            }

            if (status != "success")
            {
                tokenOrder.State = "CANCELED";
                // even though the transaction has failed, we have successfuly found the transaction
                return "sucess!"; 
            }

             tokenOrder.State = "COMPLETED";

            // add Tokens to the user
            AspNetUser user = tokenOrder.AspNetUser;
            user.TokenBalance += tokenOrder.TokenCount;

            db.SaveChanges();

            sendEmail(tokenOrder.AspNetUser.Email);

            return "success!";

        }

        private void sendEmail(string email)
        {
            //SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 25);

            //smtpClient.Credentials = new System.Net.NetworkCredential("auctions20180708042230@gmail.com", "Auctions123#");
            //smtpClient.UseDefaultCredentials = true;
            //smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            //smtpClient.EnableSsl = true;
            //MailMessage mail = new MailMessage();

            ////Setting From , To and CC
            //mail.From = new MailAddress("auctions20180708042230@gmail.com", "Auctions IEP");
            //mail.Subject = "Payment Successful!";
            //mail.Body = "Thank you for your purchase!";
            //mail.To.Add(new MailAddress(email));
            ////mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));

            //smtpClient.Send(mail);

            MailMessage mm = new MailMessage("Sauctions20180708042230@gmail.com", email);
            {
                mm.Subject = "Payment Successful!";
                string body = "Thank you for your purchase!";
                mm.Body = body;
                mm.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.EnableSsl = true;
                NetworkCredential NetworkCred = new NetworkCredential("auctions20180708042230", "Auctions123#");   //to authorize the gmail user
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = 587;           //smtp gmail port no.
                smtp.Send(mm);
            }
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