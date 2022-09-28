using System;

namespace ScooterRental.Exceptions
{
    public class ScooterIsNotRentedYetException : Exception
    {
        public ScooterIsNotRentedYetException(string id) :
            base($"Scooter with id {id} have not rented yet.")
        {

        }
    }
}
