namespace MyAcademyCQRS.Entities;

public class PhotoGallery
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? CategoryTag { get; set; }
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
}
