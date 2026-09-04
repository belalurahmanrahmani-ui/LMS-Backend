using LMS.Data;
using LMS.DTOs.Material;
using LMS.Entities;
using LMS.Enums;
using LMS.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LMS.Services
{
    public class MaterialService : IMaterialService
    {
        private readonly LmsDbContext _context;
        public MaterialService(LmsDbContext context)
        {
            _context = context;
        }
        public async Task<(MaterialOperationResult Result, MaterialResponseDto? Material)> CreateMaterialAsync(int lessonId, CreateMaterialDto dto, int teacherId)
        {
            var lesson = await _context.Lessons
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == lessonId);
            if (lesson == null)
                return (MaterialOperationResult.NotFound, null);
            if (lesson.Course.TeacherId != teacherId)
                return (MaterialOperationResult.Forbidden, null);
            var material = new Material
            {
                LessonId = lessonId,
                FileName = dto.FileName,
                FileUrl = dto.FileUrl,
                FileType = dto.FileType,
            };
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
            var createdMaterial = await GetMaterialByIdAsync(material.Id);
            return (MaterialOperationResult.Success,createdMaterial);
        }


        public async Task<MaterialOperationResult> DeleteMaterialAsync(int id, int teacherId)
        {
            var material = await _context.Materials
                .Include(m => m.Lesson)
                    .ThenInclude(l => l.Course)
                        .FirstOrDefaultAsync(m => m.Id == id);
            if(material == null) 
                return MaterialOperationResult.NotFound;
            if (material.Lesson.Course.TeacherId != teacherId)
                return MaterialOperationResult.Forbidden;
            _context.Materials.Remove(material);
            await _context.SaveChangesAsync();
            return MaterialOperationResult.Success;
        }

        public async Task<MaterialResponseDto?> GetMaterialByIdAsync(int id)
        {
            return await _context.Materials
                .Where(m => m.Id == id)
                .Select(ProjectToDto)
                .FirstOrDefaultAsync();
        }

        public async Task<List<MaterialResponseDto>> GetMaterialsByLessonAsync(int lessonId)
        {
            return await _context.Materials
                .Where(m => m.LessonId == lessonId)
                .Select(ProjectToDto)
                .ToListAsync();
        }

        public async Task<MaterialOperationResult> UpdateMaterialAsync(int id, UpdateMaterialDto dto, int teacherId)
        {
            var material = await _context.Materials
                .Include(m => m.Lesson)
                    .ThenInclude(l => l.Course)
                        .FirstOrDefaultAsync(m => m.Id == id);
            if (material == null)
                return MaterialOperationResult.NotFound;
            if (material.Lesson.Course.TeacherId != teacherId)
                return MaterialOperationResult.Forbidden;

            material.FileName = dto.FileName;
            material.FileUrl = dto.FileUrl;
            material.FileType = dto.FileType;
            await _context.SaveChangesAsync();
            return MaterialOperationResult.Success;



        }

        private static readonly System.Linq.Expressions.Expression<Func<Material, MaterialResponseDto>> ProjectToDto = m => new MaterialResponseDto
        {
            Id = m.Id,
            LessonId = m.LessonId,
            FileName = m.FileName,
            FileUrl = m.FileUrl,
            FileType = m.FileType
        };
    }
}
