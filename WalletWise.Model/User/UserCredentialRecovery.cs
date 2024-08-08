namespace WalletWise.Model.User
{
    public class UserCredentialRecovery : EntityBase
    {
        public User User { get; set; }
        public string Question { get; set; }
        public string Response { get; set; }
    }
}
