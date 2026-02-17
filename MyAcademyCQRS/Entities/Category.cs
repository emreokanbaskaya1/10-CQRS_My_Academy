namespace MyAcademyCQRS.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;

    public List<Product> Products { get; set; } = new();
}
