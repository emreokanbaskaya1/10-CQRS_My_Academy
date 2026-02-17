using System.Text.RegularExpressions;

namespace MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.ContactValidation;

public class EmailValidationHandler : ContactValidationHandler
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public override Task<(bool Success, string Message)> Handle(ContactValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(context.Email))
            return Task.FromResult((false, "E-posta alanı boş olamaz."));

        if (!EmailRegex.IsMatch(context.Email))
            return Task.FromResult((false, "Geçersiz e-posta formatı."));

        return base.Handle(context);
    }
}
