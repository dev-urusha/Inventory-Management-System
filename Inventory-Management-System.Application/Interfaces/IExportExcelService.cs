﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory_Management_System.Application.Interfaces
{
    public interface IExportExcelService
    {
        byte[] CreateFile<T>(List<T> source);
    }
}
