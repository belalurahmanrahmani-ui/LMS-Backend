using LMS.DTOs.Course;
using LMS.Enums;

namespace LMS.Services.Interface
{
    public interface ICourseService
    {
        Task<List<CourseResponseDto>> GetAllCoursesAsync(CourseFilterDto filter);
        Task<CourseResponseDto> GetCourseByIdAsync(int id);
        Task<List<CourseResponseDto>> GetMyCoursesAsync(int teacherId);

        Task<(CourseOperationResult Result, CourseResponseDto? Course)> CreateCourseAsync(CreateCourseDto dto, int teacherId);
<<<<<<< HEAD
        Task<CourseOperationResult> UpdateCourseAsync(int id, UpdateCourseDto dto, int teacherId, bool isAdmin);
        Task<CourseOperationResult> DeleteCourseAsync(int id, int teacherId, bool isAdmin);
        Task<CourseOperationResult> PublishCourseAsync(int id, int teacherId, bool isAdmin);
        Task<CourseOperationResult> UnpublishCourseAsync(int id, int teacherId, bool isAdmin);
=======
        Task<CourseOperationResult> UpdateCourseAsync(int id, UpdateCourseDto dto, int teacherId);
        Task<CourseOperationResult> DeleteCourseAsync(int id, int teacherId);
        Task<CourseOperationResult> PublishCourseAsync(int id, int teacherId);
        Task<CourseOperationResult> UnpublishCourseAsync(int id, int teacherId);
>>>>>>> edbfb03e8b76ab0e31d5edac967d6201e59d79cc
        Task<CourseDetailsDto?> GetCourseDetailsAsync(int id);
    }
}