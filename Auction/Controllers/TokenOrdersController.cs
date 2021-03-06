﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AuctionsModel;

using Microsoft.AspNet.Identity;

namespace Auction.Controllers
{
    [Authorize]
    public class TokenOrdersController : Controller
    {
        private AuctionsModelDB db = new AuctionsModelDB();

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!User.IsInRole("Admin"))
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


        // GET: TokenOrders
        public ActionResult Index()
        {
            var tokenOrders = db.TokenOrders.Include(t => t.AspNetUser);
            return View(tokenOrders.ToList());
        }

        // GET: TokenOrders/Details/5
        //public ActionResult Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    if (tokenOrder == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(tokenOrder);
        //}

        // GET: TokenOrders/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");

            return View();
        }

        // POST: TokenOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "ID,UserID,TokenCount,Price,State")] TokenOrder tokenOrder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        tokenOrder.ID = Guid.NewGuid();
        //        db.TokenOrders.Add(tokenOrder);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", tokenOrder.UserID);
        //    return View(tokenOrder);
        //}



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string package)
        {
            double price = 0;
            int tokenCount = 0;
            if (package == "Gold")
            {
                tokenCount = Settings.GlobalSettings.G;
            } else if (package == "Silver")
            {
                tokenCount = Settings.GlobalSettings.S;
            } else
            {
                tokenCount = Settings.GlobalSettings.P;
            }

            price = tokenCount * Settings.GlobalSettings.T;
            TokenOrder tokenOrder = new TokenOrder();
            if (ModelState.IsValid)
            {
                tokenOrder.ID = Guid.NewGuid();
                tokenOrder.AspNetUser = db.AspNetUsers.Find(User.Identity.GetUserId());
                tokenOrder.Price = (decimal) price;
                tokenOrder.TokenCount = tokenCount;
                tokenOrder.State = "SUBMITTED";
                db.TokenOrders.Add(tokenOrder);
                db.SaveChanges();
                Logger.Logger.log("token order " + tokenOrder.ID + " created");
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", tokenOrder.UserID);
            
            return View(tokenOrder);
        }

        // GET: TokenOrders/Edit/5
        //public ActionResult Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    if (tokenOrder == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", tokenOrder.UserID);
            
        //    return View(tokenOrder);
        //}

        // POST: TokenOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "ID,UserID,TokenCount,Price,State")] TokenOrder tokenOrder)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(tokenOrder).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", tokenOrder.UserID);
            
        //    return View(tokenOrder);
        //}

        // GET: TokenOrders/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    if (tokenOrder == null)
        //    {
        //        return HttpNotFound();
        //    }
            
        //    return View(tokenOrder);
        //}

        // POST: TokenOrders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    TokenOrder tokenOrder = db.TokenOrders.Find(id);
        //    db.TokenOrders.Remove(tokenOrder);
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
