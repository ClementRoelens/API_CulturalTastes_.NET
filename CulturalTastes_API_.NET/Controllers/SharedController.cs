using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace CulturalTastes_API_.NET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SharedController : ControllerBase
    {
        private readonly FilmService _filmService;
        private readonly UserService _userService;
        private readonly OpinionService _opinionService;

        public SharedController(FilmService filmService, UserService userService, OpinionService opinionService)
        {
            _filmService = filmService;
            _userService = userService;
            _opinionService = opinionService;
        }

        private void LikeOrDislikeResult(string itemId , ref List<string> actionList, ref List<string> reverseList, ref int actionOperation, ref int reverseOperation) 
        {
            if (actionList.IndexOf(itemId) == -1)
            {
                actionList.Add(itemId);
                actionOperation = 1; 
                if (reverseList.IndexOf(itemId) != -1)
                {
                    reverseList.Remove(itemId);
                    reverseOperation = -1;
                }
            }
            else
            {
                actionList.Remove(itemId);
                actionOperation = -1;
            }
        }

        [Authorize]
        [HttpPut]
        [Route("likeOrDislikeItem")]
        public async Task<ActionResult> LikeOrDislike([FromBody] JObject body)
        {
            Console.WriteLine("Entrée dans SharedController.LikeOrDislike");
            string action = body["action"].Value<string>();
            string itemType = body["itemType"].Value<string>();
            string userId = body["userId"].Value<string>();
            string itemId = body["itemId"].Value<string>();
            User user = await _userService.GetOneUserAsync(userId);
            List<string> newLikedList = user.likedFilmsId;
            List<string> newDislikedList = user.dislikedFilmsId;
            // Quand les autres types d'oeuvres seront intégrées...
            //if (itemType == "film")
            //{
            //    newLikedList = user.likedFilmsId;
            //    newDislikedList = user.dislikedFilmsId;
            //}
            int likesOperation = 0;
            int dislikesOperation = 0;
            // Si l'id du film est présent dans la liste de l'utilisateur, on le supprime et on désincrémente le nombre de likes
            if (action == "like")
            {
                LikeOrDislikeResult(itemId, ref newLikedList, ref newDislikedList, ref likesOperation, ref dislikesOperation);
            }
            else if (action == "dislike")
            {
                LikeOrDislikeResult(itemId, ref newDislikedList, ref newLikedList, ref dislikesOperation, ref likesOperation);
            }

            // Faire des tests quand on aura les autres types d'oeuvres
            await _userService.UpdateUserLikesAndDislikesAsync(userId, newLikedList, newDislikedList);
            await _filmService.UpdateFilmLikesAsync(itemId, likesOperation, dislikesOperation);

            return Ok();
        }

        [HttpPost]
        [Route("addOneOpinion")]
        public async Task<ActionResult<CompleteResult>> AddOpinion([FromBody] JObject body)
        {
            Console.WriteLine("SharedController.AddOpinion()");
            string itemId = body["itemId"].Value<string>();
            string userId = body["userId"].Value<string>();
            string content = body["content"].Value<string>();
            string itemType = body["itemType"].Value<string>();

            Opinion opinion = await _opinionService.CreateOpinionAsync(new Opinion(content, userId));
            if (opinion is not null)
            {
                Film film = await _filmService.UpdateFilmOpinionAsync(itemId, opinion._id.ToString());
                if (film is not null)
                {
                    User user = await _userService.UpdateUserCreatedOpinionAsync(userId, opinion._id.ToString());

                    if (user is not null)
                    {
                        return new CreatedResult(new Uri($"opinion/{opinion._id.ToString()}"), new CompleteResult(film, user, opinion));
                    }
                }
             
            }

            return BadRequest();
        }
    }
}
