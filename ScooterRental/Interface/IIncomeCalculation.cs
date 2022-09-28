using System.Collections.Generic;

namespace ScooterRental.Interface
{
    public interface IIncomeCalculation
    {
        public decimal GetRentedScooterFee(RentedScooter scooter);
        public decimal GetAllFinishedRentIncome(IList<RentedScooter> list);
        public decimal GetAllFinishedRentalIncomeByYear(int year, IList<RentedScooter> list);
        public decimal GetNotFinishedRentalIncome(int? year, IList<RentedScooter> list);
    }
}
