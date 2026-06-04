using Application.Commond.Interface.ICaso1;
using Application.Dtos;
using AutoMapper;
using Domain.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UserCase.V1.Caso1.Queries
{
    public class GetCaso1 : IRequest<Caso1ResponseDto>
    {
        public Guid Id { get; set; }
    }
    public class GetCaso1Handler(ICaso1CommandQuery _caso1, IMapper _mapper) : IRequestHandler<GetCaso1, Caso1ResponseDto>
    {
     
        public async Task<Caso1ResponseDto> Handle(GetCaso1 request, CancellationToken cancellationToken)
        {
            // Simulate fetching data from a database or service
            var caso1 = await _caso1.GetCaso1(request.Id);
            if (caso1 == null)
            {
                throw new NullReferenceException("No se encontro el Caso1");
            }
            var response = _mapper.Map<Caso1ResponseDto>(caso1);
            return response;
        }
    }

}
