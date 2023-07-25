using Data.Base;
using Data.Entities;
using Microsoft.EntityFrameworkCore;


namespace Data.Manager
{
    public class ProductosManager : BaseManager<Productos>
    {
        public override Task<List<Productos>> Borrar(Productos entity)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Productos>> BuscarAsync(Productos entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<List<Productos>> BuscarListaAsync()
        {
            return await contextSingleton.Productos.Where(x => x.Activo == true).ToListAsync();
        }
    }
}