using System.Collections.Generic;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.Delivery;
using Data.Models.ViewModels;

namespace Data.Repository.Interfaces
{
    public interface IDeliveryRepository
    {
        Task AddDeliveryService(CreateDeliveryService deliveryService);
        Task DeleteDeliveryService(int id);
        Task UpdateDeliveryService(int id, CreateDeliveryService deliveryService);
        Task<List<DeliveryService>> GetAllDeliveryServices();
        Task<int> GetDeliveryDays(int deliveryId);
    }
}
