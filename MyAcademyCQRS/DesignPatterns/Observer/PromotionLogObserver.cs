using MyAcademyCQRS.Entities;
using Serilog;

namespace MyAcademyCQRS.DesignPatterns.Observer;

public class PromotionLogObserver : IPromotionObserver
{
    public void OnPromotionCreated(Promotion promotion)
    {
        Log.ForContext("Area", "Promotion")
           .Information("Yeni kampanya oluşturuldu: {Title} | İndirim: %{Discount} | Kod: {PromoCode}",
               promotion.Title, promotion.DiscountPercentage, promotion.PromoCode ?? "Yok");
    }

    public void OnPromotionUpdated(Promotion promotion)
    {
        Log.ForContext("Area", "Promotion")
           .Information("Kampanya güncellendi: {Title} | Aktif: {IsActive} | Bitiş: {EndDate}",
               promotion.Title, promotion.IsActive, promotion.EndDate.ToString("dd.MM.yyyy"));
    }
}
