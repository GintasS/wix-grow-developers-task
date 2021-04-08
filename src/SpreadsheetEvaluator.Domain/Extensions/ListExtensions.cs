using System.Collections.Generic;

namespace SpreadsheetEvaluator.Domain.Extensions
{
    public static class ListExtensions
    {
        public static bool HasUniqueElements<T>(this List<T> list) where T : struct
        {
            if (list == null || list.Count <= 1)
            {
                return true;
            }

            var firsType = list[0];
            for (var i = 1; i < list.Count; i++)
            {
                if (list[i].Equals(firsType) == false)
                {
                    return false;
                }

                firsType = list[i];
            }

            return true;
        }
    }
}
