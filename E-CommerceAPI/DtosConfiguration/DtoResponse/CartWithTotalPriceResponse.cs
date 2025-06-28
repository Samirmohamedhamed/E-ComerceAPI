namespace E_CommerceAPI.DtosConfiguration.DtoResponse
{
    public class CartWithTotalPriceResponse
    {
        public IEnumerable<CartResponse> cartResponse { get; set; } = new List<CartResponse>();
        public decimal totalPrice { get; set; }
    }
}
