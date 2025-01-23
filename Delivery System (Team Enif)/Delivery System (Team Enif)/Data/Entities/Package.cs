namespace Delivery_System__Team_Enif_.Data.Entities
{
    public class Package : BaseEntity
    {
        public string Product {  get; set; }

        public double Weight { get; set; }

        public string Address { get; set; }

        public double ProductPrice { get; set; }
    }
}
