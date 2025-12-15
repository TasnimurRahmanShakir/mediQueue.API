using mediQueue.API.Model.DTO;
using mediQueue.API.Model.Entity;
using mediQueue.API.Repository.Interfaces;

namespace mediQueue.API.Repository.Implementations
{
    public class UserRepository : IUserRepository
    {
        public Task<User> CreateUser(UserDTO newUser)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteUser(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User?> GetUserByIdAsync(string param)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateUser(Guid id, UserDTO updatedUser)
        {
            throw new NotImplementedException();
        }
    }
}
