using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Domain.Entity;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Command.CreateCategory
{
    public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, CategoryResponse>
    {
        private readonly PostgresDbContext _dbContext;
        private readonly ILogger<CreateCategoryHandler> _logger;

        public CreateCategoryHandler(PostgresDbContext dbContext, ILogger<CreateCategoryHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<CategoryResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Ejecutando caso de uso: crear categoría. Nombre={Nombre}",
                request.Request?.Nombre
            );

            try
            {
                var entity = new Category
                {
                    Nombre = request.Request.Nombre,
                    Descripcion = request.Request.Descripcion ?? string.Empty,
                    Estado = true
                };

                _dbContext.Categories.Add(entity);
                await _dbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Categoría creada y persistida correctamente. CategoryId={CategoryId} Nombre={Nombre}",
                    entity.Id,
                    entity.Nombre
                );

                return new CategoryResponse(
                    entity.Id,
                    entity.Nombre,
                    entity.Descripcion ?? string.Empty,
                    entity.Estado
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error inesperado al crear categoría. Nombre={Nombre}",
                    request.Request?.Nombre
                );
                throw;
            }
        }
    }
}
