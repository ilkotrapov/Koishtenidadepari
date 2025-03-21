﻿namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Package : BaseEntity
    {
        public int Id { get; set; }
        public required string SenderName { get; set; }
        public required string RecipientName { get; set; }
        public required string SenderAddress { get; set; }
        public required string RecipientAddress { get; set; }
        public required decimal Length { get; set; }
        public required decimal Width { get; set; }
        public required decimal Hight { get; set; }
        public required decimal Weight { get; set; }
        public int DeliveryOptionId { get; set; }
        public required DeliveryOption DeliveryOption { get; set; }
        public int DeliveryTypeId { get; set; }
        public required DeliveryType DeliveryType { get; set; }
        public int DeliveryStatusId { get; set; }
        public DeliveryStatus DeliveryStatus { get; set; }
        public required DateTime DeliveryDate { get; set; } 
    }
}