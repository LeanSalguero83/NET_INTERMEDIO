﻿using Data.Entities;
namespace Web.Models
{
    public class ProductosViewModel
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public string Imagen { get; set; }
        public bool Activo { get; set; }
        public IFormFile? Imagen_Archivo { get; set; }

        public static implicit operator ProductosViewModel(Productos producto)
        {
            var productoViewModel = new ProductosViewModel();
            productoViewModel.Id = producto.Id;
            productoViewModel.Descripcion = producto.Descripcion;
            productoViewModel.Precio = producto.Precio;
            productoViewModel.Stock = producto.Stock;
            productoViewModel.Imagen = producto.Imagen;
            productoViewModel.Activo = producto.Activo;
            return productoViewModel;

        }
    }


}
