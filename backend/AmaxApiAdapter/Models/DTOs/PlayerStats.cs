namespace AmaxApiAdapter.Models.DTOs
{
    public class PlayerStats
    {
        public int TotalFans { get; set; }
        public int DriverScore { get; set; }
        public long RaceTime { get; set; }
        public int RaceStarts { get; set; }
        public int Wins { get; set; }
        public int PodiumFinishes { get; set; }
        public int PowerUpUses { get; set; }
        public int PowerUpHits { get; set; }
    }
}