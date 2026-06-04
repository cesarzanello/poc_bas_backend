using Application.Dtos;
using Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace Application.Commond.Mapping
{
    public class Caso1Mapping: Profile
    {
        public Caso1Mapping()
        {
            CreateMap<Caso1Dto, Caso1ResponseDto>().ReverseMap();
        }

    }
}


