using LMS.DTOs.Student;

namespace LMS.Services.Interface
{
    public interface IStudentDashboardService
    {
        Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId);
    }
}
