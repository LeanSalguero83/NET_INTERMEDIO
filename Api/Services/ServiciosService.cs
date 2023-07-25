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
                GenerateLogHelper.LogError(ex, "ServiciosService", "BuscarServiciosAsync");
                throw ex;
            }
        }

        public async Task<List<Servicios>> GuardarServicioASync(Servicios servicio)
        {
            try
            {
                var result = await _manager.Guardar(servicio, servicio.Id);
                return await _manager.BuscarListaAsync();
            }
            catch (Exception ex)
            {
                GenerateLogHelper.LogError(ex, "ServiciosService", "GuardarServicioASync");
                throw ex;
            }
        }

        public async Task<List<Servicios>> EliminarServicioASync(Servicios servicio)
        {
            try
            {
                var result = await _manager.Eliminar(servicio);
                return await _manager.BuscarListaAsync();
            }
            catch (Exception ex)
            {
                GenerateLogHelper.LogError(ex, "ServiciosService", "EliminarServicioASync");
                throw ex;
            }

        }
    }
}