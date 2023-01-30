using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        [Route("getOneOpinion/{id}")]
        public async Task<ActionResult<Opinion>> GetOneOpinionAsync(string id)
        {
            Console.WriteLine($"GetOneOpinion() sur {id}");
            return await _opinionService.GetOneOpinionAsync(id);
        }

        [Authorize]
        [HttpPut]
        [Route("modifyOpinion")]
        public async Task<ActionResult<Opinion>> ModifyOpinion([FromBody] JObject body)
        {
            Console.WriteLine("OpinionController.ModifyOpinion lancé");
            string id = body["id"].Value<string>();
            string content = body["content"].Value<string>();
            return await _opinionService.ModifyOpinion(id, content);
        }

    }
}
