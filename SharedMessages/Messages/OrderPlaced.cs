namespace SharedMessages.Messages
{
    public sealed record OrderPlaced(Guid OrderId, int Quantity);
}