namespace WalletWise.Model.DTO.Balance
{
    public class ItemReqDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public long CategoryId { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
