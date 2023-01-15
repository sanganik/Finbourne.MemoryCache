using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Finbourne.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheInspectorController : ControllerBase
    {
        public CacheInspectorController()
        {

        }

        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult> Get(string key)
        {
            return Ok($"Inspected Key is {key}");
        }
    }
}
