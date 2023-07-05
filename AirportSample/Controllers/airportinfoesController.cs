using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using AirportSample.Models;

namespace AirportSample.Controllers
{
    public class airportinfoesController : Controller
    {
        public AirportDBEntities db = new AirportDBEntities();

        // GET: airportinfoes
        public ActionResult Index()
        {
            
            return View();
        }

     
        public ActionResult Create()
        {
            var cityList = db.cityinfoes.ToList();
            ViewBag.CityList1 = new SelectList(cityList, "CITY", "CITY");


            ViewBag.CityList2 = new SelectList(cityList, "CITY", "CITY");
            return View();
        }

       
        [HttpPost]
        public  ActionResult Create(FormCollection form)
        {
           
                var cityList = db.cityinfoes.ToList();

                string From = form["CityList1"].ToString();
                cityinfo city1 = cityList.Find(m => m.CITY == From);
                var startlocation = new Location(city1.LAT,city1.LONG);
                string To = form["CityList2"].ToString();
                cityinfo city2 = cityList.Find(m => m.CITY == To);

                var destinationlocation = new Location(city2.LAT,city2.LONG);
            if (From == To)
            {
                TempData["Error"] = "Source and destination cannot be same";
                return RedirectToAction("Create");
            }
            else
            {
                var airportsInRange = new List<airportinfo>(); /// airports between cities

                var airports = db.airportinfoes.ToList();


                var maxDistance = HaversineDistance(startlocation, destinationlocation)  ;
                foreach (var airport in airports)
                {

                    var airportLocation = new Location(airport.LAT, airport.LONG);
                    var distance = CalculateDistance(startlocation, destinationlocation, airportLocation);

                    if (distance <= maxDistance)
                        airportsInRange.Add(airport);
                }
                return View("AirportDisplay", airportsInRange);
            }
                 
        }

        public ActionResult StateWiseAirports()
        {
            var airports = db.airportinfoes.ToList();
           List<String> list = airports.Select(m => m.STATE).Distinct().ToList();
            List<State> slist = new List<State>();
            foreach (var item in list)
            {
                slist.Add(new State { Sname = item });
            }


            return View(slist);
        }

        public ActionResult AirportList(string id)
        {
            var airports = db.airportinfoes.ToList();
            List<airportinfo> list = airports.FindAll(m => m.STATE == id);
            return View(list);

        }

        public double CalculateDistance(Location startLocation, Location destinationLocation, Location airportLocation)
        {
            var startToAirportDistance = HaversineDistance(startLocation, airportLocation);
            var airportToDestinationDistance = HaversineDistance(airportLocation, destinationLocation);
            var totalDistance = startToAirportDistance + airportToDestinationDistance;

            return totalDistance;
        }



        public double HaversineDistance(Location location1, Location location2)
        {
            var earthRadius = 6371; // Radius of the Earth in kilometers
            var latitudeDifference = DegreesToRadians(location2.Latitude - location1.Latitude);
            var longitudeDifference = DegreesToRadians(location2.Longitude - location1.Longitude);

            var a = Math.Sin(latitudeDifference / 2) * Math.Sin(latitudeDifference / 2) +
            Math.Cos(DegreesToRadians(location1.Latitude)) * Math.Cos(DegreesToRadians(location2.Latitude)) *
            Math.Sin(longitudeDifference / 2) * Math.Sin(longitudeDifference / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = earthRadius * c;

            return distance;
        }

        public double DegreesToRadians(double degrees)
        {

            return degrees * (Math.PI / 180);
        }
       

    }

}































   