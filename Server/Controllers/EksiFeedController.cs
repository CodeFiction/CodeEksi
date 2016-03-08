using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Server.ActionFilters;
using Services.Contracts;
using Services.Contracts.Models;
using Swashbuckle.Swagger.Annotations;

namespace Server.Controllers
{
    [ExceptionHandlingFilter]
    [SwaggerResponse(HttpStatusCode.NotFound, Type = typeof (ErrorModel)),
     SwaggerResponse(HttpStatusCode.InternalServerError, Type = typeof (ErrorModel))]
    [RoutePrefix("v1/eksifeed")]
    public class EksiFeedController : ApiController
    {
        private readonly IEksiFeedService _eksiFeedService;

        public EksiFeedController(IEksiFeedService eksiFeedService)
        {
            _eksiFeedService = eksiFeedService;
        }

        [Route("debe"), HttpGet, SwaggerResponse(HttpStatusCode.OK, Type = typeof (IList<DebeTitleModel>))]
        public async Task<IHttpActionResult> GetDebeList()
        {
            IList<DebeTitleModel> titleModels = await _eksiFeedService.GetDebeList();

            return Ok(titleModels);
        }

        [Route("populer"), HttpGet, SwaggerResponse(HttpStatusCode.OK, Type = typeof (PopulerModel))]
        public async Task<IHttpActionResult> GetPopulerList(int? page = null)
        {
            if (page.HasValue && page.Value <= 0)
            {
                return BadRequest();
            }

            PopulerModel populerModel = await _eksiFeedService.GetPopulerList(page);

            return Ok(populerModel);
        }

        [Route("entries/{entryId}"), HttpGet, SwaggerResponse(HttpStatusCode.OK, Type = typeof (EntryDetailModel))]
        public async Task<IHttpActionResult> GetEntryDetail(string entryId)
        {
            var content = await _eksiFeedService.GetEntryById(entryId.Trim('#'));

            return Ok(content);
        }

        [Route("entries/search"), HttpGet, 
            SwaggerResponse(HttpStatusCode.OK, Type = typeof(IList<EntryDetailModel>)),
            SwaggerResponse(HttpStatusCode.NotFound, Type = typeof(IList<SuggestedTitle>))]
        public async Task<IHttpActionResult> Search(string titleText)
        {
            var content = await _eksiFeedService.Search(titleText);

            if (content.Result)
            {
                return Ok(content.EntryDetailModels);
            }

            return new NotFoundWithContent<IList<SuggestedTitle>> (Request, content.SuggestedTitles);
        }
    }
}