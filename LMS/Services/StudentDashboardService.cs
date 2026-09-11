using LMS.Data;
using LMS.DTOs.Student;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly LmsDbContext _context;

        public StudentDashboardService(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<StudentDashboardDto> GetStudentDashboardAsync(int studentId)
        {
            // 1) همه Courseهایی که این Student در آن‌ها Enroll است
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => e.CourseId)
                .ToListAsync();

            var totalEnrolledCourses = enrolledCourseIds.Count;

            // 2) برای هر Course، چک می‌کنیم آیا Completed است یا نه
            var completedCourses = 0;

            foreach (var courseId in enrolledCourseIds)
            {
                var publishedLessonCount = await _context.Lessons
                    .CountAsync(l => l.CourseId == courseId && l.IsPublished);

                if (publishedLessonCount == 0)
                    continue; // کورس بدون درس منتشرشده را Completed حساب نمی‌کنیم

                var completedLessonCount = await _context.LessonProgresses
                    .CountAsync(lp => lp.StudentId == studentId
                                    && lp.IsCompletedAt
                                    && lp.Lesson.CourseId == courseId);

                if (completedLessonCount == publishedLessonCount)
                    completedCourses++;
            }

            // 3) آخرین ۵ درسی که این Student تکمیل کرده
            var recentLessons = await _context.LessonProgresses
                .Where(lp => lp.StudentId == studentId && lp.IsCompletedAt)
                .OrderByDescending(lp => lp.CompletedAt)
                .Take(5)
                .Select(lp => new RecentLessonDto
                {
                    LessonId = lp.LessionId,
                    LessonTitle = lp.Lesson.Title,
                    CourseTitle = lp.Lesson.Course.Title,
                    CompletedAt = lp.CompletedAt
                })
                .ToListAsync();

            return new StudentDashboardDto
            {
                TotalEnrolledCourses = totalEnrolledCourses,
                CompletedCourses = completedCourses,
                RecentLessons = recentLessons
            };
        }
    }
}