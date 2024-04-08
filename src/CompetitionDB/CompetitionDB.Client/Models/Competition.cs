namespace CompetitionDB.Client.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public List<Contestant> Contestants { get; set; }
    }
}