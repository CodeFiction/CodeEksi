using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Services.Contracts;
using Services.Contracts.Models;
using Swashbuckle.Swagger.Annotations;

namespace Server.Controllers
{

    [RoutePrefix("v1/eksifeed")]
    public class EksiFeedController : ApiController
    {
        private readonly IEksiFeedService _eksiFeedService;

        public EksiFeedController(IEksiFeedService eksiFeedService)
        {
            _eksiFeedService = eksiFeedService;
        }

        [Route("debe"), HttpGet, SwaggerResponse(HttpStatusCode.OK, Type = typeof(IList<TitleModel>))]
        public async Task<IHttpActionResult> GetDebeList()
        {
            IList<TitleModel> titleModels = await _eksiFeedService.GetDebeList();

            return Ok(titleModels);
        }

        [Route("populer"), HttpGet, SwaggerResponse(HttpStatusCode.OK, Type = typeof(IList<TitleModel>))]
        public async Task<IHttpActionResult> GetPopulerList([FromUri]int page)
        {
            if (page <= 0)
            {
                return BadRequest();
            }

            IList<TitleModel> titleModels = await _eksiFeedService.GetPopulerList();

            return Ok(titleModels);
        }
    }
}