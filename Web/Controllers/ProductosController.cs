﻿using Data.Base;
using Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Microsoft.AspNetCore.Authorization;
namespace Web.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IHttpClientFactory _httpClient;
        public ProductosController(IHttpClientFactory httpClient)
        {
            _httpClient = httpClient;
        }

		[Authorize]
		public IActionResult Productos()
        {
            return View();
        }

        public IActionResult ProductosAddPartial([FromBody] Productos producto)
        {
            var usuViewModel = new ProductosViewModel();
            if (producto != null)
                usuViewModel = producto;
            return PartialView("~/Views/Productos/Partial/ProductosAddPartial.cshtml", usuViewModel);
        }

        public async Task<IActionResult> GuardarProducto(Productos producto)
        {
            var token = HttpContext.Session.GetString("Token");
            var baseApi = new BaseApi(_httpClient);
            if (producto.Imagen_Archivo != null && producto.Imagen_Archivo.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    producto.Imagen_Archivo.CopyTo(ms);
                    var imagenBytes = ms.ToArray();
                    producto.Imagen = Convert.ToBase64String(imagenBytes);
                }
            }
            producto.Imagen_Archivo = null;
            var productos = await baseApi.PostToApi("Productos/GuardarProducto", producto, token);
            return RedirectToAction("Productos", "Productos");
        }

        public async Task<IActionResult> EliminarProducto([FromBody] Productos producto)
        {
            var token = HttpContext.Session.GetString("Token");
            var baseApi = new BaseApi(_httpClient);
            producto.Activo = false;
            var productos = await baseApi.PostToApi("Productos/EliminarProducto", producto, token);
            return RedirectToAction("Productos", "Productos");
        }
    }
}