using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using CBEsApi.Data;
using CBEsApi.Dtos.CBEsRoleDto;


namespace CBEsApi.Models
{
    public class CbesRoleMetadata
    {
        // Define metadata properties if needed
    }

    [MetadataType(typeof(CbesRoleMetadata))]
    public partial class CbesRole
    {
        public static List<CbesRoleDto> GetAll(CbesManagementContext db)
        {
            List<CbesRoleDto> roles = db.CbesRoles
                                        .Where(q => q.IsDeleted != true)
                                        .Select(r => new CbesRoleDto
                                        {
                                            Id = r.Id,
                                            Name = r.Name,
                                            UpdateDate = r.UpdateDate,
                                            IsDeleted = r.IsDeleted,
                                            IsLastDelete = r.IsLastDelete,
                                            CreateBy = r.CreateBy,
                                            UpdateBy = r.UpdateBy,
                                        })
                                        .ToList();
            return roles;
        }

        //TODO: Get Role Permissions and Users
        public static CbesRoleDto GetRole(CbesManagementContext db, int id)
        {
            CbesRole? role = db.CbesRoles
                                .Where(q => q.Id == id && q.IsDeleted != true)
                                .Include(q => q.CbesRoleWithPermissions)
                                    .ThenInclude((q) => q.Permission)
                                .Include(q => q.CbesUserWithRoles.Where(a => a.IsDeleted != true))
                                    .ThenInclude((q) => q.User).Where(a => a.IsDeleted != true)
                                .FirstOrDefault();

            if (role == null)
            {
                return new CbesRoleDto();
            }

            CbesRoleDto roleDto = new CbesRoleDto
            {
                Id = role.Id,
                Name = role.Name,
                UpdateDate = role.UpdateDate,
                IsDeleted = role.IsDeleted,
                IsLastDelete = role.IsLastDelete,
                CreateBy = role.CreateBy,
                UpdateBy = role.UpdateBy,
                CbesRoleWithPermissions = role.CbesRoleWithPermissions
                                            .Select(p => new CbesRoleWithPermissionDto
                                            {
                                                Id = p.Id,
                                                IsChecked = p.IsChecked,
                                                IsDeleted = p.IsDeleted,
                                                RoleId = p.RoleId,
                                                PermissionId = p.PermissionId,
                                                Permission = new CbesPermissionDto
                                                {
                                                    Id = p.Permission.Id,
                                                    Name = p.Permission.Name,
                                                },
                                                CreateDate = p.CreateDate,
                                                UpdateDate = p.UpdateDate,
                                                CreateBy = p.CreateBy,
                                                UpdateBy = p.UpdateBy
                                            }).ToList(),
                CbesUserWithRole = role.CbesUserWithRoles.Select(u => new CbesUserWithRoleDto
                {
                    ID = u.Id,
                    IsDeleted = u.IsDeleted,
                    CreateDate = u.CreateDate,
                    UpdateDate = u.UpdateDate,
                    CreateBy = u.CreateBy,
                    UpdateBy = u.UpdateBy,
                    RoleId = u.RoleId,
                    UserId = u.UserId,
                    User = new CbesUserDto
                    {
                        Id = u.User.Id,
                        Fullname = u.User.Fullname,
                        Username = u.User.Username,
                        IsDeleted = u.User.IsDeleted,
                    }
                }).ToList()

            };

            return roleDto;
        }


        public static CbesRole Create(CbesManagementContext db, CbesRole role)
        {
            db.CbesRoles.Add(role);
            db.SaveChanges();

            // Update RoleId in permissions after saving the role
            foreach (var permission in role.CbesRoleWithPermissions)
            {
                permission.RoleId = role.Id;
            }
            db.SaveChanges();

            return role;
        }

        public static CbesRole GetRoleByIdAndUser(CbesManagementContext db, int id)
        {
            CbesRole? role = db.CbesRoles.Where(q => q.Id == id).Include((u) => u.CbesUserWithRoles).ThenInclude(q => q.User).FirstOrDefault();

            if (role == null)
            {
                return null; // หรือส่งคืน CbesRoleDto ว่าง
            }


            return role;
        }

        //  Delete ID
        public static CbesRole Delete(CbesManagementContext db, int id, int updateBy)
        {
            CbesRole cbe = GetRoleByIdAndUser(db, id);

            cbe.UpdateDate = DateTime.Now;
            cbe.IsDeleted = true;
            cbe.UpdateBy = updateBy;

            db.Entry(cbe).State = EntityState.Modified;
            db.SaveChanges();

            return cbe;
        }

        //  Cancel Delete ID
        public static CbesRole cancelDelete(CbesManagementContext db, int id, int updateBy)
        {
            CbesRole cbe = GetRoleByIdAndUser(db, id);
            cbe.UpdateDate = DateTime.Now;
            cbe.IsDeleted = false;
            cbe.UpdateBy = updateBy;

            db.Entry(cbe).State = EntityState.Modified;
            db.SaveChanges();

            return cbe;
        }
        //  Last Delete ID
        public static CbesRole lastDelete(CbesManagementContext db, int id, int updateBy)
        {
            CbesRole cbe = GetRoleByIdAndUser(db, id);
            cbe.UpdateDate = DateTime.Now;
            cbe.IsLastDelete = true;
            cbe.UpdateBy = updateBy;

            db.Entry(cbe).State = EntityState.Modified;
            db.SaveChanges();

            return cbe;
        }

        public static List<CbesRole> GetAllBin(CbesManagementContext db)
        {
            List<CbesRole> roles = db.CbesRoles.Where(q => q.IsDeleted == true && q.IsLastDelete == false).ToList();
            return roles;
        }

        public static CbesRole GetById(CbesManagementContext db, int roleId)
        {
            var role = db.CbesRoles
                         .Include(q => q.CbesRoleWithPermissions)
                          .ThenInclude((q) => q.Permission)
                         .FirstOrDefault(q => q.Id == roleId);

            if (role == null)
            {
                throw new ArgumentException("Role not found");
            }

            return role;
        }

        public static CbesRole Update(CbesManagementContext db, CbesRole role)
        {
            // Attach role to the context if not already attached
            if (!db.CbesRoles.Local.Any(r => r.Id == role.Id))
            {
                db.CbesRoles.Attach(role);
            }

            // Update role information
            db.Entry(role).State = EntityState.Modified;

            try
            {
                // Save changes to the database
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // Handle concurrency issues
                // Log the exception or handle as needed
                throw new Exception("Concurrency error occurred while updating role.", ex);
            }
            catch (DbUpdateException ex)
            {
                // Handle other update errors
                // Log the exception or handle as needed
                throw new Exception("Error occurred while updating role.", ex);
            }

            return role;
        }
    }
}
