using MediatR;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.DesignPatterns.Mediator.Commands;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.DesignPatterns.Mediator.Handlers;

public class CreateTestimonialHandler : IRequestHandler<CreateTestimonialCommand, bool>
{
    private readonly AppDbContext _context;

    public CreateTestimonialHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(CreateTestimonialCommand request, CancellationToken cancellationToken)
    {
        var testimonial = new Testimonial
        {
            CustomerName = request.CustomerName,
            Comment = request.Comment,
            Rating = request.Rating,
            CustomerImageUrl = request.CustomerImageUrl,
            IsApproved = false,
            CreatedDate = DateTime.UtcNow
        };

        await _context.Testimonials.AddAsync(testimonial, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
