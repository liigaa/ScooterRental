using ScooterRental.Exceptions;
using System.Collections.Generic;
using System.Linq;
using ScooterRental.Interface;

namespace ScooterRental.Validations
{
    public static class Validator
    {
        public static void ScooterIdValidation(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new InvalidIdException();
            }
        }

        public static void ScooterAlreadyRentedValidation(string id, IScooterService service)
        {
            var scooter = service.GetScooterById(id);

            if (scooter.IsRented)
            {
                throw new ScooterAlreadyRentedException(id);
            }
        }

        public static RentedScooter ScooterIsRentedValidation(string id, IList<RentedScooter> rentedScooters)
        {
            var rentedScooter = rentedScooters.FirstOrDefault(scooter => scooter.Id == id && !scooter.EndTime.HasValue);

            if (rentedScooter == null)
            {
                throw new ScooterIsNotRentedYetException(id);
            }

            return rentedScooter;
        }

        public static void ScooterPricePerMinuteValidation(decimal price)
        {
            if (price <= 0)
            {
                throw new InvalidPriceException(price);
            }
        }

        public static void ScooterDuplicateValidation(IList<Scooter> list, string id)
        {
            if (list.Any(scooter => scooter.Id == id))
            {
                throw new DuplicateScooterException(id);
            }
        }

        public static Scooter ScooterIsNotNull(IList<Scooter> list, string id)
        {
            var scooter = list.FirstOrDefault(scooter => scooter.Id == id);

            if (scooter == null)
            {
                throw new ScooterDoesNotExistException(id);
            }

            return scooter;
        }

        public static void ScooterListIsNotEmpty(IList<Scooter> list)
        {
            if (list.Count == 0)
            {
                throw new ScooterListIsEmptyException();
            }
        }

        public static void RentedScooterListIsNotEmpty(IList<RentedScooter> list)
        {
            if (list.Count == 0)
            {
                throw new RentedScooterListIsEmptyException();
            }
        }
    }
}
