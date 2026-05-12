using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SaigonRide_FinalProject.Models;

namespace SaigonRide_FinalProject.Controllers
{
    public class ReportsController : Controller
    {
        // db connection
        private SaigonRideDBEntities db = new SaigonRideDBEntities();

        // get reports
        public ActionResult Index(string filter = "AllTime")
        {
            // check if admin
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // time filter logic
            DateTime startDate = DateTime.MinValue;
            DateTime now = DateTime.Now;

            if (filter == "Day") startDate = now.Date;
            else if (filter == "Week") startDate = now.AddDays(-7);
            else if (filter == "Month") startDate = now.AddMonths(-1);

            // 1. get the total numbers
            var transactions = db.RentalTransactions
                                 .Where(t => t.StartTime >= startDate)
                                 .ToList();

            ViewBag.CurrentFilter = filter;
            ViewBag.TotalRevenue = transactions.Sum(t => t.TotalPaid ?? 0);
            ViewBag.TotalRides = transactions.Count;

            // 2. the mystery fix: group directly in the database!
            // doing this without .ToList() forces sql server to do the join automatically
            var rawCategoryData = db.RentalTransactions
                .Where(t => t.StartTime >= startDate)
                .GroupBy(t => t.Vehicle.Category)
                .Select(g => new {
                    CatName = g.Key,
                    Total = g.Sum(t => t.TotalPaid ?? 0)
                })
                .ToList();

            // 3. safely map to dictionary and catch any missing categories
            var revByCategory = new Dictionary<string, double>();
            foreach (var item in rawCategoryData)
            {
                // if a transaction has a deleted vehicle, group it safely instead of crashing
                string safeName = string.IsNullOrEmpty(item.CatName) ? "Unknown/Deleted" : item.CatName;

                if (revByCategory.ContainsKey(safeName))
                {
                    revByCategory[safeName] += (double)item.Total;
                }
                else
                {
                    revByCategory[safeName] = (double)item.Total;
                }
            }

            ViewBag.RevByCategory = revByCategory;

            // 4. send all vehicles to view for the math stuff
            ViewBag.AllVehicles = db.Vehicles.ToList();

            var stations = db.Stations.ToList();
            return View(stations);
        }
    }
}