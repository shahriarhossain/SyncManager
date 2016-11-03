using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace SyncManager.Helper.Extension
{
    public static class IEnumerableExtension
    {
        public static DataTable AsDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            var table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    //If the value is empty then make it Nullable 
                    var o = prop.GetValue(item);
                    if (o != null && String.IsNullOrEmpty(o.ToString()))
                    {
                        row[prop.Name] = DBNull.Value;
                    }
                    else
                    {
                        //if the value is * then make it Nullable
                        var value = prop.GetValue(item);
                        if (value != null && String.Equals(value.ToString(), "*", StringComparison.InvariantCultureIgnoreCase))
                        {
                            row[prop.Name] = DBNull.Value;
                        }
                        else
                        {
                            row[prop.Name] = prop.GetValue(item);
                        }
                    }
                }
                //row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }
            return table;
        }

    }
}
