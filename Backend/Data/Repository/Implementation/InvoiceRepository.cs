using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Data.Repository.Interface;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Data.Models.Address;
using Data.Models.ViewModels;
using Data.Models.OrderModels;
using Microsoft.AspNetCore.Identity;
using Service.Models;
using Data.Models.Authentication.User;
using Data.Repository.Interfaces;


namespace Data.Repository.Implementation
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IInvoiceEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly string connectionstring = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

        public InvoiceRepository( UserManager<ApplicationUser> userManager, IInvoiceEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        async Task IInvoiceRepository.GenerateInvoice(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string checkProductQuery = "INSERT INTO Invoice (OrderId) VALUES(@OrderId);";
                SqlCommand checkProductCommand = new SqlCommand(checkProductQuery, connection);
                checkProductCommand.Parameters.AddWithValue("@OrderId", orderId);
                await checkProductCommand.ExecuteScalarAsync();
            }
        }

        async Task<byte[]> IInvoiceRepository.GetInvoice(int orderId)
        {
            GetInvoice invoiceDto = new GetInvoice();

            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string sqlOrderQuery = "SELECT * FROM Orders WHERE OrderId = @OrderId;";
                SqlCommand orderCommand = new SqlCommand(sqlOrderQuery, connection);
                orderCommand.Parameters.AddWithValue("@OrderId", orderId);

                SqlDataReader orderReader = await orderCommand.ExecuteReaderAsync();
                string userId = "";
                while (await orderReader.ReadAsync())
                {
                    invoiceDto.OrderDate = Convert.ToDateTime(orderReader["OrderDate"]);
                    TimeSpan orderTime = (TimeSpan)orderReader["OrderTime"];
                    DateTime today = invoiceDto.OrderDate;
                    DateTime orderDateTime = today.Add(orderTime);

                    invoiceDto.OrderTime = orderDateTime;
                    invoiceDto.OrderAmount = (decimal)Convert.ToInt32(orderReader["OrderAmount"]);
                    userId = Convert.ToString(orderReader["UserId"]);
                }
                await orderReader.CloseAsync();


                // Fetching User Details
                string sqlUserQuery = "SELECT * FROM AspNetUsers WHERE Id = @UserId;";
                SqlCommand userCommand = new SqlCommand(sqlUserQuery, connection);
                userCommand.Parameters.AddWithValue("@UserId", userId);

                SqlDataReader userReader = await userCommand.ExecuteReaderAsync();

                while (await userReader.ReadAsync())
                {
                    invoiceDto.UserName = Convert.ToString(userReader["UserName"]);
                    invoiceDto.UserEmail = Convert.ToString(userReader["Email"]);
                }
                await userReader.CloseAsync();

                // Fetching Address
                string sqlAddressQuery = "SELECT * FROM Addresses WHERE UserId = @UserId;";
                SqlCommand addressCommand = new SqlCommand(sqlAddressQuery, connection);
                addressCommand.Parameters.AddWithValue("@UserId", userId);

                SqlDataReader addressReader = await addressCommand.ExecuteReaderAsync();

                while (await addressReader.ReadAsync())
                {
                    invoiceDto.Address = new AddressModel
                    {
                        ZipCode = Convert.ToString(addressReader["ZipCode"]),
                        PhoneNumber = Convert.ToString(addressReader["PhoneNumber"]),
                        City = Convert.ToString(addressReader["City"]),
                        State = Convert.ToString(addressReader["States"]),
                        Street = Convert.ToString(addressReader["Street"])
                    };
                }
                await addressReader.CloseAsync();
            }


            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                await connection.OpenAsync();
                string sqlOrderQuery = "SELECT * FROM OrderHistory WHERE OrderId = @OrderId;";
                SqlCommand orderCommand = new SqlCommand(sqlOrderQuery, connection);
                orderCommand.Parameters.AddWithValue("@OrderId", orderId);

                SqlDataReader orderReader = await orderCommand.ExecuteReaderAsync();
                List<OrderHistory> orderHistories = new List<OrderHistory>();
                while (await orderReader.ReadAsync())
                {
                    OrderHistory orderHistory = new OrderHistory
                    {
                        ProductId = Convert.ToInt32(orderReader["ProductId"]),
                        Quantity = Convert.ToInt32(orderReader["Quantity"])
                    };
                    orderHistories.Add(orderHistory);

                }

                await orderReader.CloseAsync();
                foreach (OrderHistory product in orderHistories)
                {
                    string sqlProductQuery = "SELECT * FROM Product WHERE ProductId = @ProductId;";
                    SqlCommand productCommand = new SqlCommand(sqlProductQuery, connection);
                    productCommand.Parameters.AddWithValue("@ProductId", product.ProductId);

                    SqlDataReader productReader = await productCommand.ExecuteReaderAsync();

                    while (await productReader.ReadAsync())
                    {
                        InvoiceProduct product1 = new InvoiceProduct
                        {
                            ProductName = Convert.ToString(productReader["ProductName"]),
                            ProductDesc = Convert.ToString(productReader["ProductDesc"]),
                            ProductPrice = (decimal)Convert.ToInt32(productReader["ProductPrice"]),
                            Quantity = product.Quantity,
                        };
                        invoiceDto.products ??= new List<InvoiceProduct>();
                        invoiceDto.products.Add(product1);
                    }
                    await productReader.CloseAsync();
                }
            }

            var document = new PdfDocument();
            string htmlcontent = "<div style='width:100%; text-align:center'>";

            // Logo
            htmlcontent += $"<img style='width:80px;height:80%' src='{_configuration["Logo:Image"]}' />";
            htmlcontent += "<h2>Welcome to AccoMart</h2>";

            // Order No and Invoice Date
            htmlcontent += $"<h3 style='text-align:left'>Order No: {orderId}</h3>";
            htmlcontent += $"<h3 style='text-align:left'>Invoice Date: {invoiceDto.OrderDate}</h3>";

            // Customer Details
            htmlcontent += "<h3 style='text-align:left'>Customer Details</h3>";
            htmlcontent += $"<p style='text-align:left; margin-bottom:5px'>Customer: {invoiceDto.UserName}</p>";
            htmlcontent += $"<p style='text-align:left; margin-bottom:5px'>Address: {invoiceDto.Address.Street}, {invoiceDto.Address.City}, {invoiceDto.Address.State}, {invoiceDto.Address.ZipCode}</p>";
            htmlcontent += $"<p style='text-align:left; margin-bottom:5px'>Contact: {invoiceDto.PhoneNumber}</p>";
            htmlcontent += $"<p style='text-align:left; margin-bottom:5px'>Email: {invoiceDto.UserEmail}</p>";

            htmlcontent += "<div>";

            // Table Header
            htmlcontent += "<table style ='width:100%; border-collapse: collapse; border: 1px solid #000'>";
            htmlcontent += "<thead style='font-weight:bold'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'>Product Name</td>";
            htmlcontent += "<td style='border:1px solid #000'>Qty</td>";
            htmlcontent += "<td style='border:1px solid #000'>Price</td>";
            htmlcontent += "<td style='border:1px solid #000'>Total</td>";
            htmlcontent += "</tr>";
            htmlcontent += "</thead>";

            htmlcontent += "<tbody>";

            // Product Entries
            decimal productAmount = 0.00M;

            if (invoiceDto != null && invoiceDto.products != null && invoiceDto.products.Count > 0)
            {
                foreach (InvoiceProduct product in invoiceDto.products)
                {
                    htmlcontent += "<tr>";
                    htmlcontent += $"<td style='border:1px solid #000'>{product.ProductName}</td>";
                    htmlcontent += $"<td style='border:1px solid #000'>{product.Quantity}</td>";
                    htmlcontent += $"<td style='border:1px solid #000'>{product.ProductPrice.ToString("0.00")}</td>";
                    htmlcontent += $"<td style='border:1px solid #000'>{(product.ProductPrice * product.Quantity).ToString("0.00")}</td>";
                    htmlcontent += "</tr>";

                    productAmount += product.ProductPrice * product.Quantity;
                }
            }

            htmlcontent += "</tbody>";

            htmlcontent += "</table>";
            htmlcontent += "</div>";

            htmlcontent += "<div style='text-align:left'>";
            htmlcontent += "<h1>Order Summary</h1>";
            htmlcontent += "<table style='border-collapse: collapse; border: 1px solid #000'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'> Total </td>";
            htmlcontent += "<td style='border:1px solid #000'> Discount </td>";
            htmlcontent += "<td style='border:1px solid #000'> Summary Tax </td>";
            htmlcontent += "<td style='border:1px solid #000'> Amount To Be Paid </td>";
            htmlcontent += "</tr>";

            decimal discount = Math.Round(decimal.Parse(_configuration["Constants:Discount"]) * productAmount, 2);
            decimal tax = Math.Round(decimal.Parse(_configuration["Constants:Discount"]) * (productAmount + discount), 2);
            decimal totalAmount = Math.Round(productAmount - discount + tax, 2);

            htmlcontent += "<tr>";
            htmlcontent += $"<td style='border:1px solid #000'>{productAmount.ToString("0.00")}</td>";
            htmlcontent += $"<td style='border:1px solid #000'>{discount.ToString("0.00")}</td>";
            htmlcontent += $"<td style='border:1px solid #000'>{tax.ToString("0.00")}</td>";
            htmlcontent += $"<td style='border:1px solid #000'>{totalAmount.ToString("0.00")}</td>";
            htmlcontent += "</tr>";

            htmlcontent += "</table>";
            htmlcontent += "</div>";

            htmlcontent += "</div>";

            PdfGenerator.AddPdfPages(document, htmlcontent, PageSize.A4);

            byte[] response = null;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                response = ms.ToArray();
            }



            byte[] pdfBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms);
                pdfBytes = ms.ToArray();
            }

            // Send the PDF over email
            var user = await _userManager.FindByEmailAsync(invoiceDto.UserEmail);
            if (user != null)
            {
                var message = new Message(new string[] { user.Email }, "Invoice Pdf", "Please find attached your invoice PDF.");
                message.Attachments.Add(("invoice.pdf", pdfBytes, "application/pdf"));
                 _emailService.SendEmailInvoice(message);  
            } 

            return response;

        }

    }
}