using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoProcessor.Functions.Models
{
    public enum GeneralStatusEnum
    {
        Default = 0,
        Ok = 1,
        BadRequest = 2,
        ServerError = 4,
        Fail = 5
    }
}
