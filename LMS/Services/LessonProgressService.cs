using LMS.Data;
using LMS.DTOs.Progress;
using LMS.Entities;
using LMS.Enums;
using LMS.Services.Interface;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class LessonProgressService : ILessonProgressService
    {
        private readonly LmsDbContext _context;

        public LessonProgressService(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<LessonProgressOperationResult> CompleteLessonAsync(int studentId, int lessonId)
        {
            var lesson = await _context.Lessons
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson == null)
            {
                return LessonProgressOperationResult.LessonNotFound;
            }

            if (!lesson.IsPublished)
            {
                return LessonProgressOperationResult.LessonNotPublished;
            }

            bool isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == lesson.CourseId);

            if (!isEnrolled)
            {
                return LessonProgressOperationResult.NotEnrolled;
            }

            bool alreadyCompleted = await _context.LessonProgresses
                .AnyAsync(lp => lp.StudentId == studentId && lp.LessionId == lessonId);

            if (!alreadyCompleted)
            {
                var progress = new LessonProgress
                {
                    StudentId = studentId,
                    LessionId = lessonId,
                    IsCompletedAt = true,
                    CompletedAt = DateTime.UtcNow
                };

                _context.LessonProgresses.Add(progress);
                await _context.SaveChangesAsync();
            }

            return LessonProgressOperationResult.Success;
        }

        public async Task<(LessonProgressOperationResult Result, CourseProgressResponseDto? Progress)> GetCourseProgressAsync(int studentId, int courseId)
        {
            bool isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (!isEnrolled)
            {
                return (LessonProgressOperationResult.NotEnrolled, null);
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            int totalLessons = await _context.Lessons
                .CountAsync(l => l.CourseId == courseId && l.IsPublished);

            int completedLessons = await _context.LessonProgresses
                .CountAsync(lp => lp.StudentId == studentId
                                && lp.Lesson.CourseId == courseId
                                && lp.Lesson.IsPublished);

            double percentage = totalLessons == 0
                ? 0
                : Math.Round((double)completedLessons / totalLessons * 100, 1);

            var dto = new CourseProgressResponseDto
            {
                CourseId = courseId,
                CourseTitle = course?.Title ?? "Unknown",
                TotalLessons = totalLessons,
                CompletedLessons = completedLessons,
                ProgressPercentage = percentage
            };

            return (LessonProgressOperationResult.Success, dto);
        }

        public async Task<List<CourseProgressResponseDto>> GetMyProgressAsync(int studentId)
        {
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            var result = new List<CourseProgressResponseDto>();

            foreach (var courseId in enrolledCourseIds)
            {
                var (_, progress) = await GetCourseProgressAsync(studentId, courseId);
                if (progress != null)
                {
                    result.Add(progress);
                }
            }

            return result;
        }
    }
}