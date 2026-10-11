using LMS.DTOs.Progress;
using LMS.Enums;

namespace LMS.Services.Interface
{
    public interface ILessonProgressService
    {
        Task<LessonProgressOperationResult> CompleteLessonAsync(int studentId, int lessonId);
        Task<(LessonProgressOperationResult Result, CourseProgressResponseDto? Progress)> GetCourseProgressAsync(int studentId, int courseId);
        Task<List<CourseProgressResponseDto>> GetMyProgressAsync(int studentId);
        Task<List<CourseStudentResponseDto>?> GetCourseStudentsAsync(int courseId, int teacherId);
        Task<List<int>?> GetCompletedLessonIdsAsync(int studentId, int courseId);
    }
}