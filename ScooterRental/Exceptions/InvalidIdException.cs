using System;


namespace ScooterRental.Exceptions
{
    public class InvalidIdException : Exception
    {
        public InvalidIdException() : 
            base("Id cannot be empty or null") { }
    }
}
