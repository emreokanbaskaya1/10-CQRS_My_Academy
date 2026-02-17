using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.DesignPatterns.Observer;

public interface IPromotionObserver
{
    void OnPromotionCreated(Promotion promotion);
    void OnPromotionUpdated(Promotion promotion);
}
