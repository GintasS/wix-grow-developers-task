using SpreadsheetEvaluator.Domain.Models.Enums;
using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Extensions
{
    public static class ListExtensions
    {
        public static bool HasUniqueElements(this List<CellType> list)
        {
            if (list == null || list.Count <= 1)
            {
                return true;
            }

            var firsType = list[0];
            for (var i = 1; i < list.Count; i++)
            {
                if ((int)list[i] != (int)firsType)
                {
                    return false;
                }

                firsType = list[i];
            }

            return true;
        }
    }
}
