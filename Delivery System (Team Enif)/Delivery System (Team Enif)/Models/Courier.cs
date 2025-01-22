public class Courier
{
    public string CourierId { get; set; }
    public string Name { get; set; }
    public string OfficeLocation { get; set; }
    public List<Delivery> Deliveries { get; set; }
}
