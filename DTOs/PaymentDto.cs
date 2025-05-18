namespace Api_entradas.DTOs
{
    public class PaymentDto
    {
        public decimal Price { get; set; }
        public string PayerEmail { get; set; } = "";
    }
}
