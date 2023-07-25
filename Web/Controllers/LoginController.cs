using Data.Base;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using Data.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        public LoginController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

        
        public ActionResult Login()
        {
            return View();
        }

        public async Task<IActionResult> Ingresar(LoginDto login)
        {
            var baseApi = new BaseApi(_httpClient);
            var token = await baseApi.PostToApi("Authenticate/Login", login);
            var resultadoLogin = token as OkObjectResult;
            if (resultadoLogin.Value.ToString() == "true")
            {
                return View("~/Views/Home/Index.cshtml");
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }
    }
}
