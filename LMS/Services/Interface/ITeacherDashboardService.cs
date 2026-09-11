using LMS.DTOs.Teacher;

namespace LMS.Services.Interface
{
    public interface ITeacherDashboardService
    {
        Task<TeacherDashboardDto> GetTeacherDashboardAsync(int teacherId);
    }
}