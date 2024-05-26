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

                while (await reader.ReadAsync())
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
            }
            return null;
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
                invoiceDto.OrderAmount = (float)Convert.ToDouble(orderReader["OrderAmount"]);
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

        var document = new PdfDocument();

       
       
            string htmlcontent = "<div style='width:100%; text-align:center'>";
            htmlcontent += "<img style='width:80px;height:80%' src='./Static/landing-img.jpg' />";
            htmlcontent += "<h2>Welcome to AccoMart</h2>";

            htmlcontent += "<h2> Invoice No:" + "1 " + " & Invoice Date:" + invoiceDto.OrderDate + "</h2>";
            htmlcontent += "<h3> Customer : " + invoiceDto.UserName + "</h3>";
            htmlcontent += "<p>" + invoiceDto.Address + "</p>";
            htmlcontent += "<h3> Contact : 9898989898 & Email :ts@in.com </h3>";
            htmlcontent += "<div>";

            htmlcontent += "<table style ='width:100%; border: 1px solid #000'>";
            htmlcontent += "<thead style='font-weight:bold'>";
            htmlcontent += "<tr>";
            htmlcontent += "<td style='border:1px solid #000'> Product Name </td>";
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
                htmlcontent += "<td>" + invoiceDto.UserName + "</td>";
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
        }

        private async Task<byte[]> GetInvoiceByProduct(int orderId, int productId)
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
                    invoiceDto.OrderAmount = (float)Convert.ToDouble(orderReader["OrderAmount"]);
                    userId = Convert.ToString(orderReader["UserId"]);

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
        }

      

    }
    }

