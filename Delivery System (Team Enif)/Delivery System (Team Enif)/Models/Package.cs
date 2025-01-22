﻿public class Package
{
    public int Id { get; set; }
    public string SenderName { get; set; }
    public string RecipientName { get; set; }
    public string SenderAddress { get; set; }
    public string RecipientAddress { get; set; }
    public double Weight { get; set; }
    public string Size { get; set; }
    public string DeliveryType { get; set; } // Standard, Express, etc.
    public string Status { get; set; } // In Transit, Delivered, etc.
    public DateTime DeliveryDate { get; set; }
}
