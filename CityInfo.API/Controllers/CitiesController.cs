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
        public IActionResult GetCity(int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            if(city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }
    }
}
