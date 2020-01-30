using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityInfo.API.Controllers
{
    [Route("/api/cities")]
    public class PointsOfInterestController : Controller
    {
        ILogger<PointsOfInterestController> _logger;
        IMailService _mailService;
        ICityInfoRepository _cityInfoRepository;

        public PointsOfInterestController(ILogger<PointsOfInterestController> logger, IMailService mailService, ICityInfoRepository cityInfoRepository)
        {
            _logger = logger;
            _mailService = mailService;
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet("{cityId}/pointsofinterest")]
        public IActionResult GetPointsOfInterests(int cityId){
            try{
                if (!_cityInfoRepository.CityExists(cityId))
                    return NotFound();

                var pointsOfInterest = _cityInfoRepository.GetPointsOfInterest(cityId);
                var result = Mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterest);
                return Ok(result);

                //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
                //if (city == null)
                //{
                //    _logger.LogInformation($"City with {cityId} was not found");
                //    return NotFound();
                //}
                //return Ok(city);
            }
            catch(Exception ex){
                _logger.LogCritical("Critical exception happen", ex);
                return StatusCode(500, "Something went wrong, please try again later!!");   
            }
        }

        [HttpGet("{cityId}/pointsofinterest/{id}", Name = "GetPointOfInterest")]
        public IActionResult GetPointOfInterest(int cityId, int id){
            if (!_cityInfoRepository.CityExists(cityId))
            {
                _logger.LogInformation($"City with {cityId} was not found");
                return NotFound();
            }

            var pointOfInterest = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterest == null)
                return NotFound();

            var result = Mapper.Map<PointOfInterestDto>(pointOfInterest);
            return Ok(result);

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //    return NotFound();
            //var pointsOfInterest = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            //if (pointsOfInterest == null)
            //    return NotFound();
            //return Ok(pointsOfInterest);
        }

        [HttpPost("{cityId}/pointsofinterest")]
        public IActionResult CreatePointOfInterest(int cityId, [FromBody] PointsOfInterestForCreationDTO pointsOfInterest){
            if (pointsOfInterest == null)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (pointsOfInterest.Name == pointsOfInterest.Description)
            {
                ModelState.AddModelError("Desc", "Description should be different from name");
                return BadRequest(ModelState);
            }
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();

            var finalPointOfInterest = Mapper.Map<PointOfInterest>(pointsOfInterest);

            _cityInfoRepository.AddPointOfInterestForCity(cityId, finalPointOfInterest);
            if (!_cityInfoRepository.Save())
                return StatusCode(500, "Something went wrong!");

            var createdPointOfInterest = Mapper.Map<PointOfInterestDto>(finalPointOfInterest);

            return CreatedAtRoute("GetPointOfInterest", new { cityId, id = createdPointOfInterest.Id }, createdPointOfInterest);
        }

        [HttpPut("{cityid}/pointsofinterest/{id}")]
        public IActionResult UpdatePointOfInterest(int cityId, int id, [FromBody] PointsOfInterestForUpdateDto pointOfInterest){
            if (pointOfInterest == null)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (pointOfInterest.Name == pointOfInterest.Description)
            {
                ModelState.AddModelError("Desc", "Description should be different from name");
                return BadRequest(ModelState);
            }

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            Mapper.Map(pointOfInterest, pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
                return StatusCode(500, "Something went wrong!");

            return NoContent();

        }

        [HttpPatch("{cityid}/pointsofinterest/{id}")]
        public IActionResult PartialUpdatePointOfInterest(int cityId, int id, [FromBody] JsonPatchDocument<PointsOfInterestForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
                return BadRequest();

            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //    return NotFound();

            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterestFromStore == null)
                //return NotFound();

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            var pointOfInterestPatch = Mapper.Map<PointsOfInterestForUpdateDto>(pointOfInterestEntity);

            patchDoc.ApplyTo(pointOfInterestPatch, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (pointOfInterestPatch.Name == pointOfInterestPatch.Description)
            {
                ModelState.AddModelError("Desc", "Description should be different from name");
                return BadRequest(ModelState);
            }

            TryValidateModel(pointOfInterestPatch);

            Mapper.Map(pointOfInterestPatch, pointOfInterestEntity);

            if (!_cityInfoRepository.Save())
                return StatusCode(500, "Something went wrong!");
            return NoContent();

        }

        [HttpDelete("{cityid}/pointsofinterest/{id}")]
        public IActionResult DeletePointofInterest(int cityId, int id){
            //var city = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == cityId);
            //if (city == null)
            //    return NotFound();

            //var pointOfInterestFromStore = city.PointsOfInterest.FirstOrDefault(p => p.Id == id);
            //if (pointOfInterestFromStore == null)
                //return NotFound();

            if (!_cityInfoRepository.CityExists(cityId))
                return NotFound();
            var pointOfInterestEntity = _cityInfoRepository.GetPointOfInterest(cityId, id);
            if (pointOfInterestEntity == null)
                return NotFound();

            _cityInfoRepository.DeletePointOfInterest(pointOfInterestEntity);
            if (!_cityInfoRepository.Save())
                return StatusCode(500, "Something went wrong!");

            _mailService.Send("Point of interest deleted", $"Point of interest with name: {pointOfInterestEntity.Name } and { pointOfInterestEntity.Description} is deleted");

            return NoContent();

        }

    }
}
