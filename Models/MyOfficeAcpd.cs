using System;
using System.Collections.Generic;

namespace PoChunSu_BackendTest_MidLevel.Models;

public partial class MyOfficeAcpd
{
    public string AcpdSid { get; set; } = null!;

    public string? AcpdCname { get; set; }

    public string? AcpdEname { get; set; }

    public string? AcpdSname { get; set; }

    public string? AcpdEmail { get; set; }

    public byte? AcpdStatus { get; set; }

    public bool? AcpdStop { get; set; }

    public string? AcpdStopMemo { get; set; }

    public string? AcpdLoginId { get; set; }

    public string? AcpdLoginPwd { get; set; }

    public string? AcpdMemo { get; set; }

    public DateTime? AcpdNowDateTime { get; set; }

    public string? AcpdNowId { get; set; }

    public DateTime? AcpdUpddateTime { get; set; }

    public string? AcpdUpdid { get; set; }
}
