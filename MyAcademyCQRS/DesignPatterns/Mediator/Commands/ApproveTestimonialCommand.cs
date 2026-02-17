using MediatR;

namespace MyAcademyCQRS.DesignPatterns.Mediator.Commands;

public class ApproveTestimonialCommand : IRequest<bool>
{
    public int TestimonialId { get; set; }
    public bool IsApproved { get; set; }
}
