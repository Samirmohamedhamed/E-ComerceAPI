namespace E_CommerceAPI.Models
{
    public enum OrderStatus
    {
        pending,
        Inprocessing,
        Shiped,
        InWay,
        Completed,
        Canceled
    }
    public enum TransactionStatus
    {
        Completed,
        Canceled,
        Refunded,
        pending
    }
    public class Order
    {
        //Order
        public int Id { get; set; }
        public string ApplicationId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public decimal TotalPrice { get; set; }

        //Transaction

        public TransactionStatus TransactionStatus { get; set; }
        public string? SesstionId { get; set; }
        public string? PaymentId { get; set; }

        //Carrier
        public DateTime ShippedDate { get; set; }
        public string? Carrier { get; set; }
        public string? CarrierId { get; set; }




    }
}
