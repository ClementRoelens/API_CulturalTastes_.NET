using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Mvc;

namespace CulturalTastes_API_.NET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlbumController : ControllerBase
    {
        private readonly AlbumService _albumService;

        public AlbumController(AlbumService albumService) =>
            _albumService = albumService;

        [HttpGet]
        [Route("genres")]
        public ActionResult<string[]> GetGenres()
        {
            Console.WriteLine("Entrée dans AlbumController.GetGenres");
            return Ok(_albumService.GetGenres());
        }

        //Méthode uniquement utilisé pour le dev
        [HttpGet("images/{url}")]
        public ActionResult GetImage(string url)
        {
            Console.WriteLine("AlbumController.GetImage sur " + url);


            var path = Path.Combine(Directory.GetCurrentDirectory(), "images", url);

            return PhysicalFile(path, "image/jpeg");
        }

        #region GetALL

        [HttpGet]
        public async Task<ActionResult<List<Album>>> GetAll()
        {
            Console.WriteLine("AlbumController.Get lancé");
            List<Album> albums = await _albumService.GetAlbumsAsync(false);
            albums.Sort((x, y) => string.Compare(x.title, y.title));

            return (albums != null) ? Ok(albums) : BadRequest();
        }

        [HttpGet]
        [Route("author/{author}")]
        public async Task<ActionResult<List<Album>>> GetAllInOneAuthor(string author)
        {
            Console.WriteLine($"AlbumController.GetAllInOneAuthor lancé sur {author}");
            if (author != "")
            {
                List<Album> albums = await _albumService.GetAlbumsInOneAuthorAsync(author, false);
                albums.Sort((x, y) => string.Compare(x.title, y.title));
                return (albums != null) ? Ok(albums) : NotFound();
            }
            return BadRequest("author ne doit pas être vide");

        }

        [HttpGet]
        [Route("genre/{genre}")]
        public async Task<ActionResult<List<Album>>> GetAllInOneGenre(string genre)
        {
            Console.WriteLine($"AlbumController.GetAllInOneGenre lancé sur {genre}");
            if (genre != "")
            {
                List<Album> albums = await _albumService.GetAlbumsInOneGenreAsync(genre, false);
                albums.Sort((x, y) => string.Compare(x.title, y.title));
                return (albums != null) ? Ok(albums) : NotFound();
            }
            return BadRequest("genre ne doit pas être vide");

        }


        #endregion

        #region GetRandomAlbums

        [HttpGet]
        [Route("random")]
        public async Task<ActionResult<List<Album>>> GetRandomAlbums()
        {
            Console.WriteLine("AlbumController.GetRandomAlbums lancé");
            List<Album> albums = await _albumService.GetAlbumsAsync(true);
            return (albums != null) ? Ok(albums) : NotFound();
        }

        [HttpGet]
        [Route("random/author/{author}")]
        public async Task<ActionResult<List<Album>>> GetRandomInOneAuthor(string author)
        {
            Console.WriteLine($"AlbumController.GetRandomInOneAuthor lancé sur {author}");
            if (author != "")
            {
                List<Album> albums = await _albumService.GetAlbumsInOneAuthorAsync(author, true);
                return (albums != null) ? Ok(albums) : NotFound();
            }
            return BadRequest("author ne doit pas être vide");

        }

        [HttpGet]
        [Route("random/genre/{genre}")]
        public async Task<ActionResult<List<Album>>> GetRandomInOneGenre(string genre)
        {
            Console.WriteLine($"AlbumController.GetRandomInOneGenre lancé sur {genre}");
            if (genre != "")
            {
                List<Album> albums = await _albumService.GetAlbumsInOneGenreAsync(genre, true);
                return (albums != null) ? Ok(albums) : NotFound();
            }
            return BadRequest("genre ne doit pas être vide");


        }

        #endregion

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Album>> GetOne(string id)
        {
            Console.WriteLine($"AlbumController.GetOne lancé sur {id}");
            if (id != "")
            {
                Album album = await _albumService.GetOneAlbumAsync(id);
                return (album != null) ? Ok(album) : NotFound();
            }
            return BadRequest("id ne doit pas être nul");
        }

        [HttpGet]
        [Route("search/{searchedValue}")]
        public async Task<ActionResult<List<Album>>> Search(string searchedValue)
        {
            List<Album> albums = await _albumService.Search(searchedValue.ToLower());
            return (albums.Count > 0) ? Ok(albums) : NotFound();
        }

        [HttpGet]
        [Route("random/one")]
        public async Task<ActionResult<Album>> GetOneRandom()
        {
            Console.WriteLine("Album.Controller.GetOneRandom lancé");
            Album album = await _albumService.GetOneRandomAlbumAsnc();
            return (album != null) ? Ok(album) : BadRequest();
        }

        //[Authorize]
        //[HttpPost]
        //public async Task<IActionResult> Post(Album newAlbum)
        //{
        //    await _albumService.CreateAlbumAsync(newAlbum);

        //    return CreatedAtAction(nameof(GetOne), new { id = newAlbum._id }, newAlbum);
        //}
    }
}