using Application.Commond.Interface.ICaso1;
using Application.Dtos;
using Domain.Dtos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service.SClase
{
    public  class Caso1CommandQuery(IConfiguration _configuration) : ICaso1CommandQuery
    {
        public async Task<Caso1Dto> GetCaso1(Guid id)
        {
            return new Caso1Dto
            {
                Id = id,
                Nombre = "Caso 1DE BASE",
                FechaAlta = DateTime.Now 
            };
        }
    }
}
