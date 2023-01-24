using CulturalTastes_API_.NET.Models;
using CulturalTastes_API_.NET.Services;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;

namespace FilmStoreApi.Controllers;

[ApiController]
[Route("[controller]")]
public class FilmController : ControllerBase
{
    private readonly FilmService _filmService;
    
    public FilmController(FilmService filmService) =>
        _filmService = filmService;

    [HttpGet]
    [Route("getGenres")]
    public ActionResult<string[]> GetGenres()
    {
        return Ok(_filmService.GetGenres());
    }

    [HttpGet("images/{url}")]
    public ActionResult GetImage(string url)
    {
        Console.WriteLine("FilmController.GetImage sur "+url);

        var path = Path.Combine(Directory.GetCurrentDirectory(), "images", url);
        var extension = Path.GetExtension(path);

        return PhysicalFile(path, "image/jpeg");
    }

    #region GetALL

    [HttpGet]
    public async Task<ActionResult<List<Film>>> GetAll()
    {
        Console.WriteLine("FilmController.Get lancé");
        return Ok(await _filmService.GetFilmsAsync(false));
    }

    [HttpGet]
    [Route("getAllInOneAuthor/{author}")]
    public async Task<ActionResult<List<Film>>> GetAllInOneAuthor(string author)
    {
        Console.WriteLine($"FilmController.GetAllInOneAuthor lancé sur {author}");
        List<Film> films = await _filmService.GetFilmsInOneAuthorAsync(author,false);
        return (films is not null) ? Ok(films) : NotFound();
    }

    [HttpGet]
    [Route("getAllInOneGenre/{genre}")]
    public async Task<ActionResult<List<Film>>> GetAllInOneGenre(string genre)
    {
        Console.WriteLine($"FilmController.GetAllInOneGenre lancé sur {genre}");
        List<Film> films = await _filmService.GetFilmsInOneGenreAsync(genre,false);
        return (films is not null) ? Ok(films) : NotFound();
    }         
    

    #endregion

    #region GetRandomFilms

    [HttpGet]
    [Route("getRandomFilms")]
    public async Task<ActionResult<List<Film>>> GetRandomFilms()
    {
        Console.WriteLine("FilmController.GetRandomFilms lancé");
        List<Film> films = await _filmService.GetFilmsAsync(true);
        return (films is not null) ? Ok(films) : NotFound();
    }

    [HttpGet]
    [Route("getRandomInOneAuthor/{author}")]
    public async Task<ActionResult<List<Film>>> GetRandomInOneAuthor(string author)
    {
        Console.WriteLine($"FilmController.GetRandomInOneAuthor lancé sur {author}");
        List<Film> films = await _filmService.GetFilmsInOneAuthorAsync(author, true);
        return (films is not null) ? Ok(films) : NotFound();
    }

    [HttpGet]
    [Route("getRandomInOneGenre/{genre}")]
    public async Task<ActionResult<List<Film>>> GetRandomInOneGenre(string genre)
    {
        Console.WriteLine($"FilmController.GetRandomInOneGenre lancé sur {genre}");
        List<Film> films = await _filmService.GetFilmsInOneGenreAsync(genre, true);
        return (films is not null) ? Ok(films) : NotFound();
    }

    #endregion

    [HttpGet]
    [Route("getOneFilm/{id}")]
    public async Task<ActionResult<Film>> GetOne(string id)
    {
        Console.WriteLine($"FilmController.GetOne lancé sur {id}");
        var film = await _filmService.GetOneFilmAsync(id);     

        if (film is null)
        {
            return NotFound();
        }

        return Ok(film);
    }

    [HttpGet]
    [Route("getOneRandom")]
    public async Task<Film> GetOneRandom()
    {
        Console.WriteLine("Film.Controller.GetOneRandom lancé");
        return await _filmService.GetOneRandomFilmAsnc();
    }


    [HttpPost]
    public async Task<IActionResult> Post(Film newFilm)
    {
        await _filmService.CreateFilmAsync(newFilm);

        return CreatedAtAction(nameof(GetOne), new { id = newFilm._id }, newFilm);
    }

    //[HttpPut("{id:length(24)}")]
    //public async Task<IActionResult> Update(string id, Film updatedFilm)
    //{
    //    var film = await _filmService.GetOneFilmAsync(id);

    //    if (film is null)
    //    {
    //        return NotFound();
    //    }

    //    updatedFilm._id = film._id;

    //    await _filmService.UpdateFilmAsync(id, updatedFilm);

    //    return NoContent();
    //}

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var film = await _filmService.GetOneFilmAsync(id);

        if (film is null)
        {
            return NotFound();
        }

        await _filmService.RemoveAsync(id);

        return NoContent();
    }
}