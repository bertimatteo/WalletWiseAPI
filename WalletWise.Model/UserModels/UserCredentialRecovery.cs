namespace WalletWise.Model.UserModels
{
    public class UserCredentialRecovery : EntityBase
    {
        public User User { get; set; }
        public string Question { get; set; }
        public string ResponseSalt { get; set; }
        public string Response { get; set; }
    }
}
