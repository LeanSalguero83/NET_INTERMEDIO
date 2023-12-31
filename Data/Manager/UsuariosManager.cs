﻿using Data.Base;
using Data.Dtos;
using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Data.Manager
{
    public class UsuariosManager : BaseManager<Usuarios>
    {
        public UsuariosManager(ApplicationDbContext context) : base(context) { }
        public override Task<List<Usuarios>> Borrar(Usuarios entity)
        {
            throw new NotImplementedException();
        }

        public override Task<List<Usuarios>> BuscarAsync(Usuarios entity)
        {
            throw new NotImplementedException();
        }

        public async override Task<List<Usuarios>> BuscarListaAsync()
        {
            return await context.Usuarios.Where(x => x.Activo == true).Include(x => x.Roles).ToListAsync();
        }

        public async Task<Usuarios> BuscarUsuarioAsync(Usuarios usuario)
        {
            return await context.Usuarios.FirstOrDefaultAsync(x => x.Mail == usuario.Mail);
        }

        public async Task<Usuarios> BuscarUsuarioGoogleAsync(LoginDto usuario)
        {
            var result = await context.Usuarios.FirstOrDefaultAsync(x => x.Mail == usuario.Mail && x.Activo == true);
            return result;
        }
    }
}