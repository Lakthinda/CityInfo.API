using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    [Route("api/cities/")]
    public class PointOfInterestController: Controller
    {
        private ILogger<PointOfInterestController> _logger;
        private IMailService _mailService;
        private ICityInfoRepository _repository;
        public PointOfInterestController(ILogger<PointOfInterestController> logger,
                                         IMailService mailService,
                                         ICityInfoRepository repository)
        {
            _logger = logger;
            _mailService = mailService;
            _repository = repository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterest(int cityId)
        {
            
            try
            {                
                var city = _repository.GetCity(cityId, true);

                if (city == null)
                {
                    _logger.LogInformation($"City with id {cityId} has not been found.");
                    return NotFound();
                }

                var pointsOfInterest = Mapper.Map<IEnumerable<PointOfInterestDto>>(city.PointsOfInterest);

                return Ok(pointsOfInterest);
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Exception while getting PointsOfInterest with cityID {cityId}");
                return StatusCode(500, $"A Problem happen while processing the request. Please try again.");
            }
        }

        [HttpGet("{cityId}/pointofinterest/{id}", Name = "getPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId,int id)
        {
            var pointOfInterest = _repository.GetPointOfInterest(cityId, id);

            if (pointOfInterest == null)
            {
                return NotFound();
            }

            var result = Mapper.Map<PointOfInterestDto>(pointOfInterest);

            return Ok(result);
        }

        [HttpPost("{cityId}/pointofinterest")]
        public IActionResult CreatePointOfInterest(int cityId,
                                                    [FromBody] PointOfInterestForCreationDto pointOfInterest)
        {
            if(pointOfInterest == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            if(city.PointsOfInterest.FirstOrDefault(p => p.Name.Contains(pointOfInterest.Name)) != null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest already exists." });
            }

            // TO DO - Improve later
            var maxPointOfInterestId = city.PointsOfInterest.Max(p => p.Id);

            var finalPointOfInterest = new PointOfInterestDto()
            {
                Id = ++maxPointOfInterestId,
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            city.PointsOfInterest.Add(finalPointOfInterest);

            return CreatedAtRoute("getPointOfInterest",
                                  new { cityId = cityId, id = finalPointOfInterest },
                                  finalPointOfInterest);
        }

        [HttpPut("{cityId}/pointofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId,int id,
                                [FromBody] PointOfInterestForUpdateDto pointOfInterestForUpdate)
        {
            if (pointOfInterestForUpdate == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterest == null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest does not exist." });
            }

            pointOfInterest.Name = pointOfInterestForUpdate.Name;
            pointOfInterest.Description = pointOfInterestForUpdate.Description;

            return NoContent();
        }

        [HttpPatch("{cityId}/pointofinterest/{id}")]
        public IActionResult PartiallyUpdatePointOfInterest(int cityId, int id,
                                [FromBody] JsonPatchDocument<PointOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterest == null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest does not exist." });
            }

            var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            {
                Name = pointOfInterest.Name,
                Description = pointOfInterest.Description
            };

            // Apply patch
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            pointOfInterest.Name = pointOfInterestToPatch.Name;
            pointOfInterest.Description = pointOfInterestToPatch.Description;
            
            return NoContent();
        }

        [HttpDelete("{cityId}/pointofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {
            var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);

            if (city == null)
            {
                return NotFound();
            }

            var pointOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            if (pointOfInterest == null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest does not exist." });
            }

            city.PointsOfInterest.Remove(pointOfInterest);

            _mailService.Send($"Point of Sale Deleted", $"Point of Sale Deleted in {cityId}");

            return NoContent();
        }
    }
}
