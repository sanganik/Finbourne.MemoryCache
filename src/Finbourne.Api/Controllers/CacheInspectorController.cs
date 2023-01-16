using System;
using System.Threading.Tasks;
using Finbourne.Api.Domain;
using Finbourne.Cache.Component;
using Microsoft.AspNetCore.Mvc;


// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Finbourne.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CacheInspectorController : ControllerBase
    {
        private readonly IFinbourneCacheService _finbourneCacheService;

        public CacheInspectorController(IFinbourneCacheService finbourneCacheService)
        {
            _finbourneCacheService = finbourneCacheService;
        }

        [HttpGet]
        [Route("Get")]
        public async Task<ActionResult> Get(string key)
        {
            try
            {
                var cacheItem = _finbourneCacheService.Get<CustomObject>(key);
                return Ok(cacheItem);
            }
            catch(Exception)
            {
                // log exception 
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("Post")]
        public async Task<ActionResult> Post(CustomObject model)
        {
            try
            {
                var cacheItem = _finbourneCacheService.Set(model.Key, model);
                return Created(new Uri(Request.Path), model.Key);
            }
            catch (Exception)
            {
                // log exception 
                return BadRequest();
            }
        }

    }
}