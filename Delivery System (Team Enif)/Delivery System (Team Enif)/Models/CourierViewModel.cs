public class CourierViewModel
{
    public string CourierId { get; set; }
    public string Name { get; set; }
    public OfficeViewModel Office { get; set; }
    public List<Delivery> Deliveries { get; set; }
}
