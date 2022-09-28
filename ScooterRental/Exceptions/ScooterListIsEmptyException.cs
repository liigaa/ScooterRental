using System;

namespace ScooterRental.Exceptions
{
    public class ScooterListIsEmptyException : Exception
    {
        public ScooterListIsEmptyException() : 
            base("Company does not have scooters.") { }
    }
}
