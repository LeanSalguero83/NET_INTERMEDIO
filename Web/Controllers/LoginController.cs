﻿using Data.Base;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Data.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Net.Mail;
using System.Net;
using Newtonsoft.Json;
using Web.Models;
using Microsoft.AspNetCore.Authentication.Google;
using Web.Services;
using Data.Manager;
using Web.Interfaces;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;
        private readonly IUsuarioService _usuarioService;
        public LoginController(IHttpClientFactory httpClient, IConfiguration configuration, IUsuarioService usuarioService)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _smtpClient = new SmtpClient();
            _usuarioService = usuarioService;
        }

        public IActionResult OlvidoClave()
        {
            return View();
        }

        public IActionResult RecuperarCuenta()
        {
            return View();
        }

        public IActionResult CrearCuenta()
        {
            return View();
        }

        public async Task<ActionResult> LoginAsync()
        {
            if (TempData["ErrorLogin"] != null)
                ViewBag.ErrorLogin = TempData["ErrorLogin"].ToString();
            return View();
        }

        public async Task<IActionResult> Ingresar(LoginDto login)
        {
            var baseApi = new BaseApi(_httpClient);
            var token = await baseApi.PostToApi("Authenticate/Login", login, "");
            var resultadoLogin = token as OkObjectResult;

            if (resultadoLogin != null)
            {
                var resultadoSplit = resultadoLogin.Value.ToString().Split(";");
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                Claim claimNombre = new(ClaimTypes.Name, resultadoSplit[1]);
                Claim claimRole = new(ClaimTypes.Role, resultadoSplit[2]);
                Claim claimEmail = new(ClaimTypes.Email, resultadoSplit[3]);

                identity.AddClaim(claimNombre);
                identity.AddClaim(claimRole);
                identity.AddClaim(claimEmail);

                ClaimsPrincipal usuarioPrincipal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, usuarioPrincipal, new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.Now.AddDays(1)
                });
                ViewBag.NombreUsuario = resultadoSplit[1];
                HttpContext.Session.SetString("Token", resultadoSplit[0]);
                var homeViewModel = new HomeViewModel();
                homeViewModel.Token = resultadoSplit[0];

                return View("~/Views/Home/Index.cshtml", homeViewModel);
            }
            else
            {
                TempData["ErrorLogin"] = "La contraseña o el mail no coinciden";
                return RedirectToAction("Login", "Login");
            }
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Login");
        }

        public async Task<IActionResult> EnviarMail(LoginDto login)
        {
            var guid = Guid.NewGuid();
            var numeros = new String(guid.ToString().Where(Char.IsDigit).ToArray());
            var seed = int.Parse(numeros.Substring(0, 6));
            var random = new Random(seed);
            var codigo = random.Next(000000, 999999);

            login.Codigo = codigo;

            var baseApi = new BaseApi(_httpClient);
            var token = HttpContext.Session.GetString("Token");
            var response = await baseApi.PostToApi("RecuperarCuenta/GuardarCodigo", login,token);
            var resultadoLogin = response as OkObjectResult;

            if (resultadoLogin != null && resultadoLogin.Value.ToString() == "true")
            {
                MailMessage mail = new();

                string CuerpoMail = CuerpoMailLogin(codigo);

                mail.From = new MailAddress(_configuration["ConfiguracionMail:Usuario"]);
                mail.To.Add(login.Mail);
                mail.Subject = "Codigo Recuperacion";
                mail.Body = CuerpoMail;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.Normal;

                _smtpClient.Host = _configuration["ConfiguracionMail:DireccionServidor"];
                _smtpClient.Port = int.Parse(_configuration["ConfiguracionMail:Puerto"]);
                _smtpClient.EnableSsl = bool.Parse(_configuration["ConfiguracionMail:Ssl"]);
                _smtpClient.UseDefaultCredentials = false;
                _smtpClient.Credentials = new NetworkCredential(_configuration["ConfiguracionMail:Usuario"], _configuration["ConfiguracionMail:Clave"]);

                _smtpClient.Send(mail);
                return RedirectToAction("RecuperarCuenta", "Login");
            }
            else
            {
                TempData["ErrorLogin"] = "El mail que intenta recuperar no existe";
                return RedirectToAction("Login", "Login");
            }
        }

        private static string CuerpoMailLogin(int codigo)
        {
            string separacion = "<br>";
            var mensaje = "<strong>A continuacion se mostrara un codigo que debera ingresar en la web del proyecto NET INTERMEDIO</strong>";
            mensaje += $" <strong>{codigo}</strong> {separacion}";
            return mensaje;
        }

        public async Task<IActionResult> CambiarClave(LoginDto login)
        {
            var baseApi = new BaseApi(_httpClient);
            var token = HttpContext.Session.GetString("Token");
            var response = await baseApi.PostToApi("RecuperarCuenta/CambiarClave", login, token);
            var resultadoLogin = response as OkObjectResult;
            if (resultadoLogin != null && resultadoLogin.Value.ToString() == "true")
            {
                return RedirectToAction("Login", "Login");
            }
            else
            {
                TempData["ErrorLogin"] = "El codigo ingresado no coincide con el enviado al mail";
                return RedirectToAction("Login", "Login");
            }
        }

        public async Task<IActionResult> CrearUsuarioLogin(Usuarios usuario)
        {
            var baseApi = new BaseApi(_httpClient);
            usuario.Id_Rol = 2;
            usuario.Activo = true;
            var token = HttpContext.Session.GetString("Token");
            var response = await baseApi.PostToApi("Usuarios/GuardarUsuario", usuario,token);

            var resultadoLogin = response as OkObjectResult;

            if (resultadoLogin.StatusCode == 200)
            {
                // Necesitaremos deserializar el resultado manualmente usando Newtonsoft.Json
                var listaUsuarios = JsonConvert.DeserializeObject<List<Usuarios>>(resultadoLogin.Value.ToString());

                // Verificar si el usuario que acabamos de crear está en la lista
                if (listaUsuarios.Any(u => u.Mail == usuario.Mail))
                {
                    return RedirectToAction("Login", "Login");
                }
                else
                {
                    TempData["ErrorLogin"] = "No se pudo crear el usuario. Contacte a sistemas";
                    return RedirectToAction("Login", "Login");
                }
            }
            else
            {
                TempData["ErrorLogin"] = "No se pudo crear el usuario. Contacte a sistemas";
                return RedirectToAction("Login", "Login");
            }
        }

        public async Task LoginGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var resultado = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = resultado.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Value,
                claim.Type,
                claim.Issuer,
                claim.OriginalIssuer
            });
            var login = new LoginDto();
            login.Mail = claims.First(c => c.Type == ClaimTypes.Email).Value;



            var usuario = _usuarioService.BuscarUsuario(login).Result;

            if (usuario != null)
            {
                var baseApi = new BaseApi(_httpClient);
                var token = await baseApi.PostToApi("Authenticate/LoginGoogle", login, "");
                var resultadoLogin = token as OkObjectResult;

                var resultadoSplit = resultadoLogin.Value.ToString().Split(";");
                ViewBag.NombreUsuario = resultadoSplit[1];
                HttpContext.Session.SetString("Token", resultadoSplit[0]);
                var homeViewModel = new HomeViewModel();
                homeViewModel.Token = resultadoSplit[0];
                return View("~/Views/Home/Index.cshtml", homeViewModel);

            }
            else
            {
                TempData["ErrorLogin"] = "El usuario no existe";
                return RedirectToAction("Login", "Login");
            }
        }
    }
}