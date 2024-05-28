using Data.Models.Address;

namespace Data.Models.ViewModels
{
    public class GetInvoice
    {
        public int InvoiceId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }   
        public string PhoneNumber { get; set; } 
        public AddressModel Address { get; set; }         
        public List<InvoiceProduct>? products { get; set; }    
        public DateTime OrderDate {  get; set; } 
        public DateTime OrderTime { get; set;}
        public decimal OrderAmount {  get; set; } 

    }
}
