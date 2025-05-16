using System.Collections;

namespace OrderService.Data.Models;

public class Order
{
    public int Id { get; set; }
    public decimal TotalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.New;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public List<string>? CallRecord { get; set; }
    public bool IsDeleted { get; set; } = false;
    
    public int? UserId { get; set; }
    public int? ClientId { get; set; }
    
    // public List<CallRecording> CallRecordings { get; set; } = new();
    //
    //
    // public ICollection<ClientOrder> ClientOrders { get; set; } = new List<ClientOrder>();
    //
    // public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    //
    // public ICollection<UserOrders> UserOrders { get; set; } = new List<UserOrders>();
}

public enum OrderStatus
{
    New,
    Processing,
    Completed
}