namespace MyAcademyCQRS.Entities;

public class AppLog
{
    public int Id { get; set; }
    public string? Message { get; set; }
    public string? Level { get; set; }
    public DateTime TimeStamp { get; set; }
    public string? Exception { get; set; }
    public string? Properties { get; set; }
    public string? Area { get; set; }
}
