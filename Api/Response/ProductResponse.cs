namespace ApiBootcampCLT.Api.Response;

public record ProductResponse
(
    int Id,
    string Codigo,
    string Nombre,
    string Descripcion,
    decimal Precio,
    bool Activo,
    int CategoriaId,
    DateTime FechaCreacion,
    DateTime? FechaActualizacion,
    int CantidadStock
);
