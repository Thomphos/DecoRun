using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deco.Shared
{
    public  class InvoiceLine
    {
        public int LineId { get; set; }
        public string ? InvoiceNumber { get; set; }
        public string? Description { get; set; }
	public float Quantity  {get; set; }
        public float UnitSellingPriceExVAT { get; set; }

        public InvoiceHeader InvoiceHeader { get; set; }
    }
}
