using System.ComponentModel.DataAnnotations;
using CBEsApi.Data;
using CBEsApi.Dtos.CBEsDto;

namespace CBEsApi.Models
{
    public class CbesLogMetadata
    {

    }

    [MetadataType(typeof(CbesLogMetadata))]
    public partial class CbeLog
    {
        public static Cbe GetById(CbesManagementContext db)
        {
            Cbe? cbe = db.Cbes.Where(q => q.IsDeleted != true).Select(s => new Cbe
            {
                Id = s.Id,
                ThaiName = s.ThaiName,
                EngName = s.EngName,
                ShortName = s.ShortName,
                Detail = s.Detail,
                CreateDate = s.CreateDate,
                UpdateDate = s.UpdateDate,
                IsDeleted = s.IsDeleted,
                IsLastDelete = s.IsLastDelete,
                CbesProcesses = s.CbesProcesses
            }).FirstOrDefault();

            return cbe;
        }
    }

}