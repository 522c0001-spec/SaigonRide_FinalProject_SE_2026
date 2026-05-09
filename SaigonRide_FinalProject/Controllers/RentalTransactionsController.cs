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
    public class RentalTransactionsController : Controller
    {
        private SaigonRideDBEntities db = new SaigonRideDBEntities();

        // GET: RentalTransactions
        public ActionResult Index()
        {
            var rentalTransactions = db.RentalTransactions.Include(r => r.Station).Include(r => r.Station1).Include(r => r.User).Include(r => r.Vehicle);
            return View(rentalTransactions.ToList());
        }

        // GET: RentalTransactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentalTransaction rentalTransaction = db.RentalTransactions.Find(id);
            if (rentalTransaction == null)
            {
                return HttpNotFound();
            }
            return View(rentalTransaction);
        }

        // GET: RentalTransactions/Create
        public ActionResult Create()
        {
            ViewBag.EndStationID = new SelectList(db.Stations, "StationID", "LocationName");
            ViewBag.StartStationID = new SelectList(db.Stations, "StationID", "LocationName");
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName");
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Category");
            return View();
        }

        // POST: RentalTransactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,UserID,VehicleID,StartStationID,EndStationID,StartTime,EndTime,BaseFare,AppliedDiscount,TotalPaid,PaymentMethod")] RentalTransaction rentalTransaction)
        {
            if (ModelState.IsValid)
            {
                db.RentalTransactions.Add(rentalTransaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EndStationID = new SelectList(db.Stations, "StationID", "LocationName", rentalTransaction.EndStationID);
            ViewBag.StartStationID = new SelectList(db.Stations, "StationID", "LocationName", rentalTransaction.StartStationID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", rentalTransaction.UserID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Category", rentalTransaction.VehicleID);
            return View(rentalTransaction);
        }

        // GET: RentalTransactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentalTransaction rentalTransaction = db.RentalTransactions.Find(id);
            if (rentalTransaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.EndStationID = new SelectList(db.Stations, "StationID", "LocationName", rentalTransaction.EndStationID);
            ViewBag.StartStationID = new SelectList(db.Stations, "StationID", "LocationName", rentalTransaction.StartStationID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", rentalTransaction.UserID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Category", rentalTransaction.VehicleID);
            return View(rentalTransaction);
        }

        // POST: RentalTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionID,UserID,VehicleID,StartStationID,EndStationID,StartTime,EndTime,BaseFare,AppliedDiscount,TotalPaid,PaymentMethod")] RentalTransaction rentalTransaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rentalTransaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EndStationID = new SelectList(db.Stations, "StationID", "LocationName", rentalTransaction.EndStationID);
            ViewBag.StartStationID = new SelectList(db.Stations, "StationID", "LocationName", rentalTransaction.StartStationID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "FullName", rentalTransaction.UserID);
            ViewBag.VehicleID = new SelectList(db.Vehicles, "VehicleID", "Category", rentalTransaction.VehicleID);
            return View(rentalTransaction);
        }

        // GET: RentalTransactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RentalTransaction rentalTransaction = db.RentalTransactions.Find(id);
            if (rentalTransaction == null)
            {
                return HttpNotFound();
            }
            return View(rentalTransaction);
        }

        // POST: RentalTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RentalTransaction rentalTransaction = db.RentalTransactions.Find(id);
            db.RentalTransactions.Remove(rentalTransaction);
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
        // GET: RentalTransactions/CheckoutForm/5
        public ActionResult CheckoutForm(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var ride = db.RentalTransactions.Find(id);
            if (ride == null) return HttpNotFound();

            // We need the stations list so the user can pick where they are dropping it off
            ViewBag.Stations = db.Stations.ToList();
            return View(ride);
        }
        public ActionResult StartRental(int? vehicleId)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");
            if (vehicleId == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var vehicle = db.Vehicles.Find(vehicleId);
            if (vehicle == null || vehicle.Status != "Available") return HttpNotFound();

            // Determine the base rate per minute based on FR1
            ViewBag.RatePerMin = vehicle.Category == "Standard Bike" ? 500 :
                                 vehicle.Category == "Electric Scooter" ? 1500 : 1000;

            ViewBag.Stations = db.Stations.ToList();
            return View(vehicle);
        }

        // 2. POST: RentalTransactions/StartRental
        // Creates the transaction and updates vehicle/station status
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult StartRental(int vehicleId, int startStationId, string paymentMethod)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            var vehicle = db.Vehicles.Find(vehicleId);
            var station = db.Stations.Find(startStationId);

            if (vehicle != null && station != null)
            {
                // Create the active ride
                RentalTransaction newRide = new RentalTransaction
                {
                    UserID = (int)Session["UserID"],
                    VehicleID = vehicleId,
                    StartStationID = startStationId,
                    StartTime = DateTime.Now,
                    PaymentMethod = paymentMethod
                    // EndTime, BaseFare, and TotalPaid remain NULL until they return the bike
                };

                // Update fleet logic
                vehicle.Status = "In-Transit";
                station.CurrentInventory -= 1;

                db.RentalTransactions.Add(newRide);
                db.SaveChanges();

                return RedirectToAction("MyRentals");
            }
            return View();
        }

        // 3. GET: RentalTransactions/MyRentals
        // Shows the guest their active and past rides
        public ActionResult MyRentals()
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            int currentUserId = (int)Session["UserID"];
            var userRides = db.RentalTransactions
                .Where(r => r.UserID == currentUserId)
                .Include(r => r.Vehicle)
                .Include(r => r.Station) // Start Station
                .Include(r => r.Station1) // End Station
                .OrderByDescending(r => r.StartTime)
                .ToList();

            return View(userRides);
        }

        // POST: RentalTransactions/Checkout
        // This is the climax of the application where all the FR1 & FR2 math happens
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(int transactionId, int endStationId, string paymentMethod)
        {
            if (Session["UserID"] == null) return RedirectToAction("Login", "Account");

            var ride = db.RentalTransactions.Find(transactionId);
            var endStation = db.Stations.Find(endStationId);
            var vehicle = db.Vehicles.Find(ride.VehicleID);

            if (ride != null && endStation != null && vehicle != null)
            {
                // 1. Stop the stopwatch
                ride.EndTime = DateTime.Now;

                // Calculate minutes (For testing purposes, if you end it instantly, we force 1 minute so the fare isn't 0)
                var durationMinutes = (decimal)(ride.EndTime.Value - ride.StartTime).TotalMinutes;
                if (durationMinutes < 1) durationMinutes = 1;

                // 2. FR1: Calculate Base Fare based on Vehicle Category
                decimal ratePerMinute = vehicle.Category == "Standard Bike" ? 500 :
                                        vehicle.Category == "Electric Scooter" ? 1500 : 1000;

                ride.BaseFare = Math.Round(durationMinutes * ratePerMinute);

                // 3. FR2: Check Destination Station Capacity for the 15% Discount
                double utilPercentage = endStation.MaxCapacity > 0 ?
                                        ((double)endStation.CurrentInventory / endStation.MaxCapacity) * 100 : 100;

                if (utilPercentage < 20)
                {
                    // Station is almost empty, give 15% off!
                    ride.AppliedDiscount = Math.Round(ride.BaseFare.Value * 0.15m);
                }
                else
                {
                    ride.AppliedDiscount = 0;
                }

                // Finalize Transaction
                ride.TotalPaid = ride.BaseFare - ride.AppliedDiscount;
                ride.PaymentMethod = paymentMethod;
                ride.EndStationID = endStationId;

                // 4. Update the Fleet and Station Inventory
                vehicle.Status = "Available";
                vehicle.CurrentStationID = endStationId;
                endStation.CurrentInventory += 1; // Add the bike to the new station

                db.SaveChanges();

                // Send them back to their dashboard to see the final receipt
                return RedirectToAction("MyRentals");
            }

            return HttpNotFound();
        }
    }
}
