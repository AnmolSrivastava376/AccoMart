﻿using Data.Models.CartModels;
using Data.Repository.Interfaces;
using Service.Services.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;


namespace Service.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly string connectionstring = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");

        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;

        }
        async Task<IEnumerable<CartItem>> ICartService.AddToCartAsync(int cartId, IEnumerable<CartItem> cart)
        {
            return await _cartRepository.AddCart(cartId,cart);
        }

        async Task<IEnumerable<CartItem>> ICartService.GetCartItemsAsync(int cartId)
        {
            return await _cartRepository.GetCartItems(cartId);
        }

        public async Task<int> AddToCartAsync()
        {
            int cartId = 0;
            using (SqlConnection connection = new SqlConnection(connectionstring))
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
            using (SqlConnection connection = new SqlConnection(connectionstring))
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

        async Task ICartService.DeleteCartAsync(int cartId)
        {
            await _cartRepository.DeleteCart(cartId);
        }
    }
}
