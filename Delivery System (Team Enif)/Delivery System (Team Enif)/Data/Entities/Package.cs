namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Package : BaseEntity
    {
        public string SenderName { get; set; }
        public string RecipientName { get; set; }
        public string SenderAddress { get; set; }
        public string RecipientAddress { get; set; }
        public double Weight { get; set; }
        public string Size { get; set; }
        public int DeliveryOptionId { get; set; }
        public DeliveryOption DeliveryOption { get; set; }
        public int DeliveryTypeId { get; set; }
        public DeliveryType DeliveryType { get; set; }
        public int DeliveryStatusId { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; } 
        public DateTime DeliveryDate { get; set; } 
    }
}