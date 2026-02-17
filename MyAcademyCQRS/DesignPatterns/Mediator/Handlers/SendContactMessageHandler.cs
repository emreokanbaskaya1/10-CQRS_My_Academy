using MediatR;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.DesignPatterns.Mediator.Commands;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.DesignPatterns.Mediator.Handlers;

public class SendContactMessageHandler : IRequestHandler<SendContactMessageCommand, bool>
{
    private readonly AppDbContext _context;

    public SendContactMessageHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(SendContactMessageCommand request, CancellationToken cancellationToken)
    {
        var contact = new Contact
        {
            FullName = request.FullName,
            Email = request.Email,
            Subject = request.Subject,
            Message = request.Message,
            IsRead = false,
            CreatedDate = DateTime.UtcNow
        };

        await _context.Contacts.AddAsync(contact, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
