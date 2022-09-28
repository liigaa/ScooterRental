using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ScooterRental.Tests
{
    [TestClass]
    public class ScooterTests
    {
        private Scooter _scooter;
        
        [TestMethod]
        public void ScooterCreation_IDAndPricePerMinuteSetCorrectly()
        {
            // Act
            _scooter = new Scooter("1", 0.20m);

            // Assert
            _scooter.Id.Should().Be("1");
            _scooter.PricePerMinute.Should().Be(0.20m);
            _scooter.IsRented.Should().BeFalse();
        }
    }
}
