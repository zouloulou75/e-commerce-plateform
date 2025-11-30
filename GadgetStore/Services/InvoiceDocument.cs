// File: Services/InvoiceDocument.cs
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using GadgetStore.Models;   // This line was missing!

namespace GadgetStore.Services   // Add proper namespace
{
    public class InvoiceDocument : IDocument
    {
        public Order Model { get; }

        public InvoiceDocument(Order order)
        {
            Model = order;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSettings() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(40);
                    page.Size(PageSizes.A4);

                    page.Header().Height(100).Background(Colors.Blue.Darken4)
                        .AlignCenter().AlignMiddle()
                        .Text("TekUp STORE").FontColor(Colors.White).Bold();

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Spacing(20);

                            // Invoice Info
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text($"Invoice #{Model.Id}").FontSize(20).Bold();
                                    col.Item().Text($"Date: {Model.OrderDate:MMMM dd, yyyy HH:mm}");
                                    col.Item().Text($"Payment: {Model.PaymentMethod}");
                                    col.Item().Text($"Status: {Model.Status}");
                                });

                                row.ConstantItem(200).AlignRight().Column(col =>
                                {
                                    col.Item().AlignRight().Text("Tek up  Store").Bold();
                                    col.Item().AlignRight().Text("Ghazela");
                                    col.Item().AlignRight().Text("support@tekupstore.com");
                                });
                            });

                            column.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten1);

                            // Customer Info
                            column.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("Bill To:").Bold();
                                    col.Item().Text(Model.CustomerName);
                                    col.Item().Text(Model.Email);
                                    col.Item().Text(Model.Phone);
                                    col.Item().Text(Model.Address);
                                });
                            });

                            // Items Table
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.ConstantColumn(50);
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn();
                                    columns.ConstantColumn(80);
                                    columns.ConstantColumn(80);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Qty").Bold().FontSize(12);
                                    header.Cell().Text("Product").Bold().FontSize(12);
                                    header.Cell().Text("Description").Bold().FontSize(12);
                                    header.Cell().AlignRight().Text("Price").Bold();
                                    header.Cell().AlignRight().Text("Total").Bold();

                                    header.Cell().ColumnSpan(5).PaddingTop(10).LineHorizontal(1);
                                });

                                foreach (var item in Model.OrderItems)
                                {
                                    table.Cell().Text(item.Quantity.ToString());
                                    table.Cell().Text(item.Product?.Name ?? "Unknown");
                                    table.Cell().Text("–");
                                    table.Cell().AlignRight().Text($"{item.Price:C2}");
                                    table.Cell().AlignRight().Text($"{item.Price * item.Quantity:C2}");
                                }

                                table.Cell().ColumnSpan(5).PaddingTop(10).LineHorizontal(1);

                                // Total
                                table.Cell().ColumnSpan(4).AlignRight().PaddingRight(10)
                                    .Text("Grand Total").Bold().FontSize(16);

                                table.Cell().AlignRight()
                                    .Text($"{Model.TotalAmount:C2}")
                                    .Bold().FontSize(18).FontColor(Colors.Green.Darken3);
                            });

                            column.Item().AlignCenter().PaddingTop(30)
                                .Text("Thank you for shopping with Tekup Store!").Italic();
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(txt =>
                        {
                            txt.CurrentPageNumber();
                            txt.Span(" / ");
                            txt.TotalPages();
                        });
                });
        }
    }
}