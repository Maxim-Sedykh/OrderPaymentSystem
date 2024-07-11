using OrderPaymentSystem.Domain.Enum;

namespace OrderPaymentSystem.Domain.Dto.Payment
{
    public class PaymentDto
    {
        public long Id { get; set; }    

        public long BasketId { get; set; }    

        public decimal AmountOfPayment { get; set; }    

        public PaymentMethod PaymentMethod { get; set; }  
        
        public decimal CashChange { get; set; }  
        
        public string Street { get; set; }    

        public string City { get; set; }    

        public string Country { get; set; }   
        
        public string Zipcode { get; set; }   
        
        public string CreatedAt { get; set; }    
    }
}
