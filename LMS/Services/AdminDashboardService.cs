using LMS.Data;
using LMS.DTOs.Admin;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly LmsDbContext _context;
        public AdminDashboardService(LmsDbContext context)
        {
            _context = context;
        }
        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var TotalUsers = await _context.Users.CountAsync();
            var totalTeachers = await _context.Users.CountAsync(u => u.Role == Enums.UserRole.Teacher);
            var totalStuden = await _context.Users.CountAsync(u => u.Role == Enums.UserRole.Student);
            var totalCourse = await _context.Courses.CountAsync();
            var publishidCourse = await _context.Courses.CountAsync(c => c.IsPublished);
            var totalEnrolments = await _context.Enrollments.CountAsync();

            return new AdminDashboardDto
            {
                TotalUsers = TotalUsers,
                TotalTeachers = totalTeachers,
                TotalStudents = totalStuden,
                TotalCourses = totalCourse,
                PublishedCourses = publishidCourse,
                TotalEnrollments = totalEnrolments,

            };
        }
    }
}
