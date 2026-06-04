namespace Domain.Entities
{
    public class Producto
    {
        public Guid Id { get; set; }

        public Guid TenantId { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public float Precio { get; set; }
    }
}
