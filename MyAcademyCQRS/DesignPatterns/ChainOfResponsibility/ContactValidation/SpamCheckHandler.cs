namespace MyAcademyCQRS.DesignPatterns.ChainOfResponsibility.ContactValidation;

public class SpamCheckHandler : ContactValidationHandler
{
    public override Task<(bool Success, string Message)> Handle(ContactValidationContext context)
    {
        if (string.IsNullOrWhiteSpace(context.FullName))
            return Task.FromResult((false, "Ad Soyad alanı boş olamaz."));

        if (string.IsNullOrWhiteSpace(context.Subject))
            return Task.FromResult((false, "Konu alanı boş olamaz."));

        if (string.IsNullOrWhiteSpace(context.Message))
            return Task.FromResult((false, "Mesaj alanı boş olamaz."));

        return base.Handle(context);
    }
}
