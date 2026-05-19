namespace racebookApi.Models
{
    public class PlayerStatsSnapshot
    {
        public Guid SnapshotId { get; set; }
        public required int TotalFans { get; set; }
        public required long TotalRaceTimeMilleconds { get; set; }
        public required int DriverScore { get; set; }
        public required int RaceStarts { get; set; }
        public required int RaceWins {  get; set; }
        public required int RacePodiums { get; set; }
        public required int PowerUpUses {  get; set; }
        public required int intPowerUpHits { get; set; }
        public required DateTime Date {  get; set; }
    }
}