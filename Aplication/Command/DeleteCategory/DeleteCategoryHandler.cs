using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Command.DeleteCategory
{
    public class DeleteCategoryHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly PostgresDbContext _dbContext;
        private readonly ILogger<DeleteCategoryHandler> _logger;

        public DeleteCategoryHandler(PostgresDbContext dbContext, ILogger<DeleteCategoryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Ejecutando caso de uso: eliminar (lógico) categoría. CategoryId={CategoryId}",
                request.Id
            );

            try
            {
                var entity = await _dbContext.Categories
                    .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

                if (entity == null)
                {
                    _logger.LogWarning("Categoría no encontrada para eliminación lógica. CategoryId={CategoryId}", request.Id);
                    return false;
                }

                if (entity.Estado == false)
                {
                    _logger.LogInformation("La categoría ya se encontraba eliminada lógicamente. CategoryId={CategoryId}", request.Id);
                    return true;
                }

                entity.Estado = false;
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation("Categoría eliminada lógicamente correctamente. CategoryId={CategoryId}", request.Id);
                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Error de base de datos al eliminar (lógico) categoría. CategoryId={CategoryId}", request.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error inesperado al eliminar (lógico) categoría. CategoryId={CategoryId}", request.Id);
                throw;
            }
        }
    }
}
