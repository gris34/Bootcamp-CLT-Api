using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Query.GetProducts
{
    public class GetProductsHandler : IRequestHandler<GetProductsQuery, List<ProductResponse>>
    {
        private readonly PostgresDbContext _postgresDbContext;
        private readonly ILogger<GetProductsHandler> _logger;

        public GetProductsHandler(PostgresDbContext postgresDbContext, ILogger<GetProductsHandler> logger)
        {
            _postgresDbContext = postgresDbContext;
            _logger = logger;
        }

        public async Task<List<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ejecutando caso de uso: obtener lista de productos.");

            try
            {
                var productsEntity = await _postgresDbContext.Products
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                if (productsEntity.Count == 0)
                {
                    _logger.LogWarning("No se encontraron productos en la base de datos. Cantidad={Cantidad}", 0);
                    return new List<ProductResponse>();
                }

                var productResponses = productsEntity.Select(p => new ProductResponse(
                    p.Id,
                    p.Codigo,
                    p.Nombre,
                    p.Descripcion ?? string.Empty,
                    p.Precio,
                    p.Activo,
                    p.CategoriaId,
                    p.FechaCreacion,
                    p.FechaActualizacion,
                    p.CantidadStock
                )).ToList();

                _logger.LogInformation("Productos obtenidos correctamente. Cantidad={Cantidad}", productResponses.Count);
                return productResponses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener la lista de productos.");
                throw;
            }
        }
    }
}
