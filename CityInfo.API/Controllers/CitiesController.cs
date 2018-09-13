using AutoMapper;
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
            var cities = _repository.GetCities();
            var result = Mapper.Map<IEnumerable<CityWithoutPointOfInterestDto>>(cities);

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
                var cityDTOResult = Mapper.Map<CityDto>(city);
                return Ok(cityDTOResult);
            }

            // default
            var cityWithoutPointOfInterestDTOResult = Mapper.Map<CityWithoutPointOfInterestDto>(city);

            return Ok(cityWithoutPointOfInterestDTOResult);
        }
    }
}
