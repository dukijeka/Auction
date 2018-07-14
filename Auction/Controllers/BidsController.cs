using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using AuctionsModel;

namespace Auction.Controllers
{   
    [Authorize]
    public class BidsController : Controller
    {
        private AuctionsModelDB db = new AuctionsModelDB();

        // GET: Bids
        //public ActionResult Index()
        //{
        //    var bids = db.Bids.Include(b => b.AspNetUser).Include(b => b.Auction);
        //    return View(bids.ToList());
        //}

        //// GET: Bids/Details/5
        //public ActionResult Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Bid bid = db.Bids.Find(id);
        //    if (bid == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bid);
        //}

        //// GET: Bids/Create
        //public ActionResult Create()
        //{
        //    ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
        //    ViewBag.AuctionID = new SelectList(db.Auctions, "ID", "Name");
        //    return View();
        //}

        // POST: Bids/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "UserID,AuctionID,TimeOfBidding,TokensOffered")] Bid bid)
        public async Task<ActionResult> Create(String userID, Guid auctionID, String tokensOffered)
        {
            // crate bid
            Logger.Logger.log("creating bid started");
            Bid bid = new Bid();
            bid.UserID = userID;
            using (var transaction = db.Database.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    bid.AspNetUser = db.AspNetUsers.Find(userID);
                    bid.AuctionID = auctionID;
                    bid.Auction = db.Auctions.Find(auctionID);
                    bid.TimeOfBidding = DateTime.UtcNow;
                    bid.TokensOffered = Int32.Parse(tokensOffered);

                    // check if the user has enough tokens
                    if (bid.AspNetUser.TokenBalance < Int32.Parse(tokensOffered))
                    {
                        Logger.Logger.log("bid failed, not enough tokens");
                        //ViewBag.Error = "You don't have anough Tokens!";
                        return RedirectToAction("Index", "Auctions", new { error = "You don't have anough Tokens!" });
                    }


                    // refund the last bidder(if exists)
                    Bid lastBid = db.Auctions.Find(auctionID).GetLatestBid();

                    // withdraw tokens
                    bid.AspNetUser.TokenBalance -= Int32.Parse(tokensOffered);

                    if (lastBid != null)
                    {
                        lastBid.AspNetUser.TokenBalance += lastBid.TokensOffered;

                        // check if the new bid is greater than the last
                        if (bid.TokensOffered <= lastBid.TokensOffered)
                        {
                            Logger.Logger.log("bidding failed: new bid must be greater than the previous");
                            ViewBag.Error = "A new bid must be greater than the last one!";
                            return RedirectToAction("Index", "Auctions", new { error = "A new bid must be greater than the last one!" });
                        }
                    }
                    else
                    {
                        // check if the new bid is greater than the initial price
                        if (bid.TokensOffered <= bid.Auction.StartingPrice)
                        {
                            Logger.Logger.log("Bidding failed: a new bid must be greater than the starting price(there are no previous bids)");
                            return RedirectToAction("Index", "Auctions", new { error = "A new bid must be greater than the minimal price!" });

                        }
                    }

                    if (ModelState.IsValid)
                    {
                        db.Bids.Add(bid);
                        await db.SaveChangesAsync();
                        transaction.Commit();
                        Logger.Logger.log("bid successfully created by " + bid.UserID + " for auction: " + bid.AuctionID );
                        // alert other users
                        Hubs.AuctionNotificaionsHub.UpdateClientAuctions(auctionID.ToString(), Int32.Parse(tokensOffered), bid.AspNetUser.UserName);
                        return RedirectToAction("Index", "Auctions");
                    }
                }
                catch (Exception)
                {
                    Logger.Logger.log("bid db transaction failed! Rolling back...");
                    transaction.Rollback();
                    return RedirectToAction("Index", "Auctions");
                }
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", bid.UserID);
            ViewBag.AuctionID = new SelectList(db.Auctions, "ID", "Name", bid.AuctionID);
            return View(bid);
        }

        // GET: Bids/Edit/5
        //public ActionResult Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Bid bid = db.Bids.Find(id);
        //    if (bid == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", bid.UserID);
        //    ViewBag.AuctionID = new SelectList(db.Auctions, "ID", "Name", bid.AuctionID);
        //    return View(bid);
        //}

        // POST: Bids/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "UserID,AuctionID,TimeOfBidding,TokensOffered")] Bid bid)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(bid).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", bid.UserID);
        //    ViewBag.AuctionID = new SelectList(db.Auctions, "ID", "Name", bid.AuctionID);
        //    return View(bid);
        //}

        // GET: Bids/Delete/5
        //public ActionResult Delete(string id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Bid bid = db.Bids.Find(id);
        //    if (bid == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(bid);
        //}

        // POST: Bids/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(string id)
        //{
        //    Bid bid = db.Bids.Find(id);
        //    db.Bids.Remove(bid);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
