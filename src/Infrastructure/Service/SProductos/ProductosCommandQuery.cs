using Application.Commond.Interface.IProductos;
using Application.Commond.Interface.ITenants;
using Application.Dtos;
using Dapper;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace Infrastructure.Service.SProductos
{
    public class ProductosCommandQuery(ApplicationDbContext dbContext, ITenantsCommandQuery tenantsCommandQuery, ILogger<ProductosCommandQuery> logger) : IProductosCommandQuery
    {
        public async Task<ProductoResponseDto> CreateProductoAsync(Guid productoId, CreateProductoRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenantExists = await tenantsCommandQuery.ExistsTenantAsync(request.TenantId, cancellationToken);
                if (!tenantExists)
                {
                    logger.LogWarning("CreateProducto falló: tenant {TenantId} no existe.", request.TenantId);
                    throw new KeyNotFoundException("No se encontró el tenant indicado.");
                }

                var producto = new Producto
                {
                    Id = productoId,
                    TenantId = request.TenantId,
                    Codigo = request.Codigo,
                    Nombre = request.Nombre,
                    Precio = request.Precio
                };

                dbContext.Productos.Add(producto);
                await dbContext.SaveChangesAsync(cancellationToken);

                return new ProductoResponseDto
                {
                    Id = producto.Id,
                    TenantId = producto.TenantId,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creando producto {ProductoId} para tenant {TenantId}", productoId, request.TenantId);
                throw;
            }
        }

        public async Task<ProductoResponseDto> UpdateProductoAsync(Guid productoId, UpdateProductoRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                var producto = await dbContext.Productos.FirstOrDefaultAsync(x => x.Id == productoId, cancellationToken);
                if (producto is null)
                {
                    logger.LogWarning("UpdateProducto falló: producto {ProductoId} no existe.", productoId);
                    throw new KeyNotFoundException("No se encontró el producto indicado.");
                }

                var codigoExists = await dbContext.Productos.AnyAsync(x => x.TenantId == producto.TenantId && x.Codigo == request.Codigo && x.Id != productoId, cancellationToken);
                if (codigoExists)
                {
                    logger.LogWarning("UpdateProducto falló: código duplicado {Codigo} para tenant {TenantId}.", request.Codigo, producto.TenantId);
                    throw new InvalidOperationException("Ya existe un producto con el mismo código para el tenant indicado.");
                }

                producto.Codigo = request.Codigo;
                producto.Nombre = request.Nombre;
                producto.Precio = request.Precio;

                await dbContext.SaveChangesAsync(cancellationToken);

                return new ProductoResponseDto
                {
                    Id = producto.Id,
                    TenantId = producto.TenantId,
                    Codigo = producto.Codigo,
                    Nombre = producto.Nombre,
                    Precio = producto.Precio
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error actualizando producto {ProductoId}", productoId);
                throw;
            }
        }

        public async Task DeleteProductoAsync(Guid productoId, CancellationToken cancellationToken = default)
        {
            try
            {
                var producto = await dbContext.Productos.FirstOrDefaultAsync(x => x.Id == productoId, cancellationToken);
                if (producto is null)
                {
                    logger.LogWarning("DeleteProducto falló: producto {ProductoId} no existe.", productoId);
                    throw new KeyNotFoundException("No se encontró el producto indicado.");
                }

                dbContext.Productos.Remove(producto);
                await dbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error eliminando producto {ProductoId}", productoId);
                throw;
            }
        }

        public async Task<ProductoResponseDto?> GetProductoByIdAsync(Guid productoId, CancellationToken cancellationToken = default)
        {
            try
            {
                const string sql = """
                    SELECT Id, TenantId, Codigo, Nombre, Precio
                    FROM productos
                    WHERE Id = @productoId
                    """;

                var connection = dbContext.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                return await connection.QueryFirstOrDefaultAsync<ProductoResponseDto>(
                    new CommandDefinition(sql, new { productoId }, cancellationToken: cancellationToken));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error obteniendo producto por id {ProductoId}", productoId);
                throw;
            }
        }

        public async Task<IReadOnlyCollection<ProductoResponseDto>> GetProductosByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                const string sql = """
                    SELECT Id, TenantId, Codigo, Nombre, Precio
                    FROM productos
                    WHERE TenantId = @tenantId
                    ORDER BY Nombre
                    """;

                var connection = dbContext.Database.GetDbConnection();
                if (connection.State != ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                }

                var productos = await connection.QueryAsync<ProductoResponseDto>(
                    new CommandDefinition(sql, new { tenantId }, cancellationToken: cancellationToken));

                return productos.ToList();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error obteniendo productos del tenant {TenantId}", tenantId);
                throw;
            }
        }
    }
}
