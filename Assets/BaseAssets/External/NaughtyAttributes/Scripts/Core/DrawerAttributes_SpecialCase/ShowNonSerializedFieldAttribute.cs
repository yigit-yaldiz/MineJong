﻿using System;

namespace BaseAssets
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ShowNonSerializedFieldAttribute : SpecialCaseDrawerAttribute
    {
    }
}
