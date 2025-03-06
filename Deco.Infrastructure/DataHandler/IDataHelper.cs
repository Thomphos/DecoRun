using Deco.Infrastructure;
using Deco.Shared;

namespace Deco.Infrastructure.DataHandler
{
    public interface IDataHelper
    {
         void SaveData(List<InvoiceHeader> headers, List<InvoiceLine> invoiceLines);
         Task<string> RetrieveData();
    }
}
