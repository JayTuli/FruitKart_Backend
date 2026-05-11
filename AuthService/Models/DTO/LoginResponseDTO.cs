namespace AccountService.Models.DTO
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string? Token { get; set; }
    }
}
