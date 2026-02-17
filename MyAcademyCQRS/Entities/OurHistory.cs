namespace MyAcademyCQRS.Entities;

public class OurHistory
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public int SinceYear { get; set; }
}
