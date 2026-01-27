namespace ApiBootcampCLT.Api.Request;

public record UpdateProductRequest
{
    public string Codigo { get; init; } = default!;
    public string Nombre { get; init; } = default!;
    public string? Descripcion { get; init; }
    public decimal Precio { get; init; }
    public bool Activo { get; init; }
    public int CategoriaId { get; init; }
    public DateTime? FechaActualizacion { get; init; }
    public int CantidadStock { get; init; }

}