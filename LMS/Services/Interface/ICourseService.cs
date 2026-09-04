using LMS.DTOs.Course;
using LMS.Enums;

namespace LMS.Services.Interface
{
    public interface ICourseService
    {
        Task<List<CourseResponseDto>> GetAllCoursesAsync();
        Task<CourseResponseDto> GetCourseByIdAsync(int id);
        Task<List<CourseResponseDto>> GetMyCoursesAsync(int teacherId);

        Task<(CourseOperationResult Result, CourseResponseDto? Course)> CreateCourseAsync(CreateCourseDto dto, int teacherId);
        Task<CourseOperationResult> UpdateCourseAsync(int id, UpdateCourseDto dto, int teacherId);
        Task<CourseOperationResult> DeleteCourseAsync(int id, int teacherId);
        Task<CourseOperationResult> PublishCourseAsync(int id, int teacherId);
        Task<CourseOperationResult> UnpublishCourseAsync(int id, int teacherId);
    }
}
