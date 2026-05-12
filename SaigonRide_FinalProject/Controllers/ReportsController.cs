using System;
using System.Linq;
using System.Web.Mvc;
using SaigonRide_FinalProject.Models;

namespace SaigonRide_FinalProject.Controllers
{
    public class ReportsController : Controller
    {
        // db check
        private SaigonRideDBEntities db = new SaigonRideDBEntities();

        // get reports
        public ActionResult Index(string filter = "AllTime")
        {
            // check if admin
            if (Session["UserType"] == null || Session["UserType"].ToString() != "Admin")
            {
                return RedirectToAction("Login", "Account");
            }

            // time filter stuff
            DateTime startDate = DateTime.MinValue;
            DateTime now = DateTime.Now;

            if (filter == "Day") startDate = now.Date;
            else if (filter == "Week") startDate = now.AddDays(-7);
            else if (filter == "Month") startDate = now.AddMonths(-1);

            // 1. safe database query for revenue grouping (doing math inside sql server instead of c#)
            var revList = db.RentalTransactions
                .Where(t => t.StartTime >= startDate && t.Vehicle != null && t.Vehicle.Category != null)
                .GroupBy(t => t.Vehicle.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(t => t.TotalPaid ?? 0) })
                .ToList();

            ViewBag.RevByCategory = revList.ToDictionary(x => x.Category, x => x.Total);

            // 2. get regular transaction stats
            var transactions = db.RentalTransactions
                                 .Where(t => t.StartTime >= startDate)
                                 .ToList();

            ViewBag.CurrentFilter = filter;
            ViewBag.TotalRevenue = transactions.Sum(t => t.TotalPaid ?? 0);
            ViewBag.TotalRides = transactions.Count;

            // 3. send all vehicles to view for the math stuff
            ViewBag.AllVehicles = db.Vehicles.ToList();

            var stations = db.Stations.ToList();
            return View(stations);
        }
    }
}