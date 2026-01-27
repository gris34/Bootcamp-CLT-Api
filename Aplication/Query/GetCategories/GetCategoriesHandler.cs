using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Query.GetCategories
{
    public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryResponse>>
    {
        private readonly PostgresDbContext _postgresDbContext;
        private readonly ILogger<GetCategoriesHandler> _logger;

        public GetCategoriesHandler(PostgresDbContext postgresDbContext, ILogger<GetCategoriesHandler> logger)
        {
            _postgresDbContext = postgresDbContext;
            _logger = logger;
        }

        public async Task<List<CategoryResponse>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Ejecutando caso de uso: obtener lista de categorías activas.");

            try
            {
                var categoriesEntity = await _postgresDbContext
                    .Categories
                    .AsNoTracking()
                    .Where(c => c.Estado == true)
                    .OrderBy(c => c.Id)
                    .ToListAsync(cancellationToken);

                if (categoriesEntity.Count == 0)
                {
                    _logger.LogWarning("No se encontraron categorías activas. Cantidad={Cantidad}", 0);
                    return new List<CategoryResponse>();
                }

                var categoryResponse = categoriesEntity.Select(e => new CategoryResponse(
                    e.Id,
                    e.Nombre,
                    e.Descripcion ?? string.Empty,
                    e.Estado
                )).ToList();

                _logger.LogInformation("Categorías activas obtenidas correctamente. Cantidad={Cantidad}", categoryResponse.Count);
                return categoryResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener la lista de categorías activas.");
                throw;
            }
        }
    }
}
