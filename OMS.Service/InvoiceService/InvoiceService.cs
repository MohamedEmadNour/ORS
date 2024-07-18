using OMS.Data.Entites;
using OMS.Repositores.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Document = iText.Layout.Document;
using iText.Layout.Borders;
using iText.Layout.Properties;
using static System.Net.Mime.MediaTypeNames;

namespace OMS.Service.InvoiceService
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<string> GenerateInvoiceAsync(Order order)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "invoices");

            // Create directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string invoicePath = Path.Combine(directoryPath, $"invoice_{order.Customer.Name + " order " + order.OrderId}.pdf");

            try
            {
                using (FileStream fs = new FileStream(invoicePath, FileMode.Create, FileAccess.Write, FileShare.None))
                using (PdfWriter writer = new PdfWriter(fs))
                {
                    PdfDocument pdf = new PdfDocument(writer);
                    Document document = new Document(pdf);

                    Paragraph title = new Paragraph("Invoice")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(20)
                        .SetBold();

                    document.Add(title);


                    Table orderDetailsTable = new Table(2)
                        .UseAllAvailableWidth()
                        .SetMarginTop(20)
                        .SetBorder(Border.NO_BORDER);

                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph("Order ID:").SetBold()).SetBorder(Border.NO_BORDER));
                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph(order.OrderId.ToString())).SetBorder(Border.NO_BORDER));

                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph("Customer:").SetBold()).SetBorder(Border.NO_BORDER));
                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph(order.Customer.Name)).SetBorder(Border.NO_BORDER));

                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph("Email:").SetBold()).SetBorder(Border.NO_BORDER));
                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph(order.Customer.Email)).SetBorder(Border.NO_BORDER));

                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph("Order Date:").SetBold()).SetBorder(Border.NO_BORDER));
                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph(order.CreatedTime.ToString("yyyy-MM-dd"))).SetBorder(Border.NO_BORDER));

                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph("Order Status:").SetBold()).SetBorder(Border.NO_BORDER));
                    orderDetailsTable.AddCell(new Cell().Add(new Paragraph(order.Status.ToString()).SetBorder(Border.NO_BORDER)));
                    
                    document.Add(orderDetailsTable);

                    Table itemTable = new Table(4).UseAllAvailableWidth().SetMarginTop(20);
                    itemTable.AddHeaderCell("Product");
                    itemTable.AddHeaderCell("Quantity");
                    itemTable.AddHeaderCell("Unit Price");
                    itemTable.AddHeaderCell("Total");


                    foreach (var item in order.OrderItems)
                    {
                        itemTable.AddCell(item.Product.Name);
                        itemTable.AddCell(item.Quantity.ToString());
                        itemTable.AddCell(item.UnitPrice.ToString("C"));
                        itemTable.AddCell((item.Quantity * item.UnitPrice).ToString("C"));
                    }

                    itemTable.AddCell(new Cell(1, 3).Add(new Paragraph("Total Amount:").SetBold()).SetTextAlignment(TextAlignment.RIGHT));
                    itemTable.AddCell(new Cell().Add(new Paragraph(order.TotalAmount.ToString("C"))).SetTextAlignment(TextAlignment.RIGHT));

                    document.Add(itemTable);
                    document.Close();
                }
                var invoice = new Invoice
                {
                    OrderId = order.OrderId,
                    CreatedTime = order.CreatedTime,
                    TotalAmount = order.TotalAmount,
                    Status = order.Status,
                };

                await _unitOfWork.repositories<Invoice , int>().AddAsync(invoice);
                await _unitOfWork.CompleteAsync();
                return invoicePath;
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error generating PDF: {ex.Message}");
                throw; 
            }
        }


    }

}
