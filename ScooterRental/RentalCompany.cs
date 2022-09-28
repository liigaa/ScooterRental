using System;
using System.Collections.Generic;
using ScooterRental.Exceptions;
using ScooterRental.Interface;
using ScooterRental.Validations;

namespace ScooterRental
{
    public class RentalCompany : IRentalCompany
    {
        private readonly IIncomeCalculation _calculation = new IncomeCalculation();
        private readonly IScooterService _service;
        public string Name { get; }
        private readonly IList<RentedScooter> _rentedScooters;

        public RentalCompany(string name, IScooterService scooterService, IList<RentedScooter> rentedScooters)
        {
            Name = name;
            _service = scooterService;
            _rentedScooters = rentedScooters;
        }

        
        public void StartRent(string id)
        {
            Validator.ScooterIdValidation(id);
            var scooter = _service.GetScooterById(id);
            Validator.ScooterAlreadyRentedValidation(id, _service);
            scooter.IsRented = true;

            _rentedScooters.Add(new RentedScooter(scooter.Id, DateTime.UtcNow, scooter.PricePerMinute));

        }

        public decimal EndRent(string id)
        {
            Validator.ScooterIdValidation(id);
            var scooterFormList = _service.GetScooterById(id);
            var rentedScooter = Validator.ScooterIsRentedValidation(id, _rentedScooters);
            rentedScooter.EndTime = DateTime.UtcNow;
            scooterFormList.IsRented = false;
            var totalPrice = _calculation.GetRentedScooterFee(rentedScooter);
            rentedScooter.TotalPrice = totalPrice;
           
            return totalPrice;
        }

        public decimal CalculateIncome(int? year, bool includeNotCompletedRentals)
        {
            if (year < 1900 || year > DateTime.UtcNow.Year)
            {
                throw new InvalidYearException((int)year);
            }

            if (!year.HasValue && !includeNotCompletedRentals)
            {
                return _calculation.GetAllFinishedRentIncome(_rentedScooters);
            } 
            
            if (!year.HasValue)
            {
                return _calculation.GetAllFinishedRentIncome(_rentedScooters) +
                       _calculation.GetNotFinishedRentalIncome(null, _rentedScooters);
            }

            if (!includeNotCompletedRentals)
            {
                return _calculation.GetAllFinishedRentalIncomeByYear((int)year, _rentedScooters);
            }

            return _calculation.GetAllFinishedRentalIncomeByYear((int)year, _rentedScooters) +
                   _calculation.GetNotFinishedRentalIncome((int) year, _rentedScooters);
        }
    } 
}
