using CityInfo.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API
{
    public static class CityInfoExtension
    {
        public static void EnsureSeedDataForContext(this CityInfoContext context)
        {
            if (context.Cities.Any())
            {
                return;
            }

            // Init seed data
            #region SeedData
            var cities = new List<City>()
            {
                new City()
                {
                    Name="New York City",
                    Description="The one that big park",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name="Central Park",
                            Description = "The most visited urban park in the United States."
                        },
                        new PointOfInterest()
                        {
                            Name="Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhattan."
                        }
                    }
                },
                new City()
                {
                    Name="Antwrep",
                    Description="The one that big park",PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name="Central Park",
                            Description = "The most visited urban park in the United States." },

                        new PointOfInterest()
                        {
                            Name="Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhattan." },
                    }
                },

                new City()
                {
                    Name="Paris",
                    Description="The one that big park",
                    PointsOfInterest = new List<PointOfInterest>()
                    {
                        new PointOfInterest()
                        {
                            Name="Central Park",
                            Description = "The most visited urban park in the United States." },

                        new PointOfInterest()
                        {
                            Name="Empire State Building",
                            Description = "A 102-story skyscraper located in Midtown Manhattan." },
                    }
                }
            };
            #endregion

            context.Cities.AddRange(cities);
            context.SaveChanges();
        }
    }
}
