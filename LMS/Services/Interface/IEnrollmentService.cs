using LMS.DTOs.Enrollment;
using LMS.Enums;

namespace LMS.Services.Interface
{
    public interface IEnrollmentService
    {
        Task<(EnrollmentOperationResult Resul, EnrollmentResponseDto? Enrollment)> EnrollStudentAsync(int studentId, int CourseId);
        Task<List<EnrollmentResponseDto>> GetMyCourseAsyn(int studentId);
    }
}
