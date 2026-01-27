using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Query.GetProductById
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, ProductResponse?>
    {
        private readonly PostgresDbContext _postgresdbContext;
        private readonly ILogger<GetProductByIdHandler> _logger;

        public GetProductByIdHandler(PostgresDbContext dbContext, ILogger<GetProductByIdHandler> logger)
        {
            _postgresdbContext = dbContext;
            _logger = logger;
        }

        public async Task<ProductResponse?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ejecutando caso de uso: obtener producto por id. ProductId={ProductId}", request.Id);

            try
            {
                var productEntity = await _postgresdbContext.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (productEntity == null)
                {
                    _logger.LogWarning("Producto no encontrado en la base de datos. ProductId={ProductId}", request.Id);
                    return null;
                }

                _logger.LogInformation("Producto obtenido correctamente. ProductId={ProductId}", request.Id);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener producto por id. ProductId={ProductId}", request.Id);
                throw;
            }
        }
    }
}
