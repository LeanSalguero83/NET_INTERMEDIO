using Data;
using Data.Dtos;
using Microsoft.AspNetCore.Mvc;

using Common.Helpers;

namespace Api.Controllers

{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuthenticateController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginDto usuario)
        {
            usuario.Clave = EncryptHelper.Encriptar(usuario.Clave);
            var validarUsuario = _context.Usuarios.FirstOrDefault(x => x.Mail == usuario.Mail && x.Clave == usuario.Clave);

            if (validarUsuario != null)
            {
                return Ok("true");
            }
            else
            {
                return Ok("false");
            }
        }
    }
}
