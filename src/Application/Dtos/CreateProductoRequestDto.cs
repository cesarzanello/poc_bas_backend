namespace Application.Dtos
{
    public class CreateProductoRequestDto
    {
        public Guid TenantId { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public float Precio { get; set; }
    }
}
