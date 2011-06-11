using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Opuno.Brenn.Models;

namespace Opuno.Brenn.Website.Controllers
{
    using System.Diagnostics.Contracts;

    using Opuno.Brenn.DataAccess;

    public class CollectionController : Controller
    {
        private BrennContext db = new BrennContext();

        //
        // GET: /Collection/

        public ViewResult Index()
        {
            return View(db.Trips.ToList());
        }

        //
        // GET: /Collection/Details/5

        public ViewResult Details(int id)
        {
            Contract.Requires<ArgumentException>(id > 0);

            var collection = db.Trips.Find(id);

            return View(collection);
        }

        //
        // GET: /Collection/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Collection/Create

        [HttpPost]
        public ActionResult Create(Trip collection)
        {
            if (ModelState.IsValid)
            {
                db.Trips.Add(collection);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return View(collection);
        }
        
        //
        // GET: /Collection/Edit/5
 
        public ActionResult Edit(int id)
        {
            Trip collection = db.Trips.Find(id);
            return View(collection);
        }

        //
        // POST: /Collection/Edit/5

        [HttpPost]
        public ActionResult Edit(Trip collection)
        {
            if (ModelState.IsValid)
            {
                db.Entry(collection).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(collection);
        }

        //
        // GET: /Collection/Delete/5
 
        public ActionResult Delete(int id)
        {
            Trip collection = db.Trips.Find(id);
            return View(collection);
        }

        //
        // POST: /Collection/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {            
            Trip collection = db.Trips.Find(id);
            db.Trips.Remove(collection);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}