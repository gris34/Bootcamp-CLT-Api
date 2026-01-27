using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Command.UpdateCategory
{
    public class UpdateCategoryHandler : IRequestHandler<UpdateCategoryCommand, CategoryResponse?>
    {
        private readonly PostgresDbContext _dbContext;
        private readonly ILogger<UpdateCategoryHandler> _logger;

        public UpdateCategoryHandler(PostgresDbContext dbContext, ILogger<UpdateCategoryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<CategoryResponse?> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Ejecutando caso de uso: actualizar categoría. CategoryId={CategoryId}",
                request.Id
            );

            try
            {
                var entity = await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning(
                        "Categoría no encontrada para actualización. CategoryId={CategoryId}",
                        request.Id
                    );
                    return null;
                }

                entity.Nombre = request.Request.Nombre;
                entity.Descripcion = request.Request.Descripcion;

                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Categoría actualizada correctamente. CategoryId={CategoryId}",
                    entity.Id
                );

                return new CategoryResponse(
                    entity.Id,
                    entity.Nombre,
                    entity.Descripcion ?? string.Empty,
                    entity.Estado
                );
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error de base de datos al actualizar categoría. CategoryId={CategoryId}",
                    request.Id
                );
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error inesperado al actualizar categoría. CategoryId={CategoryId}",
                    request.Id
                );
                throw;
            }
        }
    }
}
