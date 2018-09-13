using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _repository;
        public CitiesController(ICityInfoRepository repository)
        {
            _repository = repository;
        }
        [HttpGet()]
        public IActionResult GetCities()
        {
            //var cities = CitiesDataStore.Current.Cities;            
            var cities = _repository.GetCities();
            var result = new List<CityWithoutPointOfInterestDto>();
            foreach(var city in cities)
            {
                result.Add(new CityWithoutPointOfInterestDto()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description
                });
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointOfInterest = false)
        {
            var city = _repository.GetCity(id, includePointOfInterest);

            if(city == null)
            {
                return NotFound();
            }

            if (includePointOfInterest)
            {
                var pointOfInterests = new List<PointOfInterestDto>();
                foreach(var p in city.PointsOfInterest)
                {
                    pointOfInterests.Add(new PointOfInterestDto()
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description
                    });
                }

                var cityDto = new CityDto()
                {
                    Id = city.Id,
                    Name = city.Name,
                    Description = city.Description,
                    NumberOfPointsOfInterest = city.PointsOfInterest.Count,
                    PointsOfInterest = pointOfInterests
                };

                return Ok(cityDto);
            }

            // default
            var cityWithoutPointOfIntrestDto = new CityWithoutPointOfInterestDto()
            {
                Id = city.Id,
                Name = city.Name,
                Description = city.Description
            };

            return Ok(cityWithoutPointOfIntrestDto);
        }
    }
}
