using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Data.Repository.Interfaces;
using Service.Models;
using Data.Models.Delivery;
using Data.Models.ViewModels;


namespace API.Controllers.DeliveryServices
{
    [ApiController]
    [Route("DeliveryServiceController")]
    public class DeliveryServiceController : ControllerBase
    {
        private readonly IDeliveryRepository _deliveryRepository;

        public DeliveryServiceController(IDeliveryRepository deliveryRepository)
        {
            _deliveryRepository = deliveryRepository;
        }

        [HttpPost("AddDeliveryService")]
        public async Task<IActionResult> AddDeliveryService(CreateDeliveryService deliveryService)
        {
            try
            {
                await _deliveryRepository.AddDeliveryService(deliveryService);
                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Delivery service added successfully.", StatusCode = 200 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while adding the delivery service: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpDelete("DeleteDeliveryService/{id}")]
        public async Task<IActionResult> DeleteDeliveryService(int id)
        {
            try
            {
                await _deliveryRepository.DeleteDeliveryService(id);
                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Delivery service deleted successfully.", StatusCode = 200 });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Delivery service not found.", StatusCode = 404 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while deleting the delivery service: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpPut("UpdateDeliveryService/{id}")]
        public async Task<IActionResult> UpdateDeliveryService(int id, CreateDeliveryService deliveryService)
        {
            try
            {
                await _deliveryRepository.UpdateDeliveryService(id, deliveryService);
                return Ok(new ApiResponse<string> { IsSuccess = true, Message = "Delivery service updated successfully.", StatusCode = 200 });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new ApiResponse<string> { IsSuccess = false, Message = "Delivery service not found.", StatusCode = 404 });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while updating the delivery service: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("GetAllDeliveryServices")]
        public async Task<IActionResult> GetAllDeliveryServices()
        {
            try
            {
                var deliveryServices = await _deliveryRepository.GetAllDeliveryServices();
                return Ok(new ApiResponse<List<DeliveryService>> { IsSuccess = true, Message = "Delivery services retrieved successfully.", StatusCode = 200, Response = deliveryServices });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while retrieving delivery services: {ex.Message}", StatusCode = 500 });
            }
        }

        [HttpGet("GetDeliveryDays/{deliveryId}")]
        public async Task<IActionResult> GetDeliveryDays(int deliveryId)
        {
            try
            {
                var days = await _deliveryRepository.GetDeliveryDays(deliveryId);
                return Ok(new ApiResponse<int> { IsSuccess = true, Message = "Delivery days retrieved successfully.", StatusCode = 200, Response = days });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string> { IsSuccess = false, Message = $"An error occurred while retrieving delivery days: {ex.Message}", StatusCode = 500 });
            }
        }
    }
}
