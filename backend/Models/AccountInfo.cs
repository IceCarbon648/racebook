namespace Models
{
    public class AccountInfo
    {
        public required Guid Uid { get; set; }
        public required string Username { get; set; }
        public string? AmaxUsername { get; set; }
        public required string PasswordHash { get; set; }
    }
}