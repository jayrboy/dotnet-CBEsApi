using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CBEsApi.Data;
using CBEsApi.Models;
using CBEsApi.Dtos.CBEsRole;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace CBEsApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CBEsRoleController : ControllerBase
    {
        private readonly ILogger<CBEsRoleController> _logger;

        public CBEsRoleController(ILogger<CBEsRoleController> logger)
        {
            _logger = logger;
        }

        private CbesManagementContext _db = new CbesManagementContext();

        public class RequestRoleID
        {
            [Required]
            public int ID { get; set; }
        }

        [HttpGet(Name = "GetRoles")]
        public ActionResult GetRoles()
        {
            List<CbesRoleDto> roles = CbesRole.GetAll(_db);

            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = roles
            });
        }

        [HttpGet("{id}", Name = "GetRole")]
        public ActionResult<Response> GetRole(int id)
        {
            CbesRoleDto role = CbesRole.GetRole(_db, id);

            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = role
            });
        }

        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/CBEsRole
        ///     
        ///     {
        ///         "name": "บทบาททดสอบ",
        ///         "createBy": 1,
        ///         "updateBy": 2,
        ///         "cbesRoleWithPermission": [
        ///             {
        ///                 "isCheck": true,
        ///                 "roleId": = 1,
        ///                 "permissionId: 1,
        ///                 "permission": { "id": 1, "name": "กำหนดสิทธิ์การใช้งานระบบ และกลุ่มผู้ใช้งาน"}
        ///             },
        ///             {
        ///                 "isCheck": true,
        ///                 "roleId": = 1,
        ///                 "permissionId: 2,
        ///                 "permission": { "id": 2, "name": "ประวัติการใช้งาน"}
        ///             }
        ///         ]
        ///     }
        ///     
        /// </remarks>
        [HttpPost(Name = "PostRolePermission")]
        public ActionResult<Response> PostRolePermission(CbesRoleDto createRole)
        {
            var userClaimsString = User.FindFirst("ID")?.Value;
            int userClaims = Convert.ToInt32(userClaimsString);

            CbesRole role = new CbesRole
            {
                Name = createRole.Name,
                CreateBy = userClaims,
                UpdateBy = userClaims,
            };

            foreach (var p in createRole.CbesRoleWithPermissions)
            {
                CbesRoleWithPermission rolePermissions = new CbesRoleWithPermission
                {
                    IsChecked = p.IsChecked,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    RoleId = role.Id, // This will be updated after saving role
                    IsDeleted = false,
                    PermissionId = p.PermissionId,
                    CreateBy = userClaims,
                    UpdateBy = userClaims,

                };

                role.CbesRoleWithPermissions.Add(rolePermissions);
            }

            role = CbesRole.Create(_db, role);

            return Ok(new Response
            {
                Status = 201,
                Message = "Role and Permissions Saved",
                Data = role
            });
        }

        /// <summary>
        /// Update role with permissions.
        /// </summary>
        /// <remarks>
        /// ตัวอย่างของคำขอ:
        ///
        ///     PUT /api/CbesRole/PutRolePermission
        ///     {
        ///         "id": 4,
        ///         "name": "บทบาททดสอบ",
        ///         "updateDate": "2024-07-09T09:37:16.647",
        ///         "isDeleted": false,
        ///         "isLastDelete": false,
        ///         "createBy": 1,
        ///         "updateBy": 1,
        ///         "cbesRoleWithPermissions": [
        ///             {
        ///                 "id": 0,
        ///                 "isChecked": false,
        ///                 "isDeleted": false,
        ///                 "roleId": 0,
        ///                 "permissionId": 1,
        ///                 "permission": {
        ///                     "id": 1,
        ///                     "name": "กำหนดสิทธิ์การใช้งานระบบ และกลุ่มผู้ใช้งาน"
        ///                 },
        ///                 "createDate": "2024-07-05T09:53:46.533",
        ///                 "updateDate": "2024-07-09T09:37:16.643",
        ///                 "createBy": 1,
        ///                 "updateBy": 1
        ///             },
        ///             {
        ///                 "id": 0,
        ///                 "isChecked": false,
        ///                 "isDeleted": false,
        ///                 "roleId": 0,
        ///                 "permissionId": 2,
        ///                 "permission": {
        ///                     "id": 2,
        ///                     "name": "ประวัติการใช้งาน"
        ///                 },
        ///                 "createDate": "2024-07-05T09:53:46.533",
        ///                 "updateDate": "2024-07-09T09:37:16.643",
        ///                 "createBy": 1,
        ///                 "updateBy": 1
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="updateRole">Object บทบาทที่อัปเดตพร้อมกับสิทธิ์</param>
        /// <returns>การตอบสนองที่บอกถึงความสำเร็จหรือความล้มเหลว</returns>
        /// <response code="200">ส่งคืนบทบาทที่อัปเดตพร้อมกับสิทธิ์</response>
        /// <response code="400">หากข้อมูลบทบาทหรือสิทธิ์ที่ให้มาไม่ถูกต้อง</response>
        /// <response code="404">หากไม่พบบทบาทที่ระบุ</response>
        /// <response code="500">หากเกิดข้อผิดพลาดภายในเซิร์ฟเวอร์</response>
        [HttpPut(Name = "PutRolePermission")]
        public ActionResult<Response> PutRolePermission(CbesRoleDto updateRole)
        {
            var userClaimsString = User.FindFirst("ID")?.Value;
            int userClaims = Convert.ToInt32(userClaimsString);

            // Fetch role as CbesRole
            CbesRole existingRole = CbesRole.GetById(_db, updateRole.Id);

            if (existingRole == null)
            {
                return NotFound(new Response
                {
                    Status = 404,
                    Message = "Role not found",
                });
            }

            existingRole.Name = updateRole.Name;
            existingRole.UpdateBy = userClaims;
            existingRole.UpdateDate = DateTime.Now;

            // Update existing permissions
            foreach (var permission in updateRole.CbesRoleWithPermissions)
            {
                var existingPermission = existingRole.CbesRoleWithPermissions
                    .FirstOrDefault(p => p.PermissionId == permission.PermissionId);

                if (existingPermission != null)
                {
                    // Update existing permission
                    existingPermission.IsChecked = permission.IsChecked;
                    existingPermission.UpdateDate = DateTime.Now;
                    existingPermission.UpdateBy = userClaims;

                    // Set RoleId for the permission (assuming existingRole.Id is correct)
                    existingPermission.RoleId = existingRole.Id;
                }
            }

            // Save updates to the database
            existingRole = CbesRole.Update(_db, existingRole);

            return Ok(new Response
            {
                Status = 200,
                Message = "Role and Permissions Updated",
                Data = existingRole
            });
        }

        [HttpDelete("delete/{id}", Name = "DeleteRole")]
        public ActionResult DeleteRole(int id)
        {
            try
            {
                CbesRole cbe = CbesRole.Delete(_db, id);

                return Ok(new Response
                {
                    Status = 200,
                    Message = "Success",
                    Data = cbe
                });
            }
            catch
            {
                // ถ้าไม่พบข้อมูล user ตาม id ที่ระบุ
                return NotFound(new Response
                {
                    Status = 404,
                    Message = "User not found",
                    Data = null
                });
            }
        }

        /// <summary>
        /// Update role with associated users and permissions.
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/CBEsRole/RoleWithUsers
        ///     
        ///     {
        ///         "id": 4,
        ///         "name": "บทบาททดสอบ",
        ///         "updateDate": "2024-07-09T09:37:16.647",
        ///         "isDeleted": false,
        ///         "isLastDelete": false,
        ///         "createBy": 1,
        ///         "updateBy": 1,
        ///         "cbesRoleWithPermissions": [],
        ///         "cbesUserWithRole": [
        ///             {
        ///                 "id": 0,
        ///                 "createDate": "2024-07-09T03:12:38.430Z",
        ///                 "updateDate": "2024-07-09T03:12:38.430Z",
        ///                 "isDeleted": false,
        ///                 "createBy": 0,
        ///                 "updateBy": 0,
        ///                 "roleId": 0,
        ///                 "userId": 4,
        ///                 "user": {
        ///                     "id": 4,
        ///                     "username": "natipong",
        ///                     "fullname": "เนติพงษ์",
        ///                     "isDeleted": false
        ///                 }
        ///             },
        ///             {
        ///                 "id": 0,
        ///                 "createDate": "2024-07-09T03:12:40.800Z",
        ///                 "updateDate": "2024-07-09T03:12:40.800Z",
        ///                 "isDeleted": false,
        ///                 "createBy": 0,
        ///                 "updateBy": 0,
        ///                 "roleId": 0,
        ///                 "userId": 5,
        ///                 "user": {
        ///                     "id": 5,
        ///                     "username": "chatchai",
        ///                     "fullname": "ชัชชาติ",
        ///                     "isDeleted": false
        ///                 }
        ///             }
        ///         ]
        ///     }
        /// </remarks>
        /// <param name="_db">Database context</param>
        /// <param name="updateRoleUsers">Updated role with users data</param>
        /// <returns>Response with status and message</returns>
        /// <response code="201">Users with Role Saved</response>
        /// <response code="400">Invalid request data</response>
        /// <response code="404">Role users not found</response>
        /// <response code="500">Internal server error</response>
        [HttpPut("RoleWithUsers", Name = "RoleUsers")]
        public ActionResult<Response> RoleUsers(CbesManagementContext _db, CbesRoleDto updateRoleUsers)
        {
            try
            {
                var userClaimsString = User.FindFirst("ID")?.Value;
                int userClaims = Convert.ToInt32(userClaimsString);

                // Get the old role including the associated users
                CbesRole? oldRole = _db.CbesRoles
                    .Include(r => r.CbesUserWithRoles)
                    .ThenInclude(u => u.User)
                    .FirstOrDefault(r => r.Id == updateRoleUsers.Id && r.IsDeleted != true);

                if (oldRole == null)
                {
                    return NotFound(new Response
                    {
                        Status = 404,
                        Message = "Role users not found",
                        Data = null
                    });
                }


                foreach (var user in updateRoleUsers.CbesUserWithRole)
                {

                    if (user.User.Id == 0)
                    {
                        return BadRequest(new Response
                        {
                            Status = 400,
                            Message = $"Invalid User ID: {user.User.Id}",
                            Data = null,
                        });
                    }

                    CbesUserWithRole? existingUserRole = oldRole.CbesUserWithRoles.FirstOrDefault(ur => ur.UserId == user.User.Id);

                    if (existingUserRole != null)
                    {
                        existingUserRole.IsDeleted = user.IsDeleted;
                        existingUserRole.UpdateBy = userClaims;
                        existingUserRole.UpdateDate = DateTime.UtcNow;
                        existingUserRole.RoleId = updateRoleUsers.Id;
                    }
                    else
                    {
                        CbesUserWithRole newUserRole = new CbesUserWithRole
                        {
                            CreateDate = DateTime.UtcNow,
                            UpdateDate = DateTime.UtcNow,
                            IsDeleted = false,
                            CreateBy = userClaims,
                            UpdateBy = userClaims,
                            UserId = user.User.Id,
                            RoleId = updateRoleUsers.Id,
                        };

                        oldRole.CbesUserWithRoles.Add(newUserRole);
                    }
                }


                _db.SaveChanges();

                return Ok(new Response
                {
                    Status = 201,
                    Message = "Users with Role Saved",
                    Data = oldRole
                });
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, new Response
                {
                    Status = 500,
                    Message = $"Database Update Error: {dbEx.Message}, Inner Exception: {dbEx.InnerException?.Message}",
                    Data = null,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Response
                {
                    Status = 500,
                    Message = $"Internal Server Error: {ex.Message}, Inner Exception: {ex.InnerException?.Message}",
                    Data = null,
                });
            }
        }

        [HttpGet("bin", Name = "GetRoleBin")]
        public ActionResult<Response> GetRoleBin()
        {
            List<CbesRole> roles = CbesRole.GetAllBin(_db);

            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = roles
            });
        }

        [HttpPut("bin/CancelDelete", Name = "UpdateDeleteRole")]
        public ActionResult<Response> UpdateDeleteRole(RequestRoleID request)
        {
            try
            {
                CbesRole cbe = CbesRole.cancelDelete(_db, request.ID);

                return Ok(new Response
                {
                    Status = 200,
                    Message = "Success",
                    Data = cbe
                });
            }
            catch
            {
                // ถ้าไม่พบข้อมูล user ตาม id ที่ระบุ
                return NotFound(new Response
                {
                    Status = 404,
                    Message = "User not found",
                    Data = null
                });
            }
        }

        [HttpDelete("bin/LastDelete/{id}", Name = "UpdateLastDeleteRole")]
        public ActionResult<Response> UpdateLastDeleteRole(int id)
        {
            try
            {
                CbesRole cbe = CbesRole.lastDelete(_db, id);

                return Ok(new Response
                {
                    Status = 200,
                    Message = "Success",
                    Data = cbe
                });
            }
            catch
            {
                // ถ้าไม่พบข้อมูล user ตาม id ที่ระบุ
                return NotFound(new Response
                {
                    Status = 404,
                    Message = "User not found",
                    Data = null
                });
            }
        }
    }
}