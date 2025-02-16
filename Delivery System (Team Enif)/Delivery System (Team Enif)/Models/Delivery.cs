public class Delivery
{
    public int Id { get; set; }
    public int PackageId { get; set; }
    public DateTime PickupTime { get; set; }
    public DateTime DeliveryTime { get; set; }
    public string DeliveryStatus { get; set; } // "In Transit", "Completed"
}
