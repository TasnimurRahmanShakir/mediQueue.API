using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;

namespace mediQueue.API.Repository.Interfaces
{
    public interface IUserRepository
    {

        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string param);
        Task<User> CreateUser(UserDTO newUser);
        Task<int> UpdateUser(Guid id, UserDTO updatedUser);
        Task<int> DeleteUser(Guid id);

    }
}
