namespace MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.ContactValidation;

public abstract class ContactValidationHandler
{
    private ContactValidationHandler? _next;

    public ContactValidationHandler SetNext(ContactValidationHandler next)
    {
        _next = next;
        return next;
    }

    public virtual Task<(bool Success, string Message)> Handle(ContactValidationContext context)
    {
        if (_next != null)
            return _next.Handle(context);

        return Task.FromResult((true, "Mesaj validasyonu başarılı."));
    }
}

public class ContactValidationContext
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
