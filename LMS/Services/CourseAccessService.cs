using LMS.Data;
using LMS.Enums;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class CourseAccessService : ICourseAccessService
    {
        private readonly LmsDbContext _context;

        public CourseAccessService(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<CourseAccess> GetCourseAccessAsync(int courseId, int userId, string? role)
        {
            if (role == "Admin")
            {
                return CourseAccess.Full;
            }

            if (role == "Teacher")
            {
                bool isOwner = await _context.Courses
                    .AnyAsync(c => c.Id == courseId && c.TeacherId == userId);
                return isOwner ? CourseAccess.Full : CourseAccess.None;
            }

            if (role == "Student")
            {
                bool isEnrolled = await _context.Enrollments
                    .AnyAsync(e => e.StudentId == userId && e.CourseId == courseId);
                return isEnrolled ? CourseAccess.Student : CourseAccess.None;
            }

            return CourseAccess.None;
        }

        public async Task<CourseAccess> GetLessonAccessAsync(int lessonId, int userId, string? role)
        {
            var lesson = await _context.Lessons
                .Where(l => l.Id == lessonId)
                .Select(l => new { l.CourseId, l.IsPublished })
                .FirstOrDefaultAsync();

            if (lesson == null)
            {
                return CourseAccess.None;
            }

            var access = await GetCourseAccessAsync(lesson.CourseId, userId, role);

            // دانشجو به درس منتشرنشده دسترسی ندارد
            if (access == CourseAccess.Student && !lesson.IsPublished)
            {
                return CourseAccess.None;
            }

            return access;
        }
    }
}