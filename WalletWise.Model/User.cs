namespace WalletWise.Model
{
    public class User
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordSalt { get; set; }
        public string Password { get; set; }
    }
}
