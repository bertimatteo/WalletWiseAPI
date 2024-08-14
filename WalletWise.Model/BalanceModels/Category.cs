using WalletWise.Model.UserModels;

namespace WalletWise.Model.BalanceModels
{
    public class Category : EntityWithDescription
    {
        public User User { get; set; }
        public CategoryType CategoryType { get; set; }
        public string Icon { get; set; }
        public string ColorBackground { get; set; }
        public bool IsDeleted { get; set; }
    }
}
