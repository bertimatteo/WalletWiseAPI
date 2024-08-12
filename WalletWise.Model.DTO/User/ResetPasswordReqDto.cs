namespace WalletWise.Model.DTO.User
{
    public class ResetPasswordReqDto
    {
        public string Username { get; set; }
        public string Response { get; set; }
        public string NewPassword { get; set; }
    }
}
