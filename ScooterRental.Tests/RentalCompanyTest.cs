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
    public class RentalCompanyTest
    {
        private IRentalCompany _company;
        private IScooterService _service;
        private IList<Scooter> _scooters;
        private IList<RentedScooter> _rentedScooterList;
        private IList<DateTime> _starTimes;
        private IList<DateTime> _endTimes;
        private IIncomeCalculation _incomeCalculation;

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
                new DateTime(2021, 09, 02, 16, 30, 00)
            };

            _scooters = new List<Scooter>
            {
                new Scooter("1", 0.5m)
            };

            _rentedScooterList = new List<RentedScooter>
            {
                new RentedScooter("8", _starTimes[0], _endTimes[0], 1m),
                new RentedScooter("7", _starTimes[1], _endTimes[1], 1m),
                new RentedScooter("5", _starTimes[3], _endTimes[3], 1m),
                new RentedScooter("3", DateTime.UtcNow.AddMinutes(-10),  1m),
                new RentedScooter("4", _starTimes[2], _endTimes[2], 1m)
            };
            _service = new ScooterService(_scooters);
            _company = new RentalCompany("Scooters Rent", _service, _rentedScooterList);
            _incomeCalculation = new IncomeCalculation();
        }

        [TestMethod]
        public void GetCompanyName()
        {
            // Assert
            _company.Name.Should().Be("Scooters Rent");
        }

        [TestMethod]
        public void StartRent_AddRentedScooterToRentedScooterList()
        {
            // Act
            _company.StartRent("1");

            // Assert
           _rentedScooterList.Count.Should().Be(6);
        }

        [TestMethod]
        public void StartRent_IfScooterAlreadyRented_ThrowNewScooterAlreadyRentedException()
        {
            // Arrange
            var scooter = _service.GetScooterById("1");
            scooter.IsRented = true;

            // Act
            Action act = () =>_company.StartRent("1");

            // Assert
            act.Should().Throw<ScooterAlreadyRentedException>()
                .WithMessage("Scooter with id 1 already rented.");
        }

        [TestMethod]
        public void StartRent_IfScooterIdNullOrEmpty_ThrowInvalidIdException()
        {
            // Act
            Action act = () => _company.StartRent("");

            // Assert
            act.Should().Throw<InvalidIdException>()
                .WithMessage("Id cannot be empty or null");
        }

        [TestMethod]
        public void StartRent_IfScooterDoesNotExists_ThrowScooterDoesNotExistsException()
        {
            // Act
            Action act = () => _company.StartRent("4");

            // Assert
            act.Should().Throw<ScooterDoesNotExistException>()
                .WithMessage("Scooter with id 4 does not exists.");
        }

        [TestMethod]
        public void EndRent_EndRentingOfScooter()
        {
            // Arrange
            var scooter = new Scooter("2", 0.5m);
            _scooters.Add(scooter);
            scooter.IsRented = true;
            var rentedScooter = new RentedScooter("2", DateTime.UtcNow, 0.2m);
            _rentedScooterList.Add(rentedScooter);

            // Act
            _company.EndRent("2");
            
            // Assert
            scooter.IsRented.Should().BeFalse();
            rentedScooter.EndTime.HasValue.Should().BeTrue();
        }

        [TestMethod]
        public void EndRent_IfScooterDoesNotExistsInRentedScooterList_ThrowScooterIsNotRentedYetException()
        {
            // Act
            Action act = () => _company.EndRent("1");

            // Assert
            act.Should().Throw<ScooterIsNotRentedYetException>()
                .WithMessage("Scooter with id 1 have not rented yet.");
        }

        [TestMethod]
        public void EndRent_IfScooterIdNullOrEmpty_ThrowInvalidIdException()
        {
            // Act
            Action act = () => _company.EndRent("");

            // Assert
            act.Should().Throw<InvalidIdException>()
                .WithMessage("Id cannot be empty or null");
        }

        [TestMethod]
        [DataRow(-2025)]
        [DataRow(15)]
        [DataRow(1800)]
        public void CalculateIncome_InvalidYear_ThrowInvalidYearException_InPast(int year)
        {
            // Act
            Action act = () => _company.CalculateIncome(year, true);

            // Assert
            act.Should().Throw<InvalidYearException>()
                .WithMessage($"Year {year} is not valid.");
        }

        [TestMethod]
        public void CalculateIncome_InvalidYear_ThrowInvalidYearException_InFuture()
        {
            // Arrange
            var year = DateTime.UtcNow.Year + 1;

            // Act
            Action act = () => _company.CalculateIncome(year, true);

            // Assert
            act.Should().Throw<InvalidYearException>()
                .WithMessage($"Year {year} is not valid.");
        }

        [TestMethod]
        public void EndRent_GetTotalPriceWhenReturned()
        {
            // Arrange
            var rentedScooter = new RentedScooter("1", DateTime.UtcNow.AddMinutes(-10), 0.2m);
            _rentedScooterList.Add(rentedScooter);

            // Assert
            _company.EndRent("1").Should().Be(2.2m);
        }

        [TestMethod]
        [DataRow(2021, false, 19)]
        [DataRow(2021, true, 19)]
        [DataRow(2022, false, 114)]
        [DataRow(2022, true, 125)]
        [DataRow(null, false, 133)]
        [DataRow(null, true, 144)]
        public void CalculateIncome_ReturnIncomes(int? year, bool notFinishedInclude, int income)
        {
            // Arrange
            AddTotalPriceToScooterInList(_rentedScooterList);

            // Assert
            _company.CalculateIncome(year, notFinishedInclude).Should().Be(income);
        }

        private void AddTotalPriceToScooterInList(IList<RentedScooter> list)
        {
            foreach (var scooter in list.Where(scooter => scooter.EndTime.HasValue))
            {
                scooter.TotalPrice = _incomeCalculation.GetRentedScooterFee(scooter);
            }
        }
    }
}
