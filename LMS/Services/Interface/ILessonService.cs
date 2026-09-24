using LMS.DTOs.Lesson;
using LMS.Enums;

namespace LMS.Services.Interface
{
    public interface ILessonService
    {
        Task<List<LessonResponseDto>> GetLessonsByCourseAsync(int courseId);
        Task<LessonResponseDto?> GetLessonByIdAsync(int id);
        Task<(LessonOperationResult Result, LessonResponseDto? Lesson)> CreateLessonAsync(int courseId, CreateLessonDto dto, int teacherId);
        Task<LessonOperationResult> UpdateLessonAsync(int id, UpdateLessonDto dto, int teacherId, bool isAdmin);
        Task<LessonOperationResult> DeleteLessonAsync(int id, int teacherId, bool isAdmin);
        Task<LessonOperationResult> PublishLessonAsync(int id, int teacherId, bool isAdmin);
        Task<LessonOperationResult> UnpublishLessonAsync(int id, int teacherId, bool isAdmin);
    }
}