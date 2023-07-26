using Data.Base;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Manager
{
    public class ServiciosManager : BaseManager<Servicios>
    {
        public ServiciosManager(ApplicationDbContext context) : base(context)
        {
        }

        public async override Task<List<Servicios>> Borrar(Servicios servicio)
        {
			context.Database.ExecuteSqlRaw($"EliminarServicio {servicio.Id}");
			return context.Servicios.FromSqlRaw("ObtenerServicios").ToList();
		}

        public override Task<List<Servicios>> BuscarAsync(Servicios entity)
        {
            throw new NotImplementedException();
        }

		public async Task<List<Servicios>> GuardarAsync(Servicios servicio)
		{
			var p = context.Database.ExecuteSqlRaw($"GuardaroActualizarServicios {servicio.Id}, {servicio.Nombre}, {servicio.Activo}");
			return context.Servicios.FromSqlRaw("ObtenerServicios").ToList();
		}

		public async  override Task<List<Servicios>> BuscarListaAsync()
        {
            return  context.Servicios.FromSqlRaw("ObtenerServicios").ToList();
		}
    }
}
