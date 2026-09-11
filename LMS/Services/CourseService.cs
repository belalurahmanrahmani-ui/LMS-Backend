using LMS.Data;
using LMS.DTOs.Course;
using LMS.Entities;
using LMS.Enums;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
//using System.Linq.Expressions;

namespace LMS.Services
{
    public class CourseService : ICourseService
    {
        private readonly LmsDbContext _context;
        public CourseService(LmsDbContext context)
        {
            _context = context;
        }
        public async Task<(CourseOperationResult Result, CourseResponseDto? Course)> CreateCourseAsync(CreateCourseDto dto, int teacherId)
        {
            bool categoyExist = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoyExist)
            {
                return (CourseOperationResult.InvalidCategor, null);
            }
            var course =  new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Thumbnail = dto.Thumbnail,
                Price = dto.Price,
                CategoryId = dto.CategoryId,
                TeacherId = teacherId,
                IsPublished = false,
                CreatedAt = DateTime.Now,
                UpdateAt = DateTime.Now
            };
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            var createdCourse = await GetCourseByIdAsync(course.Id);
            return(CourseOperationResult.Sucses, createdCourse);
        }


        public async Task<CourseOperationResult> DeleteCourseAsync(int id, int teacherId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return CourseOperationResult.NotFound;
            if (course.TeacherId != teacherId)
                return CourseOperationResult.Forbidden;
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return CourseOperationResult.Sucses;
        }

        

        public async Task<CourseResponseDto?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Where(c => c.Id == id)
                .Select(ProjectToDto)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CourseResponseDto>> GetMyCoursesAsync(int teacherId)
        {
            return await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Select(ProjectToDto)
                .ToListAsync();
        }

        public async Task<CourseOperationResult> PublishCourseAsync(int id, int teacherId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return CourseOperationResult.NotFound;

            if (course.TeacherId != teacherId)
                return CourseOperationResult.Forbidden;

            course.IsPublished = true;
            course.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return CourseOperationResult.Sucses;
        }

        public async Task<CourseOperationResult> UnpublishCourseAsync(int id, int teacherId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return CourseOperationResult.NotFound;

            if (course.TeacherId != teacherId)
                return CourseOperationResult.Forbidden;

            course.IsPublished = false;
            course.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return CourseOperationResult.Sucses;
        }

        public async Task<CourseOperationResult> UpdateCourseAsync(int id, UpdateCourseDto dto, int teacherId)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
                return CourseOperationResult.NotFound;
            if (course.TeacherId != teacherId)
                return CourseOperationResult.Forbidden;
            bool categoryExist = await _context.Categories.AnyAsync(c => c.Id == dto.CategoryId);
            if (!categoryExist)
                return CourseOperationResult.InvalidCategor;

            course.Title = dto.Title;
            course.Description = dto.Description;
            course.Thumbnail = dto.Thumbnail;
            course.Price = dto.Price;
            course.CategoryId = dto.CategoryId;
            course.UpdateAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return CourseOperationResult.Sucses;
        }

        public async Task<List<CourseResponseDto>> GetAllCoursesAsync(CourseFilterDto filter)
        {
            var query = _context.Courses
                .Where(c => c.IsPublished);

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                query = query.Where(c => c.Title.Contains(filter.Search));
            }

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                query = query.Where(c => c.Category.Name.ToLower() == filter.Category.ToLower());
            }

            if (filter.IsFree.HasValue)
            {
                query = filter.IsFree.Value
                    ? query.Where(c => c.Price == 0)
                    : query.Where(c => c.Price > 0);
            }

            return await query
                .Select(ProjectToDto)
                .ToListAsync();
        }

        public async Task<CourseDetailsDto?> GetCourseDetailsAsync(int id)
        {
            var course = await _context.Courses
                .Where(c => c.Id == id && c.IsPublished)
                .Include(c => c.Teacher)
                .Include(c => c.Category)
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync();
            if (course == null)
                return null;
            var publishedLessons = course.Lessons
                .Where(l => l.IsPublished)
                .OrderBy(l => l.Order)
                .ToList();
            var enrollmentCount = await _context.Enrollments
                .CountAsync(e => e.CourseId == id);
            return new CourseDetailsDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Thumbnail = course.Thumbnail,
                Price = course.Price,
                TeacherName = course.Teacher.FullName,
                CategoryName = course.Category.Name,
                LessonCount = publishedLessons.Count,
                TotalDuration = publishedLessons.Sum(l => l.Duration),
                EnrollmentCount = enrollmentCount,
                Lessons = publishedLessons.Select(l => new LessonSummaryDto
                {
                    Id = l.Id,
                    Title = l.Title,
                    Duration = l.Duration,
                    Order = l.Order
                }).ToList()
            };

        }

        private static readonly Expression<Func<Course, CourseResponseDto>> ProjectToDto = c => new CourseResponseDto
        {
            Id = c.Id,
            Title = c.Title,
            Description = c.Description,
            Thumbnail = c.Thumbnail,
            Price = c.Price,
            IsPublished = c.IsPublished,
            TeacherId = c.TeacherId,
            TeacherName = c.Teacher.FullName,
            CategoryId = c.CategoryId,
            CategoryName = c.Category.Name,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdateAt
        };
    }
}
