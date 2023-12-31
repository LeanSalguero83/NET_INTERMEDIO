﻿using Api.Interfaces;
using Common.Helpers;
using Data.Entities;
using Data.Manager;

namespace Api.Services
{
    public class ServiciosService : IServiciosService
    {
        private readonly ServiciosManager _manager;

        public ServiciosService(ServiciosManager manager)
        {
            _manager = manager;
        }

        public async Task<List<Servicios>> BuscarServiciosAsync()
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

        public async Task<List<Servicios>> GuardarServicioASync(Servicios servicio)
        {
            try
            {
                var result = await _manager.GuardarAsync(servicio);
				return result;
			}
            catch (Exception ex)
            {
                
                throw ex;
            }
        }

        public async Task<List<Servicios>> EliminarServicioASync(Servicios servicio)
        {
            try
            {
                var result = await _manager.Borrar(servicio);
				return result;
			}
            catch (Exception ex)
            {
                
                throw ex;
            }

        }
    }
}