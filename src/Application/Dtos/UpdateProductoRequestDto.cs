namespace Application.Dtos
{
    public class UpdateProductoRequestDto
    {
        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public float Precio { get; set; }
    }
}
