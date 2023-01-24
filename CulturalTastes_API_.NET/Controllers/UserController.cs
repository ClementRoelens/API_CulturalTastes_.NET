using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<User>> LoginAsync([FromBody] JObject body)
        {
            Console.WriteLine("UserController.Login");
            string username = body["username"].Value<string>();
            string password = body["password"].Value<string>();
            return await _userService.LoginAsync(username, password);
        }

        [HttpGet]
        [Route("getOneUser/{id}")]
        public async Task<ActionResult<User>> GetOneUser(string id)
        {
            Console.WriteLine($"UserController.GetOneUser lancé sur {id}");
            User user = await _userService.GetOneUserAsync(id);
            return (user is not null) ? Ok(user) : NotFound();
        }
    }
}
