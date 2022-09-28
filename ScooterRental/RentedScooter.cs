using System;

namespace ScooterRental
{
    public class RentedScooter
    {
        public string Id { get; set; }
        public DateTime StarTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal PricePerMinute { get; set; }
        public decimal TotalPrice { get; set; }

        public RentedScooter(string id, DateTime starTime, decimal pricePerMinute)
        {
            Id = id;
            StarTime = starTime;
            PricePerMinute = pricePerMinute;
            TotalPrice = 0;
        }
        public RentedScooter(string id, DateTime starTime, DateTime endTime, decimal pricePerMinute)
        {
            Id = id;
            StarTime = starTime;
            EndTime = endTime;
            PricePerMinute = pricePerMinute;
            TotalPrice = 0;
        }
    }
}
