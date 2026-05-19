namespace racebookApi.Models
{
    public class Session
    {
        public Guid SessionId { get; set; }
        public required List<User> Users { get; set; }
        public required string Name { get; set; }
        public required List<PlayerStatsSnapshot> StartPlayerStatsSnapshots { get; set; }
        public required List<PlayerStatsSnapshot> EndPlayerStatsSnapshots { get; set; }
    }
}