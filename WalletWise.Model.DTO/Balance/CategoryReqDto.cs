namespace WalletWise.Model.DTO.Balance
{
    public class CategoryReqDto
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Description { get; set; }
        public int Type { get; set; }
        public string Icon { get; set; }
        public string ColorBackground { get; set; }
        public bool IsDelete { get; set; }
    }
}
