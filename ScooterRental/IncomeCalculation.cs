using System;
using System.Collections.Generic;
using System.Linq;
using ScooterRental.Exceptions;
using ScooterRental.Interface;
using ScooterRental.Validations;

namespace ScooterRental
{
    public class IncomeCalculation : IIncomeCalculation
    {
        public decimal GetRentedScooterFee(RentedScooter scooter)
        {
            if (scooter.EndTime == null)
            {
                throw new EndTimeNotSetException();
            }

            var startTime = scooter.StarTime;
            var endTime = (DateTime)scooter.EndTime;
            var days = endTime.Day - startTime.Day + 1;
            decimal totalPrice;

            if (days < 2)
            {
                TimeSpan time = endTime - startTime;
                var totalMinutes = Math.Ceiling(time.TotalMinutes);
                totalPrice = (decimal) totalMinutes * scooter.PricePerMinute;

                return totalPrice <= 20 ? totalPrice : 20;
            }
            
            var firsDayMinutes = 1440 - startTime.TimeOfDay.TotalMinutes;
            var price = (decimal) firsDayMinutes * scooter.PricePerMinute;
            var firstDayPrice = price <= 20 ? price : 20;

            var lastDayMinutes = endTime.TimeOfDay.TotalMinutes;
            var lastDPrice = (decimal) lastDayMinutes * scooter.PricePerMinute;
            var lastDayPrice = lastDPrice <= 20 ? price : 20;

            totalPrice = 20 * (days - 2) + firstDayPrice + lastDayPrice;

            return totalPrice;
        }

        public decimal GetAllFinishedRentIncome(IList<RentedScooter> list)
        {
            Validator.RentedScooterListIsNotEmpty(list);
            var newList = list.Where(scooter => scooter.EndTime.HasValue).ToList();

            return newList.Sum(scooter => scooter.TotalPrice);
        }

        public decimal GetAllFinishedRentalIncomeByYear(int year, IList<RentedScooter> list)
        {
            Validator.RentedScooterListIsNotEmpty(list);
            var newList = list.Where(y => y.EndTime.HasValue && y.EndTime.Value.Year == year).ToList();

            return newList.Sum(scooter => scooter.TotalPrice);
        }

        public decimal GetNotFinishedRentalIncome(int? year, IList<RentedScooter> list)
        {
            Validator.RentedScooterListIsNotEmpty(list);

            if (year != DateTime.UtcNow.Year && year.HasValue) return 0;

            var newList = list.Where(scooter => !scooter.EndTime.HasValue);

            var totalIncome = 0m;

            foreach (var scooter in newList)
            {
                scooter.EndTime = DateTime.UtcNow;
                totalIncome += GetRentedScooterFee(scooter);
            }

            return totalIncome;
        }
    }
}
