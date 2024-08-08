namespace WalletWise.Model.User
{
    public class User : EntityBase
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordSalt { get; set; }
        public string Password { get; set; }
    }
}
