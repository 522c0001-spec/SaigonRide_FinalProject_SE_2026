using SaigonRide_FinalProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SaigonRide_FinalProject.Controllers
{
    public class ReportsController : Controller
    {
        private SaigonRideDBEntities db = new SaigonRideDBEntities();

        public ActionResult Index()
        {
            // Dũng's FR5: Revenue by Category
            var revenueData = db.RentalTransactions
                .Where(r => r.TotalPaid != null)
                .GroupBy(r => r.Vehicle.Category)
                .ToDictionary(g => g.Key, g => g.Sum(r => r.TotalPaid.Value));

            ViewBag.RevenueData = revenueData;
            ViewBag.TotalRevenue = revenueData.Values.Sum();
            ViewBag.TotalRides = db.RentalTransactions.Count(r => r.TotalPaid != null);

            // Tú's FR5: Station Inventory Utilization
            ViewBag.StationData = db.Stations.ToList();

            return View();
        }
    }
}