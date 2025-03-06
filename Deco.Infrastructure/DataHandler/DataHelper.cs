using Deco.Infrastructure.Entities;
using Deco.Shared;
using Deco.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Deco.Infrastructure.DataHandler
{
    public class DataHelper : IDataHelper
    {
        public void SaveData(List<InvoiceHeader> headers, List<InvoiceLine> invoiceLines)
        {
            using (var context = new InvoiceContext())
            {
              
                try
                {
                    context.Database.EnsureCreated();
                    foreach (var item in headers)
                    {
                        var entity = context.InvoiceHeaders.Where(x => x.InvoiceNumber == item.InvoiceNumber).FirstOrDefault();
                        if (entity is null )
                        {
                            context.InvoiceHeaders.AddRange(item);
                        }

                    }
                    foreach (var item in invoiceLines)
                    {
                        var entity = context.InvoiceLines.Where(x => x.InvoiceNumber == item.InvoiceNumber && x.Description == item.Description && x.Quantity== item.Quantity && x.UnitSellingPriceExVAT == item.UnitSellingPriceExVAT).FirstOrDefault();
                        if (entity is null)
                        {
                            context.InvoiceLines.AddRange(item);
                        }
                    }

                    context.Database.EnsureCreated();
                    context.SaveChanges();
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                }
            }
        }

        public async Task<string> RetrieveData()
        {
            using (var context = new InvoiceContext())
            {
                var outputstring = new StringBuilder();
                try
                {
                    var Headers = context.InvoiceHeaders
                    .Include(i => i.InvoiceLines);
                    foreach (var h in Headers)
                    {
                        var data = new StringBuilder();                     
                        var headerTotal = h.InvoiceTotal;
                        var linetotal = h.InvoiceLines.Where(x => x.InvoiceNumber == h.InvoiceNumber).ToList();
                        float invoiceTotal = 0;
                            foreach (var l in linetotal)
                        {
                            invoiceTotal += l.UnitSellingPriceExVAT* l.Quantity;
                        }
                        if (headerTotal == invoiceTotal)
                        {
                            data.AppendLine($"Invoice #{ h.InvoiceNumber} :header total of {headerTotal} matched calculated total of: {invoiceTotal}");
                        }
                        else
                        {
                            data.AppendLine($"Invoice #{h.InvoiceNumber} :header total of {headerTotal} did not match calculated total of : {invoiceTotal}");
                        }
                        outputstring.Append(data);
                    }
                    return outputstring.ToString();
                }
                catch (Exception ex)
                {
                    return ex.Message;                
                }
            }
        }
    }
}
