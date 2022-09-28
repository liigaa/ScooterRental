using System.Collections.Generic;
using System.Linq;
using ScooterRental.Exceptions;
using ScooterRental.Interface;
using ScooterRental.Validations;

namespace ScooterRental
{
    public class ScooterService : IScooterService
    {
        private readonly IList<Scooter> _scooters;

        public ScooterService(IList<Scooter> list)
        {
            _scooters = list;
        }
        public void AddScooter(string id, decimal pricePerMinute)
        {
            Validator.ScooterIdValidation(id);
            Validator.ScooterPricePerMinuteValidation(pricePerMinute);
            Validator.ScooterDuplicateValidation(_scooters, id);

            _scooters.Add(new Scooter("1", 0.5m));
        }

        public Scooter GetScooterById(string scooterId)
        {
            Validator.ScooterIdValidation(scooterId);
            Validator.ScooterListIsNotEmpty(_scooters);
            var scooter = Validator.ScooterIsNotNull(_scooters, scooterId);

            return scooter;
        }

        public IList<Scooter> GetScooters()
        {
            Validator.ScooterListIsNotEmpty(_scooters);
            var availableScooters = _scooters.Where(scooter => scooter.IsRented == false).ToList();

            if (availableScooters.Count > 0)
            {
                return availableScooters;
            }

            throw new AvailableScooterListIsEmptyException();
        }

        public void RemoveScooter(string id)
        {
            Validator.ScooterIdValidation(id);
            Validator.ScooterListIsNotEmpty(_scooters);
            var scooter = Validator.ScooterIsNotNull(_scooters, id);

            _scooters.Remove(scooter);
        }
    }
}
