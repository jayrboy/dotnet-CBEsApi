using CBEsApi.Data;
using CBEsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace CBEsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CBEsController : ControllerBase


    {
        private CbesManagementContext _db = new CbesManagementContext();

        /// <summary>
        /// Get All CBEs
        /// </summary>
        [HttpGet(Name = "GetCBEs")]
        public ActionResult GetCBEs()
        {
            List<Cbe> cbes = Cbe.GetAll(_db);
            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = cbes
            });
        }

        /// <summary>
        /// Get CBE By ID
        /// </summary>
        [HttpGet("{id}", Name = "GetCBE")]
        public ActionResult GetCBE(int id)
        {
            Cbe cbe = Cbe.GetById(_db, id);
            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = cbe
            });
        }

        /// <summary>
        /// Delete CBE By ID
        /// </summary>
        [HttpDelete("{id}", Name = "DeleteCBE")]
        public ActionResult<Response> DeleteCBE(int id)
        {
            try
            {
                Cbe cbe = Cbe.Delete(_db, id);

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
        /// Create new a CBE
        /// </summary>
        [HttpPost(Name = "PostCBE")]
        public ActionResult<Response> PostCBE(Cbe cbe)
        {
            return Ok(new Response
            {
                Status = 201,
                Message = "CBE Saved",
                Data = cbe
            });
        }

        /// <summary>
        /// Create new maturity
        /// </summary>
        [HttpPost("maturity", Name = "PostCbeMaturity")]
        public ActionResult<Response> PostCbeMaturity(CbesMaturity cbesMaturity)
        {
            return Ok(new Response
            {
                Status = 201,
                Message = "CBE Maturity Saved",
                Data = cbesMaturity
            });
        }

        /// <summary>
        /// Create new supervisor
        /// </summary>
        [HttpPost("supervisor", Name = "PostCbeSupervisor")]
        public ActionResult<Response> PostCbeSupervisor(CbeswithSupervisor cbeSupervisor)
        {
            return Ok(new Response
            {
                Status = 201,
                Message = "CBE Maturity Saved",
                Data = cbeSupervisor
            });
        }

        /// <summary>
        /// Get All CBEs History
        /// </summary>
        [HttpGet("history", Name = "GetAllHistory")]
        public ActionResult<Response> GetAllHistory(List<Cbe> cbeHistory)
        {
            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = cbeHistory
            });
        }

        /// <summary>
        /// Get CBE History
        /// </summary>
        [HttpGet("history/{id}", Name = "GetHistory")]
        public ActionResult<Response> GetHistory(int id)
        {
            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = id
            });
        }

        /// <summary>
        /// Get All Bin
        /// </summary>
        [HttpGet("bin", Name = "GetCBEsBin")]
        public ActionResult<Response> GetRoleBin([FromBody] List<CbesRole> role)
        {
            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = role
            });
        }

        /// <summary>
        /// Cancel Delete Bin By ID
        /// </summary>
        [HttpPut("bin/CancelDelete/{id}", Name = "UpdateDeleteCBE")]
        public ActionResult<Response> UpdateDeleteCBE(int id)
        {
            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = id
            });
        }

        /// <summary>
        /// Last Delete Bin By ID
        /// </summary>
        [HttpDelete("bin/LastDelete/{id}", Name = "UpdateLastDeleteCBE")]
        public ActionResult<Response> UpdateLastDeleteCBE(int id)
        {
            return Ok(new Response
            {
                Status = 200,
                Message = "Success",
                Data = id
            });
        }
    }
}