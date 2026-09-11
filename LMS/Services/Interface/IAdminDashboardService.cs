using LMS.DTOs.Admin;

namespace LMS.Services.Interface
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();
    }
}
