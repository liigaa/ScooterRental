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
    public class ScooterServiceTest
    {
        private IScooterService _scooterService;
        private List<Scooter> _scooters;

        [TestInitialize]
        public void Setup()
        {
            _scooters = new List<Scooter>();
            _scooterService = new ScooterService(_scooters);
        }

        [TestMethod]
        public void AddScooter_ScooterAdded()
        {
            // Act
            _scooterService.AddScooter("1", 0.5m);

            // Assert
            _scooters.Count.Should().Be(1);
        }

        [TestMethod]
        public void AddScooter_AddScooterTwice_ThrowDuplicateScooterException()
        {
            // Acct
            _scooterService.AddScooter("1", 0.5m);

            Action act = () => 
                _scooterService.AddScooter("1", 0.5m);

            // Assert
            act.Should().Throw<DuplicateScooterException>()
                .WithMessage("Scooter with id 1 already exists.");
        }

        [TestMethod]
        public void AddScooter_AddScooterWithPriceZeroOrLess_ThrowInvalidPriceException()
        {
            // Act
            Action act = () =>
                _scooterService.AddScooter("1", -0.5m);

            // Assert
            act.Should().Throw<InvalidPriceException>()
                .WithMessage("Given price -0,5 is not valid.");
        }

        [TestMethod]
        public void AddScooter_AddScooterNullOrEmptyID_ThrowInvalidIdException()
        {
            // Act
            Action act = () =>
                _scooterService.AddScooter("", 0.5m);

            // Assert
            act.Should().Throw<InvalidIdException>()
                .WithMessage("Id cannot be empty or null");
        }

        [TestMethod]
        public void RemoveScooter_ScooterExists_ScooterIsRemoved()
        {
            // Arrange
            _scooters.Add(new Scooter("1", 0.5m));
            _scooters.Add(new Scooter("2", 0.5m));

            // Act
            _scooterService.RemoveScooter("1");

            // Assert
            _scooters.Any(scooter => scooter.Id == "1").Should().BeFalse();
            _scooters.Count.Should().Be(1);
        }

        [TestMethod]
        public void RemoveScooter_ScooterDoesNotExists_ScooterDoesNotExistsException()
        {
            // Arrange
            _scooters.Add(new Scooter("2", 0.5m));

            // Act
            Action act = () => _scooterService.RemoveScooter("1");

            // Assert
            act.Should().Throw<ScooterDoesNotExistException>()
                .WithMessage("Scooter with id 1 does not exists.");
        }

        [TestMethod]
        public void RemoveScooter_NullOrEmptyIdGiven_ThrowInvalidIdException()
        {
            // Act
            Action act = () =>
                _scooterService.RemoveScooter("");

            // Assert
            act.Should().Throw<InvalidIdException>()
                .WithMessage("Id cannot be empty or null");
        }

        [TestMethod]
        public void RemoveScooter_EmptyScooterList_ThrowScooterListIsEmptyException()
        {
            // Act
            Action act = () =>
                _scooterService.RemoveScooter("1");

            // Assert
            act.Should().Throw<ScooterListIsEmptyException>()
                .WithMessage("Company does not have scooters.");
        }

        [TestMethod]
        public void GetScooters_ReturnListWithAvailableScooter()
        {
            // Arrange
            _scooters.Add(new Scooter("1", 0.35m));
            _scooters.Last().IsRented = true;
            _scooters.Add(new Scooter("2", 0.45m));

            // Act
            var scooterList = _scooterService.GetScooters();
            scooterList.Add(new Scooter("4", 0.5m));
            var newScooterList = _scooterService.GetScooters();

           // Assert
            newScooterList.Count.Should().Be(1);
        }

        [TestMethod]
        public void GetScooters_NotAvailableScooter_ThrowAvailableScooterListIsEmptyException()
        {
            // Arrange
            _scooters.Add(new Scooter("1", 0.35m));
            _scooters.Last().IsRented = true;

            // Act
            Action act = () => _scooterService.GetScooters();

            // Assert
            act.Should().Throw<AvailableScooterListIsEmptyException>()
                .WithMessage("No scooter available.");
        }

        [TestMethod]
        public void GetScooters_EmptyScooterList_ThrowScooterListIsEmptyException()
        {
            // Act
            Action act = () =>
                _scooterService.GetScooters();

            // Assert
            act.Should().Throw<ScooterListIsEmptyException>()
                .WithMessage("Company does not have scooters.");
        }

        [TestMethod]
        public void GetScooterById_GetScooterFromListById()
        {
            // Arrange
            _scooters.Add(new Scooter("1", 0.5m));

            // Act
            var scooter = _scooters.FirstOrDefault(scooter => scooter.Id == "1");

            // Assert
            _scooterService.GetScooterById("1").Should().BeEquivalentTo(scooter);
        }

        [TestMethod]
        [DataRow("1")]
        [DataRow("5")]
        public void GetScooterById_NoScooterWithSuchId_ThrowScooterDoesNotExistException(string id)
        {
            // Arrange
            _scooters.Add(new Scooter("2", 0.5m));

            // Act
            Action act = () => _scooterService.GetScooterById(id);

            // Assert
            act.Should().Throw<ScooterDoesNotExistException>()
                .WithMessage($"Scooter with id {id} does not exists.");
        }

        [TestMethod]
        public void GetScooterById_NullOrEmptyIDGiven_ThrowInvalidIdException()
        {
            // Act
            Action act = () =>
                _scooterService.GetScooterById("");

            // Assert
            act.Should().Throw<InvalidIdException>()
                .WithMessage("Id cannot be empty or null");
        }

        [TestMethod]
        public void GetScooter_EmptyScooterList_ThrowScooterListIsEmptyException()
        {
            // Act
            Action act = () =>
                _scooterService.GetScooterById("1");

            // Assert
            act.Should().Throw<ScooterListIsEmptyException>()
                .WithMessage("Company does not have scooters.");
        }
    }
}
