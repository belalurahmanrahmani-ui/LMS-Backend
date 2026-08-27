using LMS.Entities;
using System.ComponentModel.DataAnnotations;

namespace LMS.DTOs
{
    public class ChangeRoleDto
    {
        [Required(ErrorMessage = "Role is Required")]
        [EnumDataType(typeof(UserRole), ErrorMessage = "Invalid Role value")]
        public UserRole Role { get; set; }
    }
}
