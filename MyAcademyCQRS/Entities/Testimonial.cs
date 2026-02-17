namespace MyAcademyCQRS.Entities;

public class Testimonial
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? CustomerImageUrl { get; set; }
    public bool IsApproved { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
