using Application.Dtos;
using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commond.Interface.ICaso1
{
    public interface ICaso1CommandQuery : IScopedService
    {
        Task<Caso1Dto> GetCaso1(Guid id);
    }
}
