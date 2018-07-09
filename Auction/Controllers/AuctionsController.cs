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
        }

        // GET: Auctions
        public ActionResult Index(int? minOffer, int? maxOffer, string name = "", string state = "OPENED", string error = "")
        {


            ViewBag.Error = error;
            

            var auctions = db.Auctions.ToList();


            List<ViewModels.AuctionBid> auctionsBids = new List<ViewModels.AuctionBid>();

            int shownAuctions = 0;

            if (state == "")
            {
                state = "OPENED";
            }

            foreach (var auction in auctions)
            {
                shownAuctions++;
                if (shownAuctions > Settings.GlobalSettings.N)
                {
                    break;
                }

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
                    auctionBid.UserName = latestBid.AspNetUser.UserName;
                }
                else
                {
                    auctionBid.price = (int)auction.StartingPrice;
                }

                auctionsBids.Add(auctionBid);
            }
            //return View(db.Auctions.ToList());
            return View(auctionsBids);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult AdminPanel()
        {
            return View(db.Auctions.ToList());
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
            if (auction.OppenedOn.AddSeconds(auction.Duration) >= DateTime.UtcNow || Roles.IsUserInRole("Admin"))
            {
                auction.State = "CLOSED";
                auction.ClosedOn = DateTime.UtcNow;
                db.SaveChanges(); 
            }
            return RedirectToAction("Index", db.Auctions.ToList());
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
