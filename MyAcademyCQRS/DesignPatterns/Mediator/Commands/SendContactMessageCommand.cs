using MediatR;

namespace MyAcademyCQRS.DesignPatterns.Mediator.Commands;

public class SendContactMessageCommand : IRequest<bool>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
