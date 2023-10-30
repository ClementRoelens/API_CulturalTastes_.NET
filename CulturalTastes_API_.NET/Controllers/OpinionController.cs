using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;

namespace CulturalTastes_API_.NET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OpinionController : ControllerBase
    {
        private readonly OpinionService _opinionService;

        public OpinionController(OpinionService opinionService)
        {
            _opinionService = opinionService;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Opinion>> GetOneOpinionAsync(string id)
        {
            Console.WriteLine($"GetOneOpinion() sur {id}");
            bool validId = (id != "");
            if (validId)
            {
                Opinion opinion = await _opinionService.GetOneOpinionAsync(id);
                return (opinion != null) ? Ok(opinion) : NotFound();
            }
           else
            {
                return BadRequest("id ne doit pas être vide");
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<ActionResult<Opinion>> ModifyOpinion([FromBody] JObject body)
        {
            Console.WriteLine("OpinionController.ModifyOpinion lancé");

            string id = body["id"].Value<string>();
            string content = body["content"].Value<string>();

            bool validId = (id != "");
            bool validContent = (content != "");
            var modelState = new ModelStateDictionary();
            if (!validId)
            {
                modelState.AddModelError("id", "id ne doit pas être nul");
            }
            if (!validContent)
            {
                modelState.AddModelError("content", "content ne doit pas être nul");
            }
            if (validId && validContent)
            {
                Opinion opinion = await _opinionService.ModifyOpinion(id, content);
                return (opinion != null) ? Ok(opinion) : BadRequest();
            }
            return BadRequest(modelState);
        }

    }
}
