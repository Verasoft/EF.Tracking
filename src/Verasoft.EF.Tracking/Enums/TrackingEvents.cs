﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Verasoft.EF
{
    public enum TrackingEvent
    {
        BeforeSaveChanges,
        AfterSaveChanges,
        BeforeModeling,
        AfterModeling
    }
}
