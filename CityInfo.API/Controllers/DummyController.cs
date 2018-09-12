using CityInfo.API.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Controllers
{
    public class DummyController : Controller
    {
        private CityInfoContext _ctx;

        public DummyController(CityInfoContext context)
        {
            _ctx = context;
        }

        [HttpGet]
        [Route("api/testdatabase")]
        public IActionResult TestDatabse()
        {
            return Ok();
        }
    }
}