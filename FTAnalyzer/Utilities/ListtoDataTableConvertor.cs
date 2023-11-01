using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace FTAnalyzer.Utilities
{
    public class ListtoDataTableConvertor
    {
        public DataTable ToDataTable<T>(List<T> items)
        {
            DataTable dataTable = new DataTable(typeof(T).Name);
            //Get all the properties
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in Props)
                dataTable.Columns.Add(prop.Name);
            if (items != null)
            {
                foreach (T item in items)
                {
                    var values = new object[dataTable.Columns.Count];
                    for (int i = 0; i < Props.Length; i++)
                        values[i] = Props[i].GetValue(item, null);
                    dataTable.Rows.Add(values);
                }
            }
            //put a breakpoint here and check datatable
            return dataTable;
        }

        //public DataTable ToDataTable<T>(this IList<T> data)
        //{
        //    PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
        //    DataTable table = new DataTable();
        //    foreach (PropertyDescriptor prop in properties)
        //        table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        //    foreach (T item in data)
        //    {
        //        DataRow row = table.NewRow();
        //        foreach (PropertyDescriptor prop in properties)
        //            row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
        //        table.Rows.Add(row);
        //    }
        //    return table;
        //}
    }
}
