namespace ApiBootcampCLT.Domain.Entity
{
    public class Category
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }    
        public bool Estado { get; set; }

        public ICollection<Product> Productos { get; set; } = [];
    }
}
