﻿using System.IO;
using System.Threading.Tasks;
using PhotoProcessor.Functions.Models;

namespace PhotoProcessor.Functions.Data
{
    public interface IBlobContext
    {
        Task<GeneralStatusEnum> Write(MemoryStream stream, string fileName);
    }
}