using System;

namespace ScooterRental.Exceptions
{
    public class RentedScooterListIsEmptyException : Exception
    {
        public RentedScooterListIsEmptyException() : 
            base("No rented scooters.") { }
    }
}
