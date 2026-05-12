using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SaigonRide_FinalProject.Models;

namespace SaigonRide_FinalProject.Controllers
{
    public class StationsController : Controller
    {
        private SaigonRideDBEntities db = new SaigonRideDBEntities();

        // GET: Stations
        public ActionResult Index()
        {
            return View(db.Stations.ToList());
        }

        // GET: Stations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // GET: Stations/Create
        public ActionResult Create()
        {
            // SECURITY CHECK: Kick out anyone who isnt an admin
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // POST: Stations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StationID,LocationName,MaxCapacity,CurrentInventory")] Station station)
        {
            // SECURITY CHECK
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                db.Stations.Add(station);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(station);
        }

        // GET: Stations/Edit/5
        public ActionResult Edit(int? id)
        {
            // SECURITY CHECK
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // POST: Stations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StationID,LocationName,MaxCapacity,CurrentInventory")] Station station)
        {
            // SECURITY CHECK
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                db.Entry(station).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(station);
        }

        // GET: Stations/Delete/5
        public ActionResult Delete(int? id)
        {
            // SECURITY CHECK
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Station station = db.Stations.Find(id);
            if (station == null)
            {
                return HttpNotFound();
            }
            return View(station);
        }

        // POST: Stations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            // SECURITY CHECK
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            Station station = db.Stations.Find(id);
            db.Stations.Remove(station);
            db.SaveChanges();
            return RedirectToAction("Index");
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