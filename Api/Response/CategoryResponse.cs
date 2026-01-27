namespace ApiBootcampCLT.Api.Response
{
    public record CategoryResponse
    (
        int Id,
        string Nombre,
        string Descripcion,
        bool Estado
    );
    
}
