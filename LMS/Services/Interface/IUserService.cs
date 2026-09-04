using LMS.DTOs;
using LMS.Entities;

namespace LMS.Services.Interface
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetAllUserAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int id);
        Task<UserResponseDto?> UpdateUserAsync(int id, UpdateUserDto dto);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> SetActiveStatusAsync(int id, bool isActive);
        Task<bool> ChangeRoleAsync(int id, UserRole newRole);
    }
}
