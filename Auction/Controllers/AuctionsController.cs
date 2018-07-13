using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.UI.WebControls;
using AuctionsModel;
using ImageUtilities;
using Microsoft.AspNet.Identity;

namespace Auction.Controllers
{
    public class AuctionsController : Controller
    {
        private AuctionsModelDB db = new AuctionsModelDB();

        public AuctionsController()
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(!User.IsInRole("Admin"))
            {
                ViewBag.DisplayAdminPanel = "hidden";
            }
            else
            {
                ViewBag.DisplayAdminPanel = "visible";
            }

            if (User.Identity.IsAuthenticated)
            {
                ViewBag.User = db.AspNetUsers.Find(User.Identity.GetUserId());
            }
        }

        // GET: Auctions
        public ActionResult Index(int? minOffer, int? maxOffer, string name = "", string state = "OPENED", string error = "", int page = 1)
        {
            page--;
            if (page < 0)
            {
                page = 0;
            }

            ViewBag.Error = error;
            

            var auctions = db.Auctions.ToList();


            List<ViewModels.AuctionBid> auctionsBids = new List<ViewModels.AuctionBid>();

            int auctionsIndex = 0;

            if (state == "")
            {
                state = "OPENED";
            }

            foreach (var auction in auctions)
            {
                // if the auction has expiered, close it
                if (auction.OppenedOn.AddSeconds(auction.Duration) <= DateTime.UtcNow && auction.State == "OPENED")
                {
                    CloseAuction(auction.ID);
                }

               
                //// pagination
                //if (auctionsIndex < page * Settings.GlobalSettings.N)
                //{
                //    continue;
                //}

                //if (auctionsIndex >= Settings.GlobalSettings.N * (page + 1))
                //{
                //    break;
                //}

                Bid latestBid = auction.GetLatestBid();

                decimal latestOffer = latestBid == null ? auction.StartingPrice : latestBid.TokensOffered;

                if (minOffer != null && latestOffer < minOffer)
                {
                    continue;
                }

                if (maxOffer != null && latestOffer > maxOffer)
                {
                    continue;
                }

               
                if (name != "")
                {
                    bool somethingMatched = false;

                    foreach (string keyword in name.Split(' '))
                    {
                        if (auction.Name.ToLower().Contains(keyword.ToLower())) {
                            somethingMatched = true;
                            break;
                        }
                    }

                    if (!somethingMatched)
                    {
                        continue;
                    }
                }

                if (auction.State != state) {
                        continue;
                }

                ViewModels.AuctionBid auctionBid = new ViewModels.AuctionBid();
                auctionBid.AuctionID = auction.ID;
                auctionBid.AuctionName = auction.Name;
                auctionBid.Duration = auction.Duration;
                auctionBid.OppenedOn = auction.OppenedOn;
                auctionBid.State = auction.State;
                

                if (latestBid != null)
                {
                    auctionBid.price = latestBid.TokensOffered;
                    auctionBid.UserID = latestBid.UserID;
                    auctionBid.Name = latestBid.AspNetUser.Name;
                }
                else
                {
                    auctionBid.price = (int)auction.StartingPrice;
                }

                auctionsBids.Add(auctionBid);

                auctionsIndex++;
            }

            ViewBag.NumberOfPages = Math.Ceiling((decimal)auctionsBids.Count / Settings.GlobalSettings.N);
            //return View(db.Auctions.ToList());
            return View(auctionsBids.OrderBy(a => a.OppenedOn.AddSeconds(a.Duration)).ToList().Skip(page * Settings.GlobalSettings.N).Take(Settings.GlobalSettings.N));
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminPanel()
        {
            var auctions = db.Auctions.ToList();

            foreach (var auction in auctions)
            {
                // if the auction has expiered, close it
                if (auction.OppenedOn.AddSeconds(auction.Duration) <= DateTime.UtcNow && auction.State == "OPENED")
                {
                    CloseAuction(auction.ID);
                }
            }

            return View(auctions);
        }

        // GET: Auctions/Details/5
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionsModel.Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // GET: Auctions/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Auctions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,StartingPrice")] AuctionsModel.Auction auction, HttpPostedFileBase image, int? duration)
        {
            auction.ID = Guid.NewGuid();
            auction.CreatedOn = DateTime.UtcNow;
            auction.State = "READY";

            if (duration != null)
            {
                auction.Duration = (int) duration;
            } else
            {
                auction.Duration = Settings.GlobalSettings.D;
            }

            if (image != null && image.IsImage())
            {
                auction.Image = new byte[image.ContentLength];
                image.InputStream.Read(auction.Image, 0, image.ContentLength);
                //auction.Image = image.CreateThumbnail(150, 150);
            } 


            if (ModelState.IsValid)
            {
                db.Auctions.Add(auction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(auction);
        }

        // GET: Auctions/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionsModel.Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,Image,Duration,StartingPrice,CreatedOn,OppenedOn,ClosedOn,State")] AuctionsModel.Auction auction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(auction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(auction);
        }

        // GET: Auctions/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AuctionsModel.Auction auction = db.Auctions.Find(id);
            if (auction == null)
            {
                return HttpNotFound();
            }
            return View(auction);
        }

        // POST: Auctions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            AuctionsModel.Auction auction = db.Auctions.Find(id);
            db.Auctions.Remove(auction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult OpenAuction(Guid id)
        {
            AuctionsModel.Auction auction = db.Auctions.Find(id);
            auction.State = "OPENED";
            auction.OppenedOn = DateTime.UtcNow;
            db.SaveChanges();
            return RedirectToAction("Index", db.Auctions.ToList());
        }

        public ActionResult CloseAuction(Guid id)
        {

            AuctionsModel.Auction auction = db.Auctions.Find(id);

            // close only if the auction has expiered or if the user is admin
            if ((auction.OppenedOn.AddSeconds(auction.Duration) <= DateTime.UtcNow || (User != null && User.IsInRole("Admin")))
                && auction.State == "OPENED")
            {

                auction.State = "CLOSED";
                auction.ClosedOn = DateTime.UtcNow;
                db.SaveChanges();

                Hubs.AuctionNotificaionsHub.SendSignalToClientsToCloseAuction(id);
            }
            return RedirectToAction("Index", db.Auctions.ToList());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeEnvironmentSettings(int numberOfAuctions, int duration, int silver, int gold, int platinum, string currency, double tokenValue)
        {
            Settings.GlobalSettings.N = numberOfAuctions;
            Settings.GlobalSettings.D = duration;
            Settings.GlobalSettings.S = silver;
            Settings.GlobalSettings.G = gold;
            Settings.GlobalSettings.P = platinum;
            Settings.GlobalSettings.T = tokenValue;
            Settings.GlobalSettings.C = currency;

            Settings.GlobalSettings.SaveToFile();

            ViewBag.StatusMessage = "Success!";
            return RedirectToAction("AdminPanel");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


       


    }
}
