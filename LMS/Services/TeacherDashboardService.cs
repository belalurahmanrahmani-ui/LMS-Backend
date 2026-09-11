using LMS.Data;
using LMS.DTOs.Teacher;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class TeacherDashboardService : ITeacherDashboardService
    {
        private readonly LmsDbContext _context;

        public TeacherDashboardService(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<TeacherDashboardDto> GetTeacherDashboardAsync(int teacherId)
        {
            var totalCourses = await _context.Courses
                .CountAsync(c => c.TeacherId == teacherId);

            var publishedCourses = await _context.Courses
                .CountAsync(c => c.TeacherId == teacherId && c.IsPublished);

            var draftCourses = totalCourses - publishedCourses;

            var totalStudents = await _context.Enrollments
                .Where(e => e.Course.TeacherId == teacherId)
                .Select(e => e.StudentId)
                .Distinct()
                .CountAsync();

            return new TeacherDashboardDto
            {
                TotalCourses = totalCourses,
                PublishedCourses = publishedCourses,
                DraftCourses = draftCourses,
                TotalStudents = totalStudents
            };
        }
    }
}