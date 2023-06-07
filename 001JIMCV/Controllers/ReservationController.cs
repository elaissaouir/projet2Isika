﻿using _001JIMCV.Models.Classes.Enum;
using _001JIMCV.Models.Classes;
using _001JIMCV.Models.Dals;
using _001JIMCV.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System;

namespace _001JIMCV.Controllers
{
    public class ReservationController : Controller
    {
        ReservationDal reservationDal;
        LoginDal loginDal;
        JourneyDal JourneyDal;
       
        public ReservationController()
        {
            reservationDal = new ReservationDal();
            loginDal = new LoginDal();
            JourneyDal = new JourneyDal();

        }
        public IActionResult Index(int id)
        {
            JourneyViewModel jvm=new JourneyViewModel();
            PackServices packServices = new PackServices();
            packServices = JourneyDal.GetPackServices(id);
            
            jvm = GetJourneyViewModelFull(packServices.JourneyId, false);
            return View("FormReservation", jvm);
        }
        public JourneyViewModel GetJourneyViewModelFull(int JourneyId, Boolean allInclusive)
        {
            JourneyViewModel jvm = new JourneyViewModel();
            jvm.journeyId = JourneyId;

            //On récupère le voyage associé à l'Id
            Journey journey = JourneyDal.GetAllJourneys().Where(r => r.Id == JourneyId).FirstOrDefault();

            if (journey != null)
            {
                jvm.Journey = journey;

                // On récupère le Pack de Service associé à l'id du voyage et qui est clé en main
                jvm.PackService = JourneyDal.GetAllPacks().Where(r => r.JourneyId == JourneyId & r.AllInclusive == allInclusive).FirstOrDefault();


                //On récupère les vols associé au pack de Service
                FlightPackServices flightPackServices = JourneyDal.GetFLightPackServices(jvm.PackService.Id);
                jvm.DepartureFlight = JourneyDal.GetFlight(flightPackServices.DepartureFlightId);
                jvm.ReturnFlight = JourneyDal.GetFlight(flightPackServices.ReturnFlightId);

                // On récupère les Hébergements associés au Pack
                List<AccommodationPackServices> accommodationsPackServices = JourneyDal.GetAllAccommodationsPackServices().Where(r => r.PackServicesId == jvm.PackService.Id).ToList();
                List<Accommodation> accommodations = new List<Accommodation>();
                foreach (AccommodationPackServices aps in accommodationsPackServices)
                {
                    Accommodation accommodation = new Accommodation();
                    accommodation = JourneyDal.GetAccommodation(aps.AccommodationId);
                    accommodations.Add(accommodation);
                }
                jvm.Accommodations = accommodations;

                // On récupère les Activités associés au Pack
                List<ActivityPackServices> activityPackServices = JourneyDal.GetAllActivityPackServices().Where(r => r.PackServicesId == jvm.PackService.Id).ToList();
                List<Activity> activities = new List<Activity>();
                foreach (ActivityPackServices aps in activityPackServices)
                {
                    Activity activity = new Activity();
                    activity = JourneyDal.GetActivity(aps.ActivityId);
                    activities.Add(activity);
                }
                jvm.Activities = activities;

                // On récupère les restaurants associés au Pack
                List<RestaurationPackServices> restaurationPackServices = JourneyDal.GetAllRestaurationPackServices().Where(r => r.PackServicesId == jvm.PackService.Id).ToList();
                List<Restauration> restaurations = new List<Restauration>();
                foreach (RestaurationPackServices aps in restaurationPackServices)
                {
                    Restauration restauration = new Restauration();
                    restauration = JourneyDal.GetRestauration(aps.RestaurationId);
                    restaurations.Add(restauration);
                }
                jvm.Restaurations = restaurations;
            }
            return jvm;
        }
    
        private decimal CalculateTotalAmount(int numberOfPassengers)
        {
            // Effectuez les calculs appropriés pour déterminer le montant en fonction du nombre de passagers
            decimal amount = numberOfPassengers;
            return amount;
        }

        public IActionResult GetReservationsList()
        {

            var reservations = reservationDal.GetAllReservations();
            ViewData["Reservations"] = reservations ?? new List<_001JIMCV.Models.Classes.Reservation>();
            return View("List");

        }
        public ActionResult GetReservations()

        {

            LoginViewModel viewModel = new LoginViewModel { Authentified = HttpContext.User.Identity.IsAuthenticated };
            if (viewModel.Authentified)
            {
                viewModel.User = loginDal.GetUser(HttpContext.User.Identity.Name);
                UserEnum role = viewModel.User.Role;
                switch (role)
                {
                    case UserEnum.Admin:
                        return RedirectToAction("GetReservationsList");
                    case UserEnum.Customer:

                        var reservations = reservationDal.GetAllReservations()
                            .Where(p => p.ClientId == viewModel.User.Id).ToList();
                        ViewData["Reservations"] = reservations ?? new List<_001JIMCV.Models.Classes.Reservation>();

                        return View("ConfirmationPayement");
                    default:

                        return View("Error");
                }
            }

            return View();

        }
        public ActionResult GetClientReservation()
        {
            LoginViewModel viewModel = new LoginViewModel { Authentified = HttpContext.User.Identity.IsAuthenticated };
            if (viewModel.Authentified)
            {
                viewModel.User = loginDal.GetUser(HttpContext.User.Identity.Name);
                UserEnum role = viewModel.User.Role;

                if (role == UserEnum.Customer)
                {
                    var reservations = reservationDal.GetAllReservations()
                        .Where(p => p.ClientId == viewModel.User.Id)
                        .ToList();

                    ViewData["Reservations"] = reservations ?? new List<_001JIMCV.Models.Classes.Reservation>();
                }
            }

            return View("List");
        }


        [HttpPost]
        public IActionResult AddReservation(Reservation reservation)
        {
            LoginViewModel viewModel = new LoginViewModel { Authentified = HttpContext.User.Identity.IsAuthenticated };
            viewModel.User = loginDal.GetUser(HttpContext.User.Identity.Name);
            UserEnum role = viewModel.User.Role;

            reservationDal.AddReservation(viewModel.User.Id, reservation.TravelStartDate, reservation.TravelEndDate, reservation.ContactName, reservation.ContactEmail, reservation.ContactPhone,
                reservation.NumberOfPassengers, reservation.TotalAmount);

            return RedirectToAction("GetReservations");
        }

        [HttpGet]
        public IActionResult EditReservation(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            Reservation reservation = reservationDal.GetReservationById(id);

            if (reservation == null)
            {
                return View("Error");
            }

            return View("EditReservationForm", reservation);
        }


        [HttpPost]
        public IActionResult EditReservation(Reservation reservation)
        {
            if (!ModelState.IsValid)
                return View(reservation);

            if (reservation.Id != 0)
            {
                reservationDal.EditReservation(reservation.Id, reservation.TravelStartDate, reservation.TravelEndDate, reservation.ContactName, reservation.ContactEmail, reservation.ContactPhone,
                    reservation.NumberOfPassengers, reservation.TotalAmount, reservation.Status);

                return RedirectToAction("GetReservations", new { id = reservation.Id });
            }
            else
            {
                return View("Error");
            }
        }
    }
}