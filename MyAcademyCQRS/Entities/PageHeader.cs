namespace MyAcademyCQRS.Entities;

public class PageHeader
{
    public int Id { get; set; }
    
    /// <summary>
    /// Shop, Gallery, Contact, Testimonial, Cart
    /// </summary>
    public string PageName { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    
    public string? Subtitle { get; set; }
    
    public string BackgroundImageUrl { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
}
