using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HotelBot.Models
{
    public enum BedSizeOptions
    {
        King,
        Queen,
        Single,
        Double
    }

    public enum AmenitiesOptions
    {
        Kitchen = 1,
        ExtraTowels = 2,
        GymAccess = 3,
        Wifi = 4
    }

    [Serializable]
    public class RoomReservation
    {
        public BedSizeOptions? BedSize;

        public int? NumberOfOccupants;

        public DateTime? CheckInDate;

        public int? NumberOfDaysToStay;

        public List<AmenitiesOptions> Amenities;

        public static IForm<RoomReservation> BuildForm()
        {
            return new FormBuilder<RoomReservation>()
                .Message("Welcome to hotel reservation bot!")
                .Build();
        }
    }
}