using Microsoft.EntityFrameworkCore;

namespace E_CommerceAPI.Models
{
    [PrimaryKey(nameof(ProductId), nameof(OrderId))]

    public class OrderItems
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int OrderId {  get; set; }
        public Order Order { get; set; }
        public decimal ItemPrice {  get; set; }
        public int Count {  get; set; }
    }
}
