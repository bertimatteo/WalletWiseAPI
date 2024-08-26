using WalletWise.Model.UserModels;

namespace WalletWise.Model.BalanceModels
{
    public class Item
    {
        public long Id { get; set; }
        public User User { get; set; }
        public Category Category { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
    }
}
