// See https://aka.ms/new-console-template for more information
using System.IO;
using System.Reflection;
using Deco.Shared.Models;
using Deco.Shared;
using System.Diagnostics.Metrics;
using System.Numerics;
using Deco.Infrastructure.DataHandler;
using System.Diagnostics;

internal class Program
{
 private readonly IDataHelper? _dataHelper;

    static  void Main(string[] args)
    {
        Console.WriteLine("Invoice Loading......");
        ProcessInvoice();
      
    }

    private async static Task<Result> ProcessInvoice()
    {
        try
        {
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Data\data.csv");
            var invoiceitems = new List<InvoiceLine>();
            var headers = new List<InvoiceHeader>();

            using (var reader = new StreamReader(path))
            {
                var invoiceNum = string.Empty;
                while (!reader!.EndOfStream)
                {
                    var items = reader.ReadLine();
                    if (items == null) continue;
                    string[] values = items.Split(',');
                    if (items is not null && !string.IsNullOrEmpty(items))
                    {
                        try
                        {
                            var headeritems = new InvoiceHeader();
                            if (invoiceNum != values[0])
                            {
                                invoiceNum = values[0];
                                 headeritems = new InvoiceHeader
                                {
                                    InvoiceDate = DateTime.Parse(values[1].ToString()),
                                    InvoiceNumber = values[0],
                                    Address = values[2],
                                    InvoiceTotal = float.Parse(values[3])
                                };
                                headers.Add(headeritems);
                            }
                            var lineitems = new InvoiceLine
                            {
                                InvoiceNumber = values[0],
                                Quantity = float.Parse(values[5]),
                                UnitSellingPriceExVAT = float.Parse(values[6]),
                                Description = values[4],
                                InvoiceHeader = headeritems

                            };
                            
                            invoiceitems.Add(lineitems);

                        }
                        catch (Exception e)
                        {
                            var t = e.Message;
                        }

                  
                    }
                    else
                    {
                        Console.WriteLine("File does does not contain any contents");
                    } 
                    
                }
            }

            foreach (var item in headers)
            {
                var itemlines = invoiceitems.Where(x => x.InvoiceNumber == item.InvoiceNumber);
 Console.WriteLine($"Imported invoice #{item.InvoiceNumber} with #{itemlines.Sum(x=> x.Quantity)} items");
            }
       

                IDataHelper dataHelper = new DataHelper();
                 dataHelper.SaveData(headers,invoiceitems);
                Console.WriteLine("saved to DB");

                //Read Db data and display 
                //-After the import is complete check that the sum of `InvoiceLines.Quantity* InvoiceLines.UnitSellingPriceExVAT` 
                //balances back to the sum of all `InvoiceHeader.InvoiceTotal`. I.e. 21,860.71
                var outome = await dataHelper.RetrieveData();
                Console.WriteLine(outome.ToString());

                //- Print a message showing the outcome of this check.
                Console.WriteLine("Invoice Processed");
                return new Success();

            
        }
        catch (IOException ex)
        {
            //return ex.Message;
            Console.WriteLine($"Error reading file {ex.Message}");
            return new Failure();
        }
    }
}