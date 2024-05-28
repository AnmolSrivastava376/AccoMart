using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.DTO;

namespace Data.Repository.Interfaces
{
    public interface IDeliveryRepository
    {
        Task AddDeliveryService(CreateDeliveryServiceDto deliveryService);
        Task DeleteDeliveryService(int id);
        Task UpdateDeliveryService(int id, CreateDeliveryServiceDto deliveryService);
        Task<List<DeliveryService>> GetAllDeliveryServices();
        Task<int> GetDeliveryDays(int deliveryId);
    }
}
