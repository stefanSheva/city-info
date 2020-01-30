using System;
using System.Collections.Generic;
using System.Linq;
using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private CityInfoContext _ctx;
        public CityInfoRepository(CityInfoContext ctx)
        {
            _ctx = ctx;
        }

        public void AddPointOfInterestForCity(int cityId, PointOfInterest pointOfInterest)
        {
            var city = GetCity(cityId, false);
            city.PointsOfInterest.Add(pointOfInterest);
        }

        public bool CityExists(int cityId)
        {
            return _ctx.Cities.Any(c => c.Id == cityId);
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _ctx.PointsOfInterest.Remove(pointOfInterest);
        }

        public IEnumerable<City> GetCities()
        {
            return _ctx.Cities.OrderBy(c => c.Name).ToList();
        }

        public City GetCity(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return _ctx.Cities.Include(p => p.PointsOfInterest)
                           .Where(c => c.Id == cityId).FirstOrDefault();
            }
            return _ctx.Cities.Where(c => c.Id == cityId).FirstOrDefault();
        }

        public PointOfInterest GetPointOfInterest(int cityId, int id)
        {
            return _ctx.PointsOfInterest.Where(p => p.CityId == cityId && p.Id == id).FirstOrDefault();
        }

        public IEnumerable<PointOfInterest> GetPointsOfInterest(int cityId)
        {
            return _ctx.PointsOfInterest.Where(p => p.CityId == cityId).ToList();
        }

        public bool Save() => _ctx.SaveChanges() >= 0;
    }
}
