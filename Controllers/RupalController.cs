using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HelloCode.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RupalController : ControllerBase
    {
        private readonly ILogger<RupalController> _logger;

        public RupalController(ILogger<RupalController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public String Get()
        {
            return("Hello from Rupal");
        }
    }
}