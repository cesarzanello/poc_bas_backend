using Application.Commond.Interface.ICaso1;
using Application.Dtos;
using Application.UserCase.V1.Caso1.Queries;
using AutoMapper;
using Domain.Dtos;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Xunit;

namespace Application.Test.UserCase.V1.Caso1
{
    public class GetCaso1Test
    {
        private readonly Mock<ICaso1CommandQuery> _caso1CommandQueryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly GetCaso1Handler _handler;

        public GetCaso1Test()
        {
            _caso1CommandQueryMock = new Mock<ICaso1CommandQuery>();
            _mapperMock = new Mock<IMapper>();
            _handler = new GetCaso1Handler(_caso1CommandQueryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnMappedResponse_WhenCaso1Exists()
        {
            // Arrange
            var requestId = Guid.NewGuid();
            var caso1Dto = new Caso1Dto { Id = requestId, Nombre = "Test Nombre" };
            var responseDto = new Caso1ResponseDto { Id = requestId, Nombre = "Test Nombre" };

            _caso1CommandQueryMock
                .Setup(x => x.GetCaso1(requestId))
                .ReturnsAsync(caso1Dto);

            _mapperMock
                .Setup(x => x.Map<Caso1ResponseDto>(caso1Dto))
                .Returns(responseDto);

            var request = new GetCaso1 { Id = requestId };

            // Act
            var result = await _handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(requestId, result.Id);
            Assert.Equal("Test Nombre", result.Nombre);

            _caso1CommandQueryMock.Verify(x => x.GetCaso1(requestId), Times.Once);
            _mapperMock.Verify(x => x.Map<Caso1ResponseDto>(caso1Dto), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenCaso1DoesNotExist()
        {
            // Arrange
            var requestId = Guid.NewGuid();

            _caso1CommandQueryMock
                .Setup(x => x.GetCaso1(requestId))
                .ReturnsAsync((Caso1Dto)null); // Simula que no se encuentra el caso

            var request = new GetCaso1 { Id = requestId };

            // Act & Assert
            await Assert.ThrowsAsync<NullReferenceException>(() => _handler.Handle(request, CancellationToken.None));

            _caso1CommandQueryMock.Verify(x => x.GetCaso1(requestId), Times.Once);
            _mapperMock.Verify(x => x.Map<Caso1ResponseDto>(It.IsAny<Caso1Dto>()), Times.Never);
        }
    }
}
