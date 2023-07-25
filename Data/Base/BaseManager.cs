using Microsoft.EntityFrameworkCore;
using Data;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Data.Base
{
    public abstract class BaseManager<T> where T : class
    {

        protected readonly ApplicationDbContext context;


        public  BaseManager(ApplicationDbContext context)
        {
            this.context = context;
        }


        
        public abstract Task<List<T>> BuscarListaAsync();
        public abstract Task<List<T>> BuscarAsync(T entity);
        public abstract Task<List<T>> Borrar(T entity);
     

       
        public async Task<bool> Guardar(T entity, int id)
        {
            try
            {
                if (id == 0)
                    context.Entry(entity).State = EntityState.Added;
                else
                    context.Entry(entity).State = EntityState.Modified;

                var resultado = await context.SaveChangesAsync() > 0;
                context.Entry(entity).State = EntityState.Detached;
                return resultado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> Eliminar(T entity)
        {
            try
            {
                context.Entry(entity).State = EntityState.Modified;
                var resultado = await context.SaveChangesAsync() > 0;
                return resultado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        
    }
}