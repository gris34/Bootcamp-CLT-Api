using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Query.GetCategoryById
{
    public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryResponse?>
    {
        private readonly PostgresDbContext _dbContext;
        private readonly ILogger<GetCategoryByIdHandler> _logger;

        public GetCategoryByIdHandler(PostgresDbContext dbContext, ILogger<GetCategoryByIdHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<CategoryResponse?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Ejecutando caso de uso: obtener categoría por id. CategoryId={CategoryId}",
                request.Id
            );

            try
            {
                var category = await _dbContext.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (category == null)
                {
                    _logger.LogWarning("Categoría no encontrada en la base de datos. CategoryId={CategoryId}", request.Id);
                    return null;
                }

                _logger.LogInformation("Categoría obtenida correctamente. CategoryId={CategoryId}", request.Id);

                return new CategoryResponse(
                    category.Id,
                    category.Nombre,
                    category.Descripcion ?? string.Empty,
                    category.Estado
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al obtener categoría por id. CategoryId={CategoryId}", request.Id);
                throw;
            }
        }
    }
}
