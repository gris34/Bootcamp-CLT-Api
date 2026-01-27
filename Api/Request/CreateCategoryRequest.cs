namespace ApiBootcampCLT.Api.Request
{
    public class CreateCategoryRequest
    {
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; } 
    }
}
