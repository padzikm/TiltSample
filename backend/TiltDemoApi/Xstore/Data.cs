using Newtonsoft.Json;

namespace TiltDemoApi.Xstore;

public class Product
{
    [JsonIgnore]
    public int Id { get; set; }
    
    public string Name { get; set; }
}

public class User
{
    [JsonIgnore]
    public int Id { get; set; }
    
    public string Name { get; set; }
}

public class OrderItem
{
    [JsonIgnore]
    public int OrderId { get; set; }
    
    public int ProductId { get; set; }
    
    public int Quantity { get; set; }
    
    public decimal UnitPrice { get; set; }
}

public class PlacedOrder
{
    [JsonIgnore]
    public int OrderId { get; set; }
    
    public int UserId { get; set; }
    
    public DateTime PurchaseDate { get; set; }
}

public class CanceledOrder
{
    [JsonIgnore]
    public int OrderId { get; set; }
    
    public DateTime CancelDate { get; set; }
    
    public string Reason { get; set; }
}