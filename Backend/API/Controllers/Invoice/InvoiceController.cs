﻿using Data.Models.DTO;
using Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Service.Services.Interface;
using PdfSharpCore.Pdf;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharpCore;


namespace API.Controllers.Invoice
{
    public class InvoiceController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IConfiguration _configuration; 
        public InvoiceController(ICartService cartService, IConfiguration configuration)
        {
            _configuration = configuration;
            _cartService = cartService;
        }

        [HttpPost("GenerateInvoice/{orderId}")]
        public async Task<IActionResult> GenerateInvoice(int orderId)
        {
           await _cartService.GenerateInvoiceAsync(orderId);
           return Ok();

        }


        [HttpGet("GetInvoice/{orderId}")]
        public async Task<IActionResult>GetInvoice(int orderId)
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
                            State = Convert.ToString(addressReader["State"]),
                            Street = Convert.ToString(addressReader["Street"])
                        };
                    }
                    await addressReader.CloseAsync();
                }

                var document = new PdfDocument();

                string[] copies = { "Customer copy", "Company Copy" };
                for (int i = 0; i < copies.Length; i++)
                {
                    string htmlcontent = "<div style='width:100%; text-align:center'>";
                    htmlcontent += "<img style='width:80px;height:80%' src='' />";
                    htmlcontent += "<h2>" + copies[i] + "</h2>";
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


            }

            byte[]? response = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms);
                    response = ms.ToArray();
                }
                string Filename = "Invoice_" + ".pdf";
                return File(response, "application/pdf", Filename);
            }
        }
    
}
