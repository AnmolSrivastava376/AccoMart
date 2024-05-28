


using Data.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Data.Repository.Interface;
using PdfSharpCore;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using Azure;
using Data.Models.Statistic_Models;


namespace Data.Repository.Implementation
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IConfiguration _configuration;

        public InvoiceRepository(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        async Task IInvoiceRepository.GenerateInvoice(int orderId)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = $"SELECT * FROM Orders WHERE OrderId = @OrderId;";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                SqlDataReader reader = await command.ExecuteReaderAsync();

                /* while (await reader.ReadAsync())
                 {
                     int? ProductId = reader["ProductId"] as int?;
                     int? CartId = reader["CartId"] as int?;

                     if (ProductId.HasValue)
                     {
                         int actualProductId = ProductId.Value;
                         await reader.CloseAsync();
                         return await GetInvoiceByProduct(orderId, actualProductId);
                     }
                     else
                     {
                         await reader.CloseAsync();
                         return await GetInvoiceByCart(orderId);
                     }
                 }
                 await reader.CloseAsync();
             }*/
                return await GetInvoiceByCart(orderId);
            }
        }

        private async Task<byte[]> GetInvoiceByCart(int orderId)
        {
            GetInvoiceDto invoiceDto = new GetInvoiceDto();

            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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


            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
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
                        InvoiceProductDto product1 = new InvoiceProductDto
                        {
                            ProductName = Convert.ToString(productReader["ProductName"]),
                            ProductDesc = Convert.ToString(productReader["ProductDesc"]),
                            ProductPrice = (decimal)Convert.ToInt32(productReader["ProductPrice"]),
                            Quantity = product.Quantity,
                        };
                        invoiceDto.products ??= new List<InvoiceProductDto>();
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
                foreach (InvoiceProductDto product in invoiceDto.products)
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

            decimal discount = Math.Round(0.05m * productAmount, 2);
            decimal tax = Math.Round(0.18m * (productAmount + discount), 2);
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
            return response;

        }

       /* private async Task<byte[]> GetInvoiceByProduct(int orderId)
        {
            GetInvoiceDto invoiceDto = new GetInvoiceDto();
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string sqlQuery = $" SELECT * FROM Orders WHERE OrderId = @OrderId;";
                SqlCommand command = new SqlCommand(sqlQuery, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                string userId = "";


                SqlDataReader orderReader = await command.ExecuteReaderAsync();
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


                SqlDataReader historyOrderReader = await command.ExecuteReaderAsync();
                while (await historyOrderReader.ReadAsync())
                {
                    productId = Convert.ToInt32(historyOrderReader["OrderDate"]);
                    quanti

                }
                await orderReader.CloseAsync();

                //Fetching Product Details
                string sqlQuery1 = $" SELECT * FROM Product WHERE ProductId = @ProductId;";
                SqlCommand command1 = new SqlCommand(sqlQuery1, connection);
                command1.Parameters.AddWithValue("@ProductId", productId);
                List<InvoiceProductDto> products = new List<InvoiceProductDto>();
                InvoiceProductDto product = new InvoiceProductDto();
                SqlDataReader productReader = await command1.ExecuteReaderAsync();
                while (await productReader.ReadAsync())
                {
                    product.ProductName = Convert.ToString(productReader["ProductName"]);
                    product.ProductDesc = Convert.ToString(productReader["ProductDesc"]);
                    product.ProductPrice = (decimal)Convert.ToInt32(productReader["ProductPrice"]);
                }
                products.Add(product);
                invoiceDto.products = products;
                await productReader.CloseAsync();


                //Fetching User Details

                string sqlQuery2 = $" SELECT * FROM AspNetUsers WHERE Id = @UserId;";
                SqlCommand command2 = new SqlCommand(sqlQuery2, connection);
                command2.Parameters.AddWithValue("@UserId", userId);
                SqlDataReader userReader = await command2.ExecuteReaderAsync();
                while (await userReader.ReadAsync())
                {
                    invoiceDto.UserName = Convert.ToString(userReader["UserName"]);
                    invoiceDto.UserEmail = Convert.ToString(userReader["Email"]);
                }
                await userReader.CloseAsync();


                //Fetching Address                
                string sqlQuery3 = $" SELECT * FROM Addresses WHERE UserId = @UserId;";
                SqlCommand command3 = new SqlCommand(sqlQuery3, connection);
                command3.Parameters.AddWithValue("@UserId", userId);
                AddressModel addressModel = new AddressModel();
                SqlDataReader addressReader = await command3.ExecuteReaderAsync();
                while (await addressReader.ReadAsync())
                {
                    addressModel.ZipCode = Convert.ToString(addressReader["ZipCode"]);
                    addressModel.PhoneNumber = Convert.ToString(addressReader["PhoneNumber"]);
                    addressModel.City = Convert.ToString(addressReader["City"]);
                    addressModel.State = Convert.ToString(addressReader["States"]);
                    addressModel.Street = Convert.ToString(addressReader["Street"]);
                }
                invoiceDto.Address = addressModel;
                await addressReader.CloseAsync();

            }
            var document = new PdfDocument();



            string htmlcontent = "<div style='width:100%; text-align:center'>";
            htmlcontent += "<img style='width:80px;height:80%' src='' />";

            htmlcontent += "<h2>Welcome to AccoMart</h2>";

            htmlcontent += "<h2> Invoice No:" + "1 " + " & Invoice Date:" + invoiceDto.OrderDate + "</h2>";
            htmlcontent += "<h3> Customer : " + invoiceDto.UserName + "</h3>";
            htmlcontent += "<p>" + invoiceDto.Address + "</p>";
            htmlcontent += "<h3> Contact : 9898989898 & Email :ts@in.com </h3>";
            htmlcontent += "<div>";

            htmlcontent += "<table style ='width:100%; border: 1px solid #000'>";
            htmlcontent += "<thead style='font-weight:bold'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'> Product Code </td>";
            htmlcontent += "<td style='border:1px solid #000'> Description </td>";
            htmlcontent += "<td style='border:1px solid #000'>Qty</td>";
            htmlcontent += "<td style='border:1px solid #000'>Price</td >";
            htmlcontent += "<td style='border:1px solid #000'>Total</td>";
            htmlcontent += "</tr>";
            htmlcontent += "</thead >";

            htmlcontent += "<tbody>";
            if (invoiceDto != null)
            {

                htmlcontent += "<tr>";
                htmlcontent += "<td>" + invoiceDto + "</td>";
                htmlcontent += "<td>" + invoiceDto.UserName + "</td>";
                htmlcontent += "<td>" + invoiceDto.UserName + "</td >";
                htmlcontent += "<td>" + invoiceDto.UserName + "</td>";
                htmlcontent += "<td> " + invoiceDto.UserName + "</td >";
                htmlcontent += "</tr>";
            }
            htmlcontent += "</tbody>";

            htmlcontent += "</table>";
            htmlcontent += "</div>";

            htmlcontent += "<div style='text-align:right'>";
            htmlcontent += "<h1> Summary Info </h1>";
            htmlcontent += "<table style='border:1px solid #000;float:right' >";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'> Summary Total </td>";
            htmlcontent += "<td style='border:1px solid #000'> Summary Tax </td>";
            htmlcontent += "<td style='border:1px solid #000'> Summary NetTotal </td>";
            htmlcontent += "</tr>";
            if (invoiceDto != null)
            {
                htmlcontent += "<tr>";
                htmlcontent += "<td style='border: 1px solid #000'> " + invoiceDto.OrderAmount + " </td>";
                htmlcontent += "<td style='border: 1px solid #000'>" + invoiceDto.OrderAmount + "</td>";
                htmlcontent += "<td style='border: 1px solid #000'> " + invoiceDto.OrderAmount + "</td>";
                htmlcontent += "</tr>";
            }
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
            return response;
        }*/



    }
}