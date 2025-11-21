namespace TCG_COMPANION.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        public string? DisplayName { get; set; }
        public string? Bio { get; set; }
        public string? ProfileImage { get; set; }
    }
}