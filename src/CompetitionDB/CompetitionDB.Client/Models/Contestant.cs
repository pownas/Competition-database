namespace CompetitionDB.Client.Models;

public class Contestant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Scored { get; set; }

    public int CompetitionId { get; set; }
    public Competition Competition { get; set; }
}
