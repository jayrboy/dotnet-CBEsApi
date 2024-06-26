﻿using System;
using System.Collections.Generic;

namespace CBEsApi.Models;

public partial class CbesLogHeader
{
    public int Id { get; set; }

    public int? Round { get; set; }

    public string? Remark { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public bool? IsDeleted { get; set; }

    public int? CbesLogTypeId { get; set; }

    public int? CbesLogId { get; set; }

    public virtual CbesLog? CbesLog { get; set; }

    public virtual CbesLogType? CbesLogType { get; set; }

    public virtual ICollection<CbesLog> CbesLogs { get; set; } = new List<CbesLog>();

    public virtual ICollection<CbesMaturityLog> CbesMaturityLogs { get; set; } = new List<CbesMaturityLog>();
}
