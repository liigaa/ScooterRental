using System;

namespace ScooterRental.Exceptions
{
    public class AvailableScooterListIsEmptyException : Exception
    {
        public AvailableScooterListIsEmptyException() : 
            base("No scooter available.") { }
    }
}
