using Api.Interfaces;
using Data.Entities;
using Data.Manager;
using Common.Helpers;

namespace Api.Services
{
    public class UsuariosService : IUsuariosService
    {
        private readonly UsuariosManager _manager;

        public UsuariosService(UsuariosManager manager)
        {
            _manager = manager;
        }

        public async Task<List<Usuarios>> BuscarUsuariosAsync()
        {
            try
            {
                var result = await _manager.BuscarListaAsync();
                return result;
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public async Task<List<Usuarios>> GuardarUsuarioASync(Usuarios usuario)
        {
            try
            {
                var existe = await _manager.BuscarUsuarioAsync(usuario);
                usuario.Clave = EncryptHelper.Encriptar(usuario.Clave);
                if (existe != null)
                {
                    return await _manager.BuscarListaAsync();
                }
                else
                {
                    var result = await _manager.Guardar(usuario, usuario.Id);
                }
                return await _manager.BuscarListaAsync();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Usuarios>> EliminarUsuarioASync(Usuarios usuario)
        {
            try
            {
                var result = await _manager.Eliminar(usuario);
                return await _manager.BuscarListaAsync();
            }
            catch (Exception ex)
            {
                
                throw ex;
            }
        }
    }
}