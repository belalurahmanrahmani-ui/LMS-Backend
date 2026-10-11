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
        public async Task<List<CourseStudentResponseDto>?> GetCourseStudentsAsync(int courseId, int teacherId)
        {
            
            bool isOwner = await _context.Courses
                .AnyAsync(c => c.Id == courseId && c.TeacherId == teacherId);

            if (!isOwner)
            {
                return null;
            }

            int totalLessons = await _context.Lessons
                .CountAsync(l => l.CourseId == courseId && l.IsPublished);

            var students = await _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .OrderBy(e => e.EnrolledAt)
                .Select(e => new CourseStudentResponseDto
                {
                    StudentId = e.StudentId,
                    FullName = e.Student.FullName,
                    Email = e.Student.Email,
                    EnrolledAt = e.EnrolledAt,
                    TotalLessons = totalLessons,
                    CompletedLessons = e.Student.LessonProgresses
                        .Count(lp => lp.Lesson.CourseId == courseId && lp.Lesson.IsPublished)
                })
                .ToListAsync();

            foreach (var student in students)
            {
                student.ProgressPercentage = totalLessons == 0
                    ? 0
                    : Math.Round((double)student.CompletedLessons / totalLessons * 100, 1);
            }

            return students;
        }
        public async Task<List<int>?> GetCompletedLessonIdsAsync(int studentId, int courseId)
        {
            bool isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (!isEnrolled)
            {
                return null;
            }

            return await _context.LessonProgresses
                .Where(lp => lp.StudentId == studentId
                          && lp.Lesson.CourseId == courseId
                          && lp.Lesson.IsPublished)
                .Select(lp => lp.LessionId)
                .ToListAsync();
        }
    }
}