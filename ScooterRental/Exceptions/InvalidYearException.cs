using System;

namespace ScooterRental.Exceptions
{
    public class InvalidYearException : Exception
    {
        public InvalidYearException(int year) : base($"Year {year} is not valid.") { }
    }
}
