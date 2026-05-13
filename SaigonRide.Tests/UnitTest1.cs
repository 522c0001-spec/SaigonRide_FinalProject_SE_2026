using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SaigonRide.Tests
{
    [TestClass]
    public class PricingLogicTests
    {
        // This helper isolates the math from your controller so it's easy to test!
        private double CalculateDiscount(int currentInventory, int maxCapacity)
        {
            double capacityPercent = ((double)currentInventory / maxCapacity) * 100;
            if (capacityPercent < 20) return 0.15; // 15% discount
            return 0.00; // 0% discount
        }

        [TestMethod]
        public void TC01_TestEmptyStation_ReturnsDiscount()
        {
            double result = CalculateDiscount(0, 10); // 0% capacity
            Assert.AreEqual(0.15, result);
        }

        [TestMethod]
        public void TC02_TestJustBelowBoundary_ReturnsDiscount()
        {
            double result = CalculateDiscount(19, 100); // 19% capacity
            Assert.AreEqual(0.15, result);
        }

        [TestMethod]
        public void TC03_TestExactBoundary_ReturnsNoDiscount()
        {
            double result = CalculateDiscount(20, 100); // 20% capacity
            Assert.AreEqual(0.00, result);
        }

        [TestMethod]
        public void TC04_TestJustAboveBoundary_ReturnsNoDiscount()
        {
            double result = CalculateDiscount(21, 100); // 21% capacity
            Assert.AreEqual(0.00, result);
        }

        [TestMethod]
        public void TC05_TestFullStation_ReturnsNoDiscount()
        {
            double result = CalculateDiscount(80, 100); // 80% capacity
            Assert.AreEqual(0.00, result);
        }
    }
}