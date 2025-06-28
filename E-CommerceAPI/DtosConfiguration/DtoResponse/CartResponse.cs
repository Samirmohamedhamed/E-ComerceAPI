namespace E_CommerceAPI.DtosConfiguration.DtoResponse
{
    public class CartResponse
    {
        public int ProductId {  get; set; }
        public int Count {  get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string ProductMainImg { get; set; } = string.Empty;

    }
}
