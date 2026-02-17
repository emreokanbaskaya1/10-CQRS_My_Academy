namespace MyAcademyCQRS.Entities;

public class ServiceStep
{
    public int Id { get; set; }
    public int StepNumber { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public string? ImageUrl { get; set; }
}
