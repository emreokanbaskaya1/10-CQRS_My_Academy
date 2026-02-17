using MediatR;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Context;
using MyAcademyCQRS.DesignPatterns.Mediator.Commands;

namespace MyAcademyCQRS.DesignPatterns.Mediator.Handlers;

public class ApproveTestimonialHandler : IRequestHandler<ApproveTestimonialCommand, bool>
{
    private readonly AppDbContext _context;

    public ApproveTestimonialHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ApproveTestimonialCommand request, CancellationToken cancellationToken)
    {
        var testimonial = await _context.Testimonials
            .FirstOrDefaultAsync(t => t.Id == request.TestimonialId, cancellationToken);

        if (testimonial == null) return false;

        testimonial.IsApproved = request.IsApproved;
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
