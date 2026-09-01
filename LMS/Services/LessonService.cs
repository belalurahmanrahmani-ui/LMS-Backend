using LMS.Data;
using LMS.DTOs.Lesson;
using LMS.Entities;
using LMS.Enums;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class LessonService : ILessonService
    {
        private readonly LmsDbContext _context;
        public LessonService(LmsDbContext Context)
        {
             _context = Context;
        }
        public async Task<(LessonOperationResult Result, LessonResponseDto? Lesson)> CreateLessonAsync(int courseId, CreateLessonDto dto, int teacherId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
                return (LessonOperationResult.CourseNotFound, null);
            if(course.TeacherId != teacherId)
                return(LessonOperationResult.Forbidden, null);
            var lesson = new Lesson
            {
                CourseId = courseId,
                Title = dto.Title,
                Description = dto.Description,
                VideoUrl = dto.VideoUrl,
                Duration = dto.Duration,
                Order = dto.Order,
                IsPublished = false
            };
            _context.Lessons.Add(lesson);  
            await _context.SaveChangesAsync();
            var createdLesson = await GetLessonByIdAsync(lesson.Id);
            return (LessonOperationResult.Success, createdLesson);
        }

        public async Task<LessonOperationResult> DeleteLessonAsync(int id, int teacherId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
                return LessonOperationResult.NotFound;
            if (lesson.Course.TeacherId != teacherId)
                return LessonOperationResult.Forbidden;
            _context.Lessons.Remove(lesson);
            await _context.SaveChangesAsync();
            return LessonOperationResult.Success;
        }

        public async Task<LessonResponseDto?> GetLessonByIdAsync(int id)
        {
            return await _context.Lessons
                .Where(l => l.Id == id)
                .Select(ProjectToDto)
                .FirstOrDefaultAsync();
        }

        public async Task<List<LessonResponseDto>> GetLessonsByCourseAsync(int courseId)
        {
            return await _context.Lessons
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Id)
                .Select(ProjectToDto)
                .ToListAsync();
        }

        public async Task<LessonOperationResult> PublishLessonAsync(int id, int teacherId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
                return LessonOperationResult.NotFound;
            if (lesson.Course.TeacherId != teacherId)
                return LessonOperationResult.Forbidden;
            lesson.IsPublished = true;
            await _context.SaveChangesAsync();
            return LessonOperationResult.Success;   
        }

        public async Task<LessonOperationResult> UnpublishLessonAsync(int id, int teacherId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                return LessonOperationResult.NotFound;

            if (lesson.Course.TeacherId != teacherId)
                return LessonOperationResult.Forbidden;

            lesson.IsPublished = false;
            await _context.SaveChangesAsync();
            return LessonOperationResult.Success;

        }

        public async Task<LessonOperationResult> UpdateLessonAsync(int id, UpdateLessonDto dto, int teacherId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);
            if (lesson == null)
                return LessonOperationResult.NotFound;
            if (lesson.Course.TeacherId != teacherId)
                return LessonOperationResult.Forbidden;
            lesson.Title = dto.Title;
            lesson.Description = dto.Description;
            lesson.Duration = dto.Duration;
            lesson.VideoUrl = dto.VideoUrl;
            lesson.Order = dto.Order;

            await _context.SaveChangesAsync();
            return LessonOperationResult.Success;

        }

        private static readonly System.Linq.Expressions.Expression<Func<Lesson, LessonResponseDto>> ProjectToDto = l => new LessonResponseDto
        {
            Id = l.Id,
            CourseId = l.CourseId,
            Title = l.Title,
            Description = l.Description,
            VideoUrl = l.VideoUrl,
            Duration = l.Duration,
            Order = l.Order,
            IsPublished = l.IsPublished
        };
    }
}
