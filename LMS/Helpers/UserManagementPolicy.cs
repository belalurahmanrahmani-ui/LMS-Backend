using LMS.Enums;

namespace LMS.Helpers
{
    public static class UserManagementPolicy
    {
        // آیا actor می‌تواند این کاربر را حذف/غیرفعال/ویرایش کند؟
        public static bool CanManage(
            int actorId, int targetId, UserRole targetRole, int primaryAdminId)
        {
            if (targetId == primaryAdminId)
                return false;                 

            if (actorId == primaryAdminId)
                return true;                  

            
            return targetRole is UserRole.Teacher or UserRole.Student;
        }

        
        public static bool CanAssignRole(
            int actorId, UserRole role, int primaryAdminId)
        {
            return role != UserRole.Admin || actorId == primaryAdminId;
        }
    }
}