namespace WebApplication5.Models;

public class OrderProcessed
{
    public Guid OrderId { get; set; }
    public Guid ProcessingId { get; set; }
}