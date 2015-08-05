﻿using KellermanSoftware.CompareNetObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dangl.Test.Common
{
    public static class Comparator
    {
        public static CompareLogic GetCompareLogic()
        {
            return new CompareLogic()
            {
                Config = new ComparisonConfig()
                {
                    CompareChildren = true,
                    CompareFields = true,
                    CompareReadOnly = true,
                    ComparePrivateFields = true,
                    ComparePrivateProperties = true,
                    CompareProperties = true,
                    CompareStaticFields = false,
                    CompareStaticProperties = false,
                    MaxMillisecondsDateDifference = 50,
                    MembersToIgnore = new List<string> { "GUID" }
                }
            };
        }
    }
}