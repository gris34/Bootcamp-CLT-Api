using ApiBootcampCLT.Api.Response;
using ApiBootcampCLT.Aplication.Command.CreateProduct;
using ApiBootcampCLT.Domain.Entity;
using ApiBootcampCLT.Infraestructure.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace api.bootcamp.clt.Aplication.Command.CreateProduct
{
    public class CreateProductHandler : IRequestHandler<CreateProductCommand, ProductResponse>
    {
        private readonly PostgresDbContext _postgresDbContext;
        private readonly ILogger<CreateProductHandler> _logger;

        public CreateProductHandler(PostgresDbContext postgresDbContext, ILogger<CreateProductHandler> logger)
        {
            _postgresDbContext = postgresDbContext;
            _logger = logger;
        }

        public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Ejecutando caso de uso: crear producto. Codigo={Codigo} Nombre={Nombre} CategoriaId={CategoriaId}",
                request.ProductRequest?.Codigo,
                request.ProductRequest?.Nombre,
                request.ProductRequest?.CategoriaId
            );

            try
            {
                var productEntity = new Product
                {
                    Codigo = request.ProductRequest.Codigo,
                    Nombre = request.ProductRequest.Nombre,
                    Descripcion = request.ProductRequest.Descripcion,
                    Precio = request.ProductRequest.Precio,
                    Activo = request.ProductRequest.Activo,
                    CategoriaId = request.ProductRequest.CategoriaId,
                    FechaCreacion = DateTime.UtcNow,
                    CantidadStock = 0
                };

                _postgresDbContext.Products.Add(productEntity);

                await _postgresDbContext.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Producto creado y persistido correctamente. ProductId={ProductId} Codigo={Codigo}",
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
                // Muy común si el índice único de codigo se viola
                _logger.LogError(
                    ex,
                    "Error de base de datos al crear producto (posible código duplicado). Codigo={Codigo}",
                    request.ProductRequest?.Codigo
                );
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error inesperado al crear producto. Codigo={Codigo}",
                    request.ProductRequest?.Codigo
                );
                throw;
            }
        }
    }
}
