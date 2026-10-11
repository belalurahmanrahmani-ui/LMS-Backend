using LMS.Enums;

namespace LMS.Services.Interface
{
    public interface ICourseAccessService
    {
        Task<CourseAccess> GetCourseAccessAsync(int courseId, int userId, string? role);
        Task<CourseAccess> GetLessonAccessAsync(int lessonId, int userId, string? role);
    }
}