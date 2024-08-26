namespace WebApplication1.Models
{
    public class Player
    {
        public int PlayerID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Country { get; set; }

        public int CurrentWorldRanking { get; set; }

        public int TotalMatchesPlayed { get; set; }
    }
}
