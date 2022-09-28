using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ScooterRental.Interface;

namespace ScooterRental.Tests
{
    [TestClass]
    public class RentedScooterTests
    {
        private RentedScooter _rentedScooter;
        private readonly DateTime _startDate = new(2022, 08, 31, 19, 14, 21);

        [TestMethod]
        public void RentedScooterCreation_IDAndPricePerMinuteSetCorrectly()
        {
            // Act
            _rentedScooter = new RentedScooter("1", _startDate, 0.25m);

            // Assert
            _rentedScooter.Id.Should().Be("1");
            _rentedScooter.PricePerMinute.Should().Be(0.25m);
            _rentedScooter.StarTime.Should().Be(_startDate);
            _rentedScooter.TotalPrice.Should().Be(0);
        }
    }
}
