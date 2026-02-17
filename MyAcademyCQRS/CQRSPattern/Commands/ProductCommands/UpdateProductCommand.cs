namespace MyAcademyCQRS.CQRSPattern.Commands.ProductCommands
{
    public record UpdateProductCommand(int Id,
                                       string Name,
                                       string Description,
                                       decimal Price,
                                       string ImageUrl,
                                       int CategoryId)
    {
    }
}
