namespace OrderPaymentSystem.Domain.Dto.Product
{
    public class UpdateProductDto
    {
        public int Id { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public decimal Cost { get; set; }
    }
}
