using LMS.Data;
using LMS.DTOs.Enrollment;
using LMS.Entities;
using LMS.Enums;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly LmsDbContext _context;
        public EnrollmentService(LmsDbContext context)
        {
            _context = context;
        }

        public async Task<(EnrollmentOperationResult Resul, EnrollmentResponseDto? Enrollment)> EnrollStudentAsync(int studentId, int CourseId)
        {
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == CourseId);
            if(course == null) 
                return(EnrollmentOperationResult.CourseNotFound, null);
            if(!course.IsPublished)
                return(EnrollmentOperationResult.CourseNotPublished, null);
            bool alreadyEnroled = await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.CourseId == CourseId); 
            if(alreadyEnroled)
                return(EnrollmentOperationResult.AlreadyEnrolled, null);
            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = CourseId,
                EnrolledAt = DateTime.UtcNow
            };
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            var teacher = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == course.TeacherId);
            var responseDto = new EnrollmentResponseDto
            {
                Id = enrollment.Id,
                CourseId = course.Id,
                CourseTitle = course.Title,
                TeacherName = teacher?.FullName ?? "Unknown",
                Thumbnail = course.Thumbnail,
                EnrolledAt = enrollment.EnrolledAt
            };
            return (EnrollmentOperationResult.Success, responseDto);
        }

        public async Task<List<EnrollmentResponseDto>> GetMyCourseAsyn(int studentId)
        {
            return await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Select(e => new EnrollmentResponseDto
                {
                    Id = e.Id,
                    CourseId = e.CourseId,
                    CourseTitle = e.Course.Title,
                    TeacherName = e.Course.Teacher.FullName,
                    Thumbnail = e.Course.Thumbnail,
                    EnrolledAt = e.EnrolledAt
                }).ToListAsync();
        }
    }
}
