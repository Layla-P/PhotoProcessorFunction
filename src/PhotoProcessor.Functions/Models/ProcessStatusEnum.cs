using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoProcessor.Functions.Models
{
    public enum ProcessStatusEnum
    {
        Default = 0,
        Uploaded = 1,
        Processing = 2,
        Completed = 4,
        Failed = 5
    }
}
