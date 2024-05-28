using Data.Models.Address;

namespace Data.Repository.Interfaces
{
    public interface IAddressRepository
    {
        Task<int> AddAddressAsync(AddressModel address, string userId);
        Task<AddressModel> GetAddressByIdAsync(int addressId);
        Task<List<AddressModel>> GetAddressesByUserIdAsync(string userId);
        Task<bool> UpdateAddressAsync(int id, AddressModel address);
        Task<bool> DeleteAddressAsync(int id);
    }

}
