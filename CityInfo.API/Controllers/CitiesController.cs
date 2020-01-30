using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CityInfo.API.Models;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [Route("/api/cities")]
    public class CitiesController : Controller
    {
        private ICityInfoRepository _cityInfoRepository;

        public CitiesController(ICityInfoRepository cityInfoRepository)
        {
            _cityInfoRepository = cityInfoRepository;
        }

        [HttpGet()]
        public IActionResult GetCities()
        {
            //return Ok(CitiesDataStore.Current.Cities);
            var result = Mapper.Map<IEnumerable<CityWithoutPointsOfInterest>> (_cityInfoRepository.GetCities());

            return Ok(result);

        }

        [HttpGet("{id}")]
        public IActionResult GetCity(int id, bool includePointsOfInterest = false){

            var city = _cityInfoRepository.GetCity(id, includePointsOfInterest);

            if (city == null)
                return NotFound();
            if(includePointsOfInterest){
                var result = Mapper.Map<CityDto>(city);
                return Ok(result);
            }

            var res = Mapper.Map<CityWithoutPointsOfInterest>(city);
            return Ok(res);

            //var res = CitiesDataStore.Current.Cities.FirstOrDefault(c => c.Id == id);
            //if (res == null)
            //    return NotFound();
            //return Ok(res);
        }
    }
}
