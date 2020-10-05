namespace CurrencyService.Common
{
    using CurrencyService.Dto;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;

    public static class Helper
    {
        public static IEnumerable<T> MultipleSort<T>(this IEnumerable<T> data, List<GridSortDto> gridsorts)
        {
            var sortExpressions = new List<Tuple<string,
                string>>();
            for (int i = 0; i < gridsorts.Count(); i++)
            {
                var fieldName = gridsorts[i].Field.Trim();
                var sortOrder = (gridsorts[i].Dir.Length > 1) ?
                    gridsorts[i].Dir.Trim().ToLower() : "asc";
                sortExpressions.Add(new Tuple<string, string>(fieldName, sortOrder));
            }

            if ((sortExpressions == null) || (sortExpressions.Count <= 0))
            {
                return data;
            }

            IEnumerable<T> query = from item in data select item;
            IOrderedEnumerable<T> orderedQuery = null;
            for (int i = 0; i < sortExpressions.Count; i++)
            {
                var index = i;
                Func<T, object> expression = item => item.GetType()
                 .GetProperty(sortExpressions[index].Item1)
                 .GetValue(item, null);
                if (sortExpressions[index].Item2 == "asc")
                {
                    orderedQuery = (index == 0) ? query.OrderBy(expression) :
                        orderedQuery.ThenBy(expression);
                }
                else
                {
                    orderedQuery = (index == 0) ? query.OrderByDescending(expression) :
                        orderedQuery.ThenByDescending(expression);
                }
            }
            query = orderedQuery;
            return query;
        }

        public static List<GridSortDto> ConvertSortModel(string sortQuery)
        {
            var result = new List<GridSortDto>();

            var sortList = sortQuery.Split(", ");
            foreach (var item in sortList)
            {
                var fields = item.Split(' ');
                if (fields.Length > 1 && !string.IsNullOrEmpty(fields[0]))
                {
                    result.Add(new GridSortDto
                    {
                        Field = fields[0],
                        Dir = item.ToLower().Contains("asc") ? "asc" : "desc"
                    });
                }
            }
            return result;
        }

        private static void SaveWithoutDeclaration(this XDocument doc, string FileName)
        {
            using (var fs = new StreamWriter(FileName))
            {
                fs.Write(doc.ToString());
            }
        }
    }
}
