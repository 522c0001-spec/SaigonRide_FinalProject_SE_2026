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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(int transactionId, int endStationId, string paymentMethod)
        {
            // 1. Find the active ride, the vehicle, and the destination station in the database
            var ride = db.RentalTransactions.Find(transactionId);
            var vehicle = db.Vehicles.Find(ride.VehicleID);
            var endStation = db.Stations.Find(endStationId);

            if (ride == null || endStation == null)
            {
                return HttpNotFound();
            }

            // 2. Stop the clock and calculate total minutes
            ride.EndTime = DateTime.Now;
            ride.EndStationID = endStationId;

            // We use Math.Ceiling to round up (e.g., 5.1 minutes becomes 6 minutes)
            double totalMinutes = Math.Ceiling((ride.EndTime.Value - ride.StartTime).TotalMinutes);

            // 3. Figure out the Base Fare based on the vehicle category
            decimal ratePerMinute = vehicle.Category == "E-Scooter" ? 1500m : 500m;
            ride.BaseFare = (decimal)totalMinutes * ratePerMinute;

            // 4. Check the Station Capacity for the 15% Discount
            // Formula: Current / Max
            double capacityPercentage = (double)endStation.CurrentInventory / endStation.MaxCapacity;

            if (capacityPercentage < 0.20) // Less than 20% full
            {
                ride.AppliedDiscount = ride.BaseFare * 0.15m; // 15% off
            }
            else
            {
                ride.AppliedDiscount = 0m; // No discount
            }

            // 5. Calculate Final Price
            ride.TotalPaid = ride.BaseFare - ride.AppliedDiscount;
            ride.PaymentMethod = paymentMethod;

            // 6. Update the physical Vehicle and Station statuses
            vehicle.Status = "Available";
            vehicle.CurrentStationID = endStationId;
            endStation.CurrentInventory += 1;

            // 7. Save everything to SQL Server
            db.SaveChanges();

            // Send the user to a "Receipt" page to see their final fare
            return RedirectToAction("Details", new { id = ride.TransactionID });
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
    }
}
