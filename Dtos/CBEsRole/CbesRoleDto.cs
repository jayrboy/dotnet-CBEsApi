using CBEsApi.Models;

namespace CBEsApi.Dtos.CBEsRole
{
    public class CbesRoleDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsLastDelete { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        public List<CbesRoleWithPermissionDto>? CbesRoleWithPermissions { get; set; }
        public List<CbesRoleUserDto>? CbesUserWithRole { get; set; }
    }
}