using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
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

        [Route("debe"), HttpGet, SwaggerResponse(HttpStatusCode.OK, Type = typeof(IList<DebeTitleModel>))]
        public async Task<IHttpActionResult> GetDebeList()
        {
            IList<DebeTitleModel> titleModels = await _eksiFeedService.GetDebeList();

            return Ok(titleModels);
        }

        [Route("populer"), HttpGet, SwaggerResponse(HttpStatusCode.OK, Type = typeof(IList<PopulerTitleModel>))]
        public async Task<IHttpActionResult> GetPopulerList(int? page = null)
        {
            if (page.HasValue && page.Value <= 0)
            {
                return BadRequest();
            }

            IList<PopulerTitleModel> titleModels = await _eksiFeedService.GetPopulerList();

            return Ok(titleModels);
        }
    }
}