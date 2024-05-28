
﻿using Data.Models.Address;
using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Service.Models;


namespace API.Controllers.Addresses
{
    [Route("AddressController")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAddressRepository _addressRepository;

        public AddressController(IConfiguration configuration, IAddressRepository addressRepository)
        {
            _addressRepository = addressRepository;
            _configuration = configuration;
        }

        [HttpPost("PostAddress/{userId}")]
        public async Task<IActionResult> PostAddress(AddressModel address, string userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { IsSuccess = false, Message = "Invalid input data.", StatusCode = 400 });
            }

            try
            {
                var result = await _addressRepository.AddAddressAsync(address, userId);
                return Ok(new ApiResponse<int> { IsSuccess = true, Response = result, Message = "Address added successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }

        }

        [HttpGet("GetAddress/addressId={addressId}")]
        public async Task<IActionResult> GetAddress(int addressId)
        {
            try
            {
                var address = await _addressRepository.GetAddressByIdAsync(addressId);
                if (address != null)
                {
                    return Ok(new ApiResponse<AddressModel> { IsSuccess = true, Response = address, StatusCode = 200 });
                }
                else
                {
                    return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Address not found.", StatusCode = 404 });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }


        [HttpGet("GetAddress/{userId}")]
        public async Task<IActionResult> GetAddressByUserId(string userId)
        {
            try
            {
                var addresses = await _addressRepository.GetAddressesByUserIdAsync(userId);
                if (addresses != null && addresses.Count > 0)
                {
                    return Ok(new ApiResponse<List<AddressModel>> { IsSuccess = true, Response = addresses, StatusCode = 200 });
                }
                else
                {
                    return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Addresses not found.", StatusCode = 404 });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {e.Message}", StatusCode = 500 });
            }
        }

        [HttpPut("UpdateAddress/addressId={id}")]
        public async Task<IActionResult> UpdateAddress(int id, AddressModel address)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<string> { IsSuccess = false, Message = "Invalid input data.", StatusCode = 400 });
            }

            try
            {
                bool updated = await _addressRepository.UpdateAddressAsync(id, address);
                if (updated)
                {
                    return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Address updated successfully.", StatusCode = 200 });
                }
                else
                {
                    return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Address not found.", StatusCode = 404 });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpDelete("DeleteAddress/addressId={id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            try
            {
                bool deleted = await _addressRepository.DeleteAddressAsync(id);
                if (deleted)
                {
                    return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Address deleted successfully.", StatusCode = 200 });
                }
                else
                {
                    return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Address not found.", StatusCode = 404 });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"Internal server error: {ex.Message}", StatusCode = 500 });
            }
        }
    }
}

