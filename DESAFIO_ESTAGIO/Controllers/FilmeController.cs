using Microsoft.AspNetCore.Mvc;
using DESAFIO_ESTAGIO;
using Microsoft.Identity.Client;

namespace DESAFIO_ESTAGIO.Controllers
{
    [ApiController]
    [Route("api/filmes")]
    public class FilmeController : ControllerBase
    {
       
        private readonly List<Filme> _filmes = new();
        [HttpGet]
        public IActionResult Listar(string? titulo = null)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return Ok(_filmes);
            }
            List<Filme> filtrados = new();
            foreach (var filme in _filmes)
            {
                if (filme.Titulo == titulo)
                {
                    filtrados.Add(filme);
                }
            }
            return Ok(filtrados);
        }
        public FilmeController()
        {
            Filme filme = new Filme();
            filme.Id = 1;
            filme.Titulo = "It, a coisa";
            filme.Diretor = "Coringa";
            _filmes.Add(filme);
        }
        [HttpGet("{id:int}")]
        public IActionResult BuscarId(int id) 
        {
            foreach (var filme in _filmes)
            {
                if (filme.Id == id)
                {
                    return Ok(filme);
                }
            }   
            return NotFound();
        }
        [HttpPost]
        public IActionResult Cadastrar(Filme filme)
        {

            return Ok(filme);
        }

        [HttpPut("{id:int}/voto")]
        public IActionResult Votar(int id, Voto voto)
        {
            foreach (var filme in _filmes)
            {
                if (filme.Id == id )
                {
                    voto.FilmeId = id;
                   var aux = filme.AdicionarVoto(voto);
                    if (aux == false)
                    {
                       return BadRequest();
                    }
                    return NoContent();
                }
            }
            return NotFound();
        }
    }
}
