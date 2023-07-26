using Data;
using Data.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

			var validarUsuario = _context.Usuarios.Where(x => x.Mail == usuario.Mail && x.Clave == usuario.Clave).Include(x => x.Roles).FirstOrDefault();

			if (validarUsuario != null)
            {
				return Ok(validarUsuario.Nombre + ";" + validarUsuario.Roles.Nombre + ";" + validarUsuario.Mail);
			}
            else
            {
				return Unauthorized();
			}
        }
    }
}
