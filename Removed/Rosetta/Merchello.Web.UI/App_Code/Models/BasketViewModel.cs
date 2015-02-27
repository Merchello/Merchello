namespace Models
{
    // simple basket view model
    public class BasketViewModel
    {
        public decimal TotalPrice { get; set; }
     
        public BasketViewLineItem[] Items { get; set; } 
    }
}