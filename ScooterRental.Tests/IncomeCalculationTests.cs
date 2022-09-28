using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScooterRental.Exceptions;
using ScooterRental.Interface;

namespace ScooterRental.Tests
{
    [TestClass]
    public class IncomeCalculationTests
    {
        private IIncomeCalculation _incomeCalculation;
        private List<DateTime> _starTimes;
        private List<DateTime> _endTimes;
        private List<RentedScooter> _history;

        [TestInitialize]
        public void Setup()
        {
            _starTimes = new List<DateTime>
            {
                new DateTime(2022, 09, 01, 23, 11, 00),
                new DateTime(2022, 09, 02, 23, 45, 00),
                new DateTime(2022, 09, 02, 16, 11, 00),
                new DateTime(2021, 09, 02, 16, 11, 00)
            };

            _endTimes = new List<DateTime>
            {
                new DateTime(2022, 09, 03, 00, 30, 00),
                new DateTime(2022, 09, 03, 00, 30, 00),
                new DateTime(2022, 09, 02, 16, 30, 00),
                new DateTime(2021, 09, 02, 16, 30, 00),
                new DateTime(2021, 09, 04, 16, 30, 00)
            };

            _history = new List<RentedScooter>
            {
                new RentedScooter("1", _starTimes[0], _endTimes[0], 1m),
                new RentedScooter("2", _starTimes[1], _endTimes[1], 1m),
                new RentedScooter("5", _starTimes[3], _endTimes[3], 1m),
                new RentedScooter("3", DateTime.UtcNow.AddMinutes(-10), 1m),
                new RentedScooter("4", _starTimes[2], _endTimes[2], 1m)
            };

            _incomeCalculation = new IncomeCalculation();
        }

        [TestMethod]
        public void GetTotalPriceForScooterWhenReturned()
        {
            // Act
            var result = _incomeCalculation.GetRentedScooterFee(_history[0]);
            var result2 = _incomeCalculation.GetRentedScooterFee(_history[1]);
            var result3 = _incomeCalculation.GetRentedScooterFee(_history[4]);

            // Assert
            result.Should().Be(60);
            result2.Should().Be(35);
            result3.Should().Be(19);
        }

        [TestMethod]
        public void GetTotalPriceForScooterWhenReturned_IfNotEndTimeSet_ThrowEndTimeNotSetException()
        {
            // Act
            Action act = () => _incomeCalculation.GetRentedScooterFee(_history[3]);

            // Assert
            act.Should().Throw<EndTimeNotSetException>()
                .WithMessage("Scooter EndTime not set.");
        }

        [TestMethod]
        public void GetAllIncome_IfRentIsFinished()
        {
            // Arrange
            AddTotalPriceToScooterInList(_history);

            // Act
            var result = _incomeCalculation.GetAllFinishedRentIncome(_history);

            // Assert
            result.Should().Be(133);
        }

        [TestMethod]
        public void GetAllIncome_RentedScooterListIsEmpty_ThrowRentedScooterListIsEmptyException()
        {
            // Arrange
            _history.Clear();

            // Act
            Action act = () => _incomeCalculation.GetAllFinishedRentIncome(_history);

            // Assert
            act.Should().Throw<RentedScooterListIsEmptyException>()
                .WithMessage("No rented scooters.");
        }
        

        [TestMethod]
        [DataRow(2021, 0)]
        [DataRow(2022, 11)]
        [DataRow(null, 11)]
        public void GetNotFinishedRentalFee(int? year, int income)
        {
            // Act
            var result = _incomeCalculation.GetNotFinishedRentalIncome(year, _history);

            // Assert
            result.Should().Be(income);
        }

        [TestMethod]
        [DataRow(2021)]
        [DataRow(2022)]
        [DataRow(null)]
        public void GetNotFinishedRentalFee_RentedScooterListIsEmpty_ThrowRentedScooterListIsEmptyException(int? year)
        {
            // Arrange
            _history.Clear();

            // Act
            Action act = () => _incomeCalculation.GetNotFinishedRentalIncome(year, _history);

            // Assert
            act.Should().Throw<RentedScooterListIsEmptyException>()
                .WithMessage("No rented scooters.");
        }

        [TestMethod]
        [DataRow(2021, 19)]
        [DataRow(2022, 114)]
        public void GetTotalRentalFeeByYear_FromAllReturnedScooters(int year, int income)
        {
            // Arrange
            AddTotalPriceToScooterInList(_history);

            // Act
            var result = _incomeCalculation.GetAllFinishedRentalIncomeByYear(year, _history);

            //Assert
            result.Should().Be(income);
        }

        [TestMethod]
        [DataRow(2021)]
        [DataRow(2022)]
        public void GetTotalFeeByYear_RentedScooterListIsEmpty_ThrowRentedScooterListIsEmptyException(int year)
        {
            // Arrange
            _history.Clear();

            // Act
            Action act = () => _incomeCalculation.GetAllFinishedRentalIncomeByYear(year,_history);

            // Assert
            act.Should().Throw<RentedScooterListIsEmptyException>()
                .WithMessage("No rented scooters.");
        }

        [TestMethod]
        public void GetTotalRentalFeeByYear2022_FromAllReturnedScooters_AndAllNotFinishedRentalTillNow()
        {
            // Arrange
            AddTotalPriceToScooterInList(_history);

            // Act
            var result = _incomeCalculation.GetAllFinishedRentalIncomeByYear(2022, _history) +
                         _incomeCalculation.GetNotFinishedRentalIncome(2022, _history);

            //Assert
            result.Should().Be(125);
        }

        [TestMethod]
        public void GetTotalRentalFee_FromAllReturnedScooters_AndAllNotFinishedRentalTillNow()
        {
            // Arrange
            AddTotalPriceToScooterInList(_history);

            // Act
            var result = _incomeCalculation.GetAllFinishedRentIncome(_history) +
                         _incomeCalculation.GetNotFinishedRentalIncome(null, _history);

            //Assert
            result.Should().Be(144);
        }

        private void AddTotalPriceToScooterInList(List<RentedScooter> list)
        {
            foreach (var scooter in list.Where(scooter => scooter.EndTime.HasValue))
            {
                scooter.TotalPrice = _incomeCalculation.GetRentedScooterFee(scooter);
            }
        }
    }
}
