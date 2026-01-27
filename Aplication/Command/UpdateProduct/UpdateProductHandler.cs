using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Command.UpdateProduct
{
    public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, ProductResponse>
    {
        private readonly PostgresDbContext _postgresDbContext;
        private readonly ILogger<UpdateProductHandler> _logger;

        public UpdateProductHandler(PostgresDbContext postgresDbContext, ILogger<UpdateProductHandler> logger)
        {
            _postgresDbContext = postgresDbContext;
            _logger = logger;
        }

        public async Task<ProductResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ejecutando caso de uso: actualizar producto. ProductId={ProductId}", request.Id);

            try
            {
                var productEntity = await _postgresDbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (productEntity == null)
                {
                    _logger.LogWarning("Producto no encontrado para actualización. ProductId={ProductId}", request.Id);
                    throw new KeyNotFoundException("Producto no encontrado.");
                }

                productEntity.Codigo = request.ProductRequest.Codigo;
                productEntity.Nombre = request.ProductRequest.Nombre;
                productEntity.Descripcion = request.ProductRequest.Descripcion;
                productEntity.Precio = request.ProductRequest.Precio;
                productEntity.Activo = request.ProductRequest.Activo;
                productEntity.CategoriaId = request.ProductRequest.CategoriaId;
                productEntity.CantidadStock = request.ProductRequest.CantidadStock;
                productEntity.FechaActualizacion = DateTime.UtcNow;

                await _postgresDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Producto actualizado y persistido correctamente. ProductId={ProductId} Codigo={Codigo}",
                    productEntity.Id,
                    productEntity.Codigo
                );

                return new ProductResponse(
                    productEntity.Id,
                    productEntity.Codigo,
                    productEntity.Nombre,
                    productEntity.Descripcion ?? string.Empty,
                    productEntity.Precio,
                    productEntity.Activo,
                    productEntity.CategoriaId,
                    productEntity.FechaCreacion,
                    productEntity.FechaActualizacion,
                    productEntity.CantidadStock
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error de base de datos al actualizar producto (posible código duplicado). ProductId={ProductId} Codigo={Codigo}",
                    request.Id,
                    request.ProductRequest?.Codigo
                );
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar producto. ProductId={ProductId}", request.Id);
                throw;
            }
        }
    }
}
