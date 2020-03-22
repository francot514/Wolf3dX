using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nexxt.Common.Enums
{
    public enum DecelerationType
    {
        Slow = 3,
        Normal = 2,
        Fast = 1
    };

    public enum SummingMethod
    {
        WeightedAverage,
        Prioritized,
        Dithered
    };
}
