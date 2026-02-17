using MediatR;

namespace MyAcademyCQRS.DesignPatterns.Mediator.Commands;

public class CreateTestimonialCommand : IRequest<bool>
{
    public string CustomerName { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? CustomerImageUrl { get; set; }
}
