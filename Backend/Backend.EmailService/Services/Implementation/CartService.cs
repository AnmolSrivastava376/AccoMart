using Data.Models.CartModels;
using Data.Repository.Interfaces;
using Service.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Data.Repository.Implementation;
using Data.Models;

namespace Service.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IConfiguration _configuration;
        public CartService(ICartRepository cartRepository, IConfiguration configuration)
        {
            _cartRepository = cartRepository;
            _configuration = configuration;
        }
        async Task<CartItem> ICartService.AddItemToCartAsync(int productId, int quantity, int cardId)
        {
            return await _cartRepository.AddCartItem(productId, quantity,cardId);
        }

        async Task ICartService.DeleteCartItemAsync(int productId)
        {
            await _cartRepository.DeleteCartItem(productId);
        }

        async Task<CartItem> ICartService.UpdateCartItemAsync(int productId, int quantity, int cartId)
        {
            return await _cartRepository.UpdateCartItem(productId, quantity, cartId);
        }

        async Task<IEnumerable<CartItem>> ICartService.GetCartItemsAsync(int cartId)
        {
            return await _cartRepository.GetCartItems(cartId);
        }

        public async Task<int> AddToCartAsync()
        {
            int cartId = 0;
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                // Create a new instance of the Cart entity

                string insertCartQuery = "INSERT INTO Cart DEFAULT VALUES; SELECT SCOPE_IDENTITY();";
                SqlCommand insertCartCommand = new SqlCommand(insertCartQuery, connection);
                object result = await insertCartCommand.ExecuteScalarAsync();
                if (result != null && result != DBNull.Value)
                {
                    cartId = Convert.ToInt32(result);
                }
            }
            return cartId;
        }


        async Task ICartService.AddUser(Users user)
        {
            using (SqlConnection connection = new SqlConnection(_configuration["ConnectionStrings:AZURE_SQL_CONNECTIONSTRING"]))
            {
                await connection.OpenAsync();
                string insertCartQuery = "INSERT INTO Users (UserName, UserPassword, UserEmail, CartId) VALUES (@UserName, @UserPassword, @UserEmail, @CartId);";
                SqlCommand insertCartCommand = new SqlCommand(insertCartQuery, connection);
                insertCartCommand.Parameters.AddWithValue("@UserName", user.UserName);
                insertCartCommand.Parameters.AddWithValue("@UserPassword", user.UserPassword);
                insertCartCommand.Parameters.AddWithValue("@UserEmail", user.UserEmail);
                insertCartCommand.Parameters.AddWithValue("@CartId", user.CartId);
                await insertCartCommand.ExecuteNonQueryAsync();

            }
        }

        async Task ICartService.GenerateInvoiceAsync(int cartId)
        {
             await _cartRepository.GenerateInvoice(cartId);
        }

        async Task ICartService.GetInvoiceAsync(int orderId)
        {
           await _cartRepository.GetInvoice(orderId);
        }

        async Task ICartService.DeleteCartAsync(int cartId)
        {
            await _cartRepository.DeleteCart(cartId);
        }
    }
}
