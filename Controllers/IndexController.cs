using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreElasticApp.Controllers
{
    [AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        [HttpPost]
        [Route("index")]
        public async Task<IActionResult> CreateIndexAsync()
        {
            return Ok(await new IndexEvery().CreateIndex());
        }

        [HttpPost]
        [Route("data")]
        public async Task<IActionResult> CreateDataAsync()
        {
            await new IndexEvery().BulkAsync();
            return Ok();
        }
    }
}
