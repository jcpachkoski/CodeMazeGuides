namespace SharedModels
{
    // This is noun-verb past tense which is an Event.
    // This gets published.
    public interface OrderCreated
    {
        int Id { get; set; }
        string ProductName { get; set; }
        decimal Price { get; set; }
        int Quantity { get; set; }
    }   
}