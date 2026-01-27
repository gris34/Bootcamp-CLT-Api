namespace ApiBootcampCLT.Api.Request
{
    public class UpdateCategoryRequest
    {
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; } 
        public bool Estado { get; set; }
    }
}
