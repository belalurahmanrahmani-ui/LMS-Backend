using LMS.DTOs.Material;
using LMS.Enums;

namespace LMS.Services.Interface
{
    public interface IMaterialService
    {
        Task<List<MaterialResponseDto>> GetMaterialsByLessonAsync(int lessonId);
        Task<MaterialResponseDto?> GetMaterialByIdAsync(int id);
        Task<(MaterialOperationResult Result, MaterialResponseDto? Material)> CreateMaterialAsync(int lessonId, CreateMaterialDto dto, int teacherId);
        Task<MaterialOperationResult> UpdateMaterialAsync(int id, UpdateMaterialDto dto, int teacherId);
        Task<MaterialOperationResult> DeleteMaterialAsync(int id, int teacherId);
    }
}