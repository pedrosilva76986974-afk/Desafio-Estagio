using Microsoft.AspNetCore.Mvc;

namespace DESAFIO_ESTAGIO.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuariosController : ControllerBase
    {
        [HttpGet]
        public IActionResult Listar()
        {
            List<Usuario> usuarios = new();
            return Ok(usuarios);
        }
        [HttpGet("{id:int}")]
        public IActionResult BuscaId(int id)
        {

            return NotFound();
        }
        [HttpPost]
        public IActionResult Cadastrar()
        {
            return BadRequest();
        }
    }
}
     