using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json.Linq;
using System;

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

        private void LikeOrDislikeResult(string itemId, ref List<string> actionList, ref List<string> reverseList, ref int actionOperation, ref int reverseOperation)
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
        public async Task<ActionResult> LikeOrDislikeItem([FromBody] JObject body)
        {
            Console.WriteLine("Entrée dans SharedController.LikeOrDislike");

            string action = body["action"].Value<string>();
            string itemType = body["itemType"].Value<string>();
            string userId = body["userId"].Value<string>();
            string itemId = body["itemId"].Value<string>();

            bool validAction = (action == "like" || action == "dislike");
            bool validItemType = (itemType == "film" || itemType == "album" || "itemType" == "game");
            bool validUserId = (userId != "");
            bool validItemId = (itemId != "");
            var modelState = new ModelStateDictionary();
            if (!validAction)
            {
                modelState.AddModelError("action", "action doit être 'like' ou 'dislike'");
            }
            if (!validItemType)
            {
                modelState.AddModelError("itemType", "itemType doit être 'film','album' ou 'game");
            }
            if (!validUserId)
            {
                modelState.AddModelError("userId", "userId ne doit pas être vide");
            }
            if (!validItemId)
            {
                modelState.AddModelError("itemId", "itemId ne doit pas être vide");
            }
            if (validAction && validItemType && validUserId && validItemId)
            {
                User user = await _userService.GetOneUserAsync(userId);
                Film film = await _filmService.GetOneFilmAsync(itemId);
                if (user != null && film != null)
                {
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
                    await _userService.LikeOrDislikeItemAsync(userId, newLikedList, newDislikedList);
                    await _filmService.UpdateFilmLikesAsync(itemId, likesOperation, dislikesOperation);

                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest(modelState);
            }
            
        }

        [Authorize]
        [HttpPost]
        [Route("addOneOpinion")]
        public async Task<ActionResult> AddOpinion([FromBody] JObject body)
        {
            Console.WriteLine("SharedController.AddOpinion()");

            string itemId = body["itemId"].Value<string>();
            string userId = body["userId"].Value<string>();
            string username = body["username"].Value<string>();
            string content = body["content"].Value<string>();
            string itemType = body["itemType"].Value<string>();

          
            bool validContent = (content != "");
            bool validItemType = (itemType == "film" || itemType == "album" || "itemType" == "game");
            bool validUserId = (userId != "");
            bool validItemId = (itemId != "");
            User user = await _userService.GetOneUserAsync(userId);
            var modelState = new ModelStateDictionary();
            bool validUsername = (username != "" && username == user.username);
            if (!validUsername)
            {
                modelState.AddModelError("username", "username ne doit pas être vide et correspondre au userId renseigné");
            }
            if (!validItemType)
            {
                modelState.AddModelError("itemType", "itemType doit être 'film','album' ou 'game'");
            }
            if (!validUserId)
            {
                modelState.AddModelError("userId", "userId ne doit pas être vide");
            }
            if (!validItemId)
            {
                modelState.AddModelError("itemId", "itemId ne doit pas être vide");
            }
            if (!validContent)
            {
                modelState.AddModelError("content", "content ne doit pas être nul");
            }
            if (validUsername && validItemType && validItemId && validUserId && validUsername)
            {
                Opinion opinion = await _opinionService.CreateOpinionAsync(new Opinion(content, itemType, userId, username));
                if (opinion != null)
                {
                    Console.WriteLine("Avis créé");
                    // Tester l'itemType quand les autres types seront implémentés
                    Film film = await _filmService.CreatedOpinionAsync(itemId, opinion._id.ToString());
                    if (film != null)
                    {
                        Console.WriteLine("Film mis à jour");
                        User updatedUser = await _userService.CreatedOpinionAsync(userId, opinion._id.ToString());

                        if (updatedUser != null)
                        {
                            Console.WriteLine("User mis à jour");
                            return new OkObjectResult(opinion);
                        }
                    }
                }
                return BadRequest();
            }
           
            return BadRequest(modelState);
        }

        [Authorize]
        [HttpPut]
        [Route("likeOpinion")]
        public async Task<ActionResult<Opinion>> LikeOpinion([FromBody] JObject body)
        {
            string opinionId = body["opinionId"].Value<string>();
            string userId = body["userId"].Value<string>();

            Console.WriteLine($"SharedController.LikeOpinion() sur l'user {userId} et l'opinion {opinionId}");

            bool validOpinionId = (opinionId != "");
            bool validUserId = (userId != "");
            var modelState = new ModelStateDictionary();
            if (!validOpinionId)
            {
                modelState.AddModelError("opinionId", "opinionId ne doit pas être vide");
            }
            if (!validUserId)
            {
                modelState.AddModelError("userId", "userId ne doit pas être vide");
            }
            if (validOpinionId && validUserId)
            {
                User user = await _userService.GetOneUserAsync(userId);
                if (user != null)
                {
                    List<string> opinionsId = user.likedOpinionsId;
                    int index = opinionsId.IndexOf(opinionId);
                    int operation;
                    if (index == -1)
                    {
                        Console.WriteLine("L'avis n'était pas encore liké");
                        operation = 1;
                        opinionsId.Add(opinionId);
                    }
                    else
                    {
                        Console.WriteLine("L'avis était déjà liké");
                        operation = -1;
                        opinionsId.Remove(opinionId);
                    }
                    Opinion opinion = await _opinionService.LikeOrDislikeOpinionAsync(opinionId, operation);
                    if (opinion != null)
                    {
                        User updatedUser = await _userService.LikeOrDislikeOpinionAsync(userId, opinionsId, operation);
                        if (updatedUser != null)
                        {
                            return Ok(opinion);
                        }
                        else
                        {
                            return new BadRequestObjectResult("Erreur lors de la mise à jour de l'user");
                        }
                    }
                    return new BadRequestObjectResult("Erreur lors de la mise à jour de l'avis");
                }
                return NotFound();
            }
            return BadRequest(modelState);
           
        }

        [Authorize]
        [HttpDelete]
        [Route("removeOpinion/{opinionId}/{userId}/{itemId}")]
        public async Task<ActionResult> RemoveOpinion(string opinionId, string userId, string itemId)
        {
            Console.WriteLine($"SharedController.EraseOpinion({opinionId},{userId},{itemId}");

            bool validOpinionId = (opinionId != "");
            bool validUserId = (userId != "");
            bool validItemId = (itemId != "");
            var modelState = new ModelStateDictionary();
            if (!validOpinionId)
            {
                modelState.AddModelError("opinionId", "opinionId ne doit pas être vide");
            }
            if (!validUserId)
            {
                modelState.AddModelError("userId", "userId ne doit pas être vide");
            }
            if (!validItemId)
            {
                modelState.AddModelError("itemId", "itemId ne doit pas être vide");
            }
            if (validOpinionId && validUserId && validItemId)
            {
                User user = await _userService.GetOneUserAsync(userId);
                if (user != null)
                {
                    List<string> newOpinionsId = user.opinionsId;
                    newOpinionsId.Remove(opinionId);
                    User updatedUser = await _userService.RemoveOpinionAsync(newOpinionsId, userId);
                    if (updatedUser != null)
                    {
                        Console.WriteLine("User mis à jour");
                        Film film = await _filmService.GetOneFilmAsync(itemId);
                        List<string> newFilmOpinionsId = film.opinionsId;
                        // Vérifier ici
                        newFilmOpinionsId.Remove(opinionId);
                        Film updatedFilm = await _filmService.RemoveOpinionAsync(itemId, newFilmOpinionsId);
                        if (updatedFilm != null)
                        {
                            Console.WriteLine("Film mis à jour");
                            await _opinionService.RemoveOpinionAsync(opinionId);
                            Console.WriteLine("Avis supprimé");
                            return Ok();
                        }
                        else
                        {
                            return new BadRequestObjectResult("Erreur lors de la mise à jour du film (user correctement mis à jour");
                        }
                    }
                    else
                    {
                        return new BadRequestObjectResult("Erreur lors de la mise à jour de l'user");
                    }
                }
                return NotFound();
               
            }
            return BadRequest(modelState);
           
        }

    }
}
