namespace Deco.Shared
{
    public  class InvoiceHeader
    {
        public int InvoiceId {get;set;}
    public string? InvoiceNumber { get; set; } 
    public DateTime? InvoiceDate {  get; set; }
	public string Address { get; set; } = string.Empty;
	public float InvoiceTotal {  get; set; }
        public ICollection<InvoiceLine> InvoiceLines { get; set; } = new List<InvoiceLine>();
    }
}
