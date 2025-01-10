namespace SharedMessages.Messages
{
    public sealed record OrderPlaced(Guid OrderId, int Quantity);
    public sealed record InventoryReserved(Guid OrderId);
    public record PaymentCompleted(Guid OrderId);
}