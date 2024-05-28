using Data.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers.Chart
{
    [ApiController]
    [Route("ChartController")]
    public class ChartController : ControllerBase
    {
        private readonly IChartRepository _chartRepository;

        public ChartController(IChartRepository chartRepository)
        {
            _chartRepository = chartRepository;
        }

        [HttpGet("FetchDailyOrderQuantity")]
        public async Task<IActionResult> FetchDailyOrderQuantity()
        {
            try
            {
                var orderQuantity = await _chartRepository.FetchDailyOrderQuantity();
                return Ok(orderQuantity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FetchCategoryWiseQuantity")]
        public async Task<IActionResult> FetchCategoryWiseQuantity()
        {
            try
            {
                var categoryItems = await _chartRepository.FetchCategoryWiseQuantity();
                return Ok(categoryItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("FetchProductWiseQuantity")]
        public async Task<IActionResult> FetchProductWiseQuantity()
        {
            try
            {
                var productItems = await _chartRepository.FetchProductWiseQuantity();
                return Ok(productItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
