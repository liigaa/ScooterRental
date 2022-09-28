using System;

namespace ScooterRental.Exceptions
{
    public class EndTimeNotSetException : Exception
    {
        public EndTimeNotSetException() :
            base("Scooter EndTime not set.") { }
    }
}
