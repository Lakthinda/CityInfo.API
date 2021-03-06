﻿using AutoMapper;
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
            
            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointsOfInterest = _repository.GetPointsOfInterest(cityId);
                        
            if(pointsOfInterest.FirstOrDefault(p => p.Name.Contains(pointOfInterest.Name)) != null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest already exists." });
            }

            var finalPointOfInterest = Mapper.Map<Entities.PointOfInterest>(pointOfInterest);
            _repository.AddPointOfInterestForCity(cityId, finalPointOfInterest);

            if (!_repository.Save())
            {
                _logger.LogCritical($"Error when saving new point of Interest for CityID {cityId}, PointOfInterest {finalPointOfInterest.Id},{finalPointOfInterest.Name}");
                return StatusCode(500, "A problem happen when creating the point of interest. Please try again");
            }

            var createdPointOfInterest = Mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("getPointOfInterest",
                                  new { cityId = cityId, id = createdPointOfInterest.Id },
                                  createdPointOfInterest);
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

            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _repository.GetPointOfInterest(cityId, id);
            if (pointOfInterest == null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest does not exist." });
            }

            Mapper.Map(pointOfInterestForUpdate, pointOfInterest);

            if (!_repository.Save())
            {
                _logger.LogCritical($"Error when updating point of Interest for CityID {cityId}, PointOfInterest {id},{pointOfInterestForUpdate.Name}");
                return StatusCode(500, "A problem happen when updating the point of interest. Please try again");
            }
            
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

            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _repository.GetPointOfInterest(cityId, id);
            if (pointOfInterest == null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest does not exist." });
            }

            // var pointOfInterestToPatch = new PointOfInterestForUpdateDto()
            // {
            //     Name = pointOfInterest.Name,
            //     Description = pointOfInterest.Description
            // };

            var pointOfInterestToPatch = Mapper.Map<PointOfInterestForUpdateDto>(pointOfInterest);

            // Apply patch
            patchDoc.ApplyTo(pointOfInterestToPatch, ModelState);

            TryValidateModel(pointOfInterestToPatch);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Mapper.Map(pointOfInterestToPatch, pointOfInterest);

            if (!_repository.Save())
            {
                _logger.LogCritical($"Error when patch updating point of Interest for CityID {cityId}, PointOfInterest {id},{pointOfInterestToPatch.Name}");
                return StatusCode(500, "A problem happen when patch updating the point of interest. Please try again");
            }
            
            return NoContent();

            // pointOfInterest.Name = pointOfInterestToPatch.Name;
            // pointOfInterest.Description = pointOfInterestToPatch.Description;
            
            // return NoContent();
        }

        [HttpDelete("{cityId}/pointofinterest/{id}")]
        public IActionResult DeletePointOfInterest(int cityId, int id)
        {            
            if (!_repository.CityExists(cityId))
            {
                return NotFound();
            }

            var pointOfInterest = _repository.GetPointOfInterest(cityId,id);
            if (pointOfInterest == null)
            {
                return BadRequest(new { ErrorMessage = "This point of interest does not exist." });
            }

            _repository.RemovePointOfInterest(pointOfInterest);

            if (!_repository.Save())
            {
                _logger.LogCritical($"Error when deleting point of Interest for CityID {cityId}, PointOfInterest {id},{pointOfInterest.Name}");
                return StatusCode(500, "A problem happen when deleting the point of interest. Please try again");
            }

            _mailService.Send($"Point of Sale Deleted", $"Point of Sale Deleted in {cityId}");

            return NoContent();
        }
    }
}
