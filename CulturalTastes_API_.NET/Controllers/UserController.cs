using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Nodes;

namespace UserStoreApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {

        private readonly UserService _userService;

        public UserController(UserService usersService) =>
            _userService = usersService;

        [HttpPost]
        [Route("signin")]
        public async Task<ActionResult<User>> SigninAsync([FromBody] JObject body)
        {
            string username = body["username"].Value<string>();
            string password = body["password"].Value<string>();
            Console.WriteLine($"UserController.Singin de {username}");
            bool validUsername = (username != "");
            bool validPassword = (password != "");
            var modelState = new ModelStateDictionary();
            if (!validUsername)
            {
                ModelState.AddModelError("Pseudo","Votre pseudo ne peut pas être vide");
            }
            if (!validPassword)
            {
                ModelState.AddModelError("Mot de passe", "Votre mot de passe ne peut pas être vide");
            }
            return (validUsername && validPassword) ? Ok(await _userService.LoginAsync(username, password)) : BadRequest(modelState);
        }
        
        [HttpPost]
        [Route("signup")]
        public async Task<ActionResult> SignUpAsync([FromBody] JObject body)
        {
            Console.WriteLine("UserController.Signup");
            string username = body["username"].Value<string>();
            string password = body["password"].Value<string>();
            bool validUsername = (username != "");
            bool validPassword = (password != "");
            var modelState = new ModelStateDictionary();
            if (!validUsername)
            {
                ModelState.AddModelError("Pseudo", "Votre pseudo ne peut pas être vide");
            }
            if (!validPassword)
            {
                ModelState.AddModelError("Mot de passe", "Votre mot de passe ne peut pas être vide");
            }
            if (validUsername && validPassword)
            {
                User user = await _userService.Signup(username, password);
                return Created("", user);
            }
            else
            {
                return BadRequest(modelState);
            }
        }

        //public async Task<ActionResult<User>> GetCreatedUserAsync(string id) =>
        //    await _userService.GetOneUserAsync(id);
        

        [Authorize]
        [HttpGet]
        [Route("getOneUser/{id}")]
        public async Task<ActionResult<User>> GetOneUserAsync(string id)
        {
            Console.WriteLine($"UserController.GetOneUser lancé sur {id}");
            if (id != "")
            {
                var userId = User.FindFirst("userId")?.Value;
                if (userId == id)
                {
                    User user = await _userService.GetOneUserAsync(id);
                    return (user != null) ? Ok(user) : NotFound();
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return BadRequest("L'id renseigné est vide");
            }
        }
    }
}
