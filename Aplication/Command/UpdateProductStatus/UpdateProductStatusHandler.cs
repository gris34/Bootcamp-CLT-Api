using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Aplication.Command.UpdateProductStatus;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Commands
{
    public class UpdateProductStatusHandler : IRequestHandler<UpdateProductStatusCommand, ProductResponse>
    {
        private readonly PostgresDbContext _postgresDbContext;
        private readonly ILogger<UpdateProductStatusHandler> _logger;

        public UpdateProductStatusHandler(PostgresDbContext postgresDbContext, ILogger<UpdateProductStatusHandler> logger)
        {
            _postgresDbContext = postgresDbContext;
            _logger = logger;
        }

        public async Task<ProductResponse> Handle(UpdateProductStatusCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Ejecutando caso de uso: actualizar estado de producto. ProductId={ProductId} Activo={Activo}",
                request.Id,
                request.Activo
            );

            try
            {
                var productEntity = await _postgresDbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (productEntity == null)
                {
                    _logger.LogWarning("Producto no encontrado para actualización de estado. ProductId={ProductId}", request.Id);
                    throw new KeyNotFoundException("Producto no encontrado.");
                }

                if (request.Activo.HasValue)
                {
                    productEntity.Activo = request.Activo.Value;
                    productEntity.FechaActualizacion = DateTime.UtcNow;
                }

                await _postgresDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Estado de producto actualizado correctamente. ProductId={ProductId} Activo={Activo}",
                    productEntity.Id,
                    productEntity.Activo
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
                _logger.LogError(ex, "Error de base de datos al actualizar estado de producto. ProductId={ProductId}", request.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al actualizar estado de producto. ProductId={ProductId}", request.Id);
                throw;
            }
        }
    }
}
