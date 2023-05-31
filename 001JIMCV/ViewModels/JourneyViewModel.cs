﻿using _001JIMCV.Models.Classes;
using _001JIMCV.Models.Classes.Enum;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace _001JIMCV.ViewModels
{
    public class JourneyViewModel
    {
        public Journey Journey { get; set; }
        public PackServices PackServices { get; set; }
        public Service Service { get; set; }
        public ServicePackServices ServicePackServices { get; set; }

        public Flight Flights { get; set; }
        public Accommodation  Accommodations { get; set; }
        public Activity Activities { get; set; }

        public List<string> FlightsCocheIds { get; set; }
        public List<string> AccommodationsCocheIds { get; set; }
        public List<string> ActivitiesCocheIds { get; set; }
             
    }


}

