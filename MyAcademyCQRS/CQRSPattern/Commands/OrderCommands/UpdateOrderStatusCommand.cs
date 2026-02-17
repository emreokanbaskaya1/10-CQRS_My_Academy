using MyAcademyCQRS.Entities.Enums;

namespace MyAcademyCQRS.CQRSPattern.Commands.OrderCommands;

public record UpdateOrderStatusCommand(int OrderId, OrderStatus NewStatus);
