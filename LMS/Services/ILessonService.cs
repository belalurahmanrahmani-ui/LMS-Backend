using LMS.DTOs.Lesson;
using LMS.Enums;

namespace LMS.Services
{
    public interface ILessonService
    {
        Task<List<LessonResponseDto>> GetLessonsByCourseAsync(int courseId);
        Task<LessonResponseDto?> GetLessonByIdAsync(int id);
        Task<(LessonOperationResult Result, LessonResponseDto? Lesson)> CreateLessonAsync(int courseId, CreateLessonDto dto, int teacherId);
        Task<LessonOperationResult> UpdateLessonAsync(int id, UpdateLessonDto dto, int teacherId);
        Task<LessonOperationResult> DeleteLessonAsync(int id, int teacherId);
        Task<LessonOperationResult> PublishLessonAsync(int id, int teacherId);
        Task<LessonOperationResult> UnpublishLessonAsync(int id, int teacherId);
    }
}