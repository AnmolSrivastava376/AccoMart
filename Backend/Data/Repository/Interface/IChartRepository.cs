using Data.Models.Statistic_Models;

namespace Data.Repository.Interfaces
{
    public interface IChartRepository
    {
        Task<List<OrderQuantity>> FetchDailyOrderQuantity();
        Task<List<CategoryItem>> FetchCategoryWiseQuantity();
        Task<List<ProductItem>> FetchProductWiseQuantity();
    }
}
