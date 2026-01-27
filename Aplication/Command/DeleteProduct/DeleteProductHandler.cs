using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApiBootcampCLT.Aplication.Command.DeleteProduct
{
    public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, bool>
    {
        private readonly PostgresDbContext _postgresDbContext;
        private readonly ILogger<DeleteProductHandler> _logger;

        public DeleteProductHandler(PostgresDbContext postgresDbContext, ILogger<DeleteProductHandler> logger)
        {
            _postgresDbContext = postgresDbContext;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Ejecutando caso de uso: eliminar producto. ProductId={ProductId}",
                request.Id
            );

            try
            {
                var productEntity = await _postgresDbContext.Products
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (productEntity == null)
                {
                    _logger.LogWarning(
                        "Producto no encontrado para eliminación. ProductId={ProductId}",
                        request.Id
                    );
                    return false;
                }

                _postgresDbContext.Products.Remove(productEntity);
                await _postgresDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Producto eliminado correctamente. ProductId={ProductId}",
                    request.Id
                );

                return true;
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(
                    ex,
                    "Error de base de datos al eliminar producto. ProductId={ProductId}",
                    request.Id
                );
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error inesperado al eliminar producto. ProductId={ProductId}",
                    request.Id
                );
                throw;
            }
        }
    }
}
