using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO.Compression;
using System.Reflection;

using Newtonsoft.Json;

namespace On4Net.Extensions.Common;

/// <summary>
/// This Extension Helper for developers
/// </summary>
public static class ObjectExtension
{
    /// <summary>
    /// Compress data byte array
    /// </summary>
    /// <param name="data">data as byte array </param>
    /// <returns> byte  array</returns>
    public static byte[] Compress(this byte[] data)
    {
        using (var compressedStream = new MemoryStream())
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Compress))
        {
            zipStream.Write(data, 0, data.Length);
            zipStream.Close();
            return compressedStream.ToArray();
        }
    }

    /// <summary>
    /// Decompress data byte array
    /// </summary>
    /// <param name="data">data as byte array </param>
    /// <returns> byte  array</returns>
    public static byte[] Decompress(this byte[] data)
    {
        using (var compressedStream = new MemoryStream(data))
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var resultStream = new MemoryStream())
        {
            zipStream.CopyTo(resultStream);
            return resultStream.ToArray();
        }
    }

    /// <summary>
    /// Decompress data byte array and return memory stream
    /// </summary>
    /// <param name="data"></param>
    /// <returns>memory stream</returns>
    public static MemoryStream DecompressMemoryStream(this byte[] data)
    {
        using (var compressedStream = new MemoryStream(data))
        using (var zipStream = new GZipStream(compressedStream, CompressionMode.Decompress))
        using (var resultStream = new MemoryStream())
        {
            zipStream.CopyTo(resultStream);
            return resultStream;
        }
    }

    /// <summary>
    /// Number generator from GUID 
    /// </summary>
    /// <param name="guid">Guid</param>
    /// <returns>string</returns>
    public static string GetNumberByGuid(this Guid guid)
    {
        byte[] buffer = guid.ToByteArray();
        var FormNumber = BitConverter.ToUInt32(buffer, 0) ^ BitConverter.ToUInt32(buffer, 4) ^ BitConverter.ToUInt32(buffer, 8) ^ BitConverter.ToUInt32(buffer, 12);
        return FormNumber.ToString();
    }
    /// <summary>
    /// Number generator from GUID as string
    /// </summary>
    /// <param name="arg">string GUID</param>
    /// <returns>string</returns>
    public static string GetNumberByGuid(this string arg)
    {
        var id = arg.ToGuid();
        return GetNumberByGuid(id);
    }

    /// <summary>
    /// Convert List to Data Table
    /// </summary>
    /// <typeparam name="T">type of data as class</typeparam>
    /// <param name="data">data list</param>
    /// <param name="tableName">data table name</param>
    /// <returns>Data table</returns>
    public static DataTable ConvertToDatatable<T>(this IList<T> data, string tableName)
    {
        PropertyDescriptorCollection props =
            TypeDescriptor.GetProperties(typeof(T));
        DataTable table = new DataTable(tableName);
        for (int arg = 0; arg < props.Count; arg++)
        {
            PropertyDescriptor prop = props[arg];
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(
        prop.PropertyType) ?? prop.PropertyType);
        }
        object[] values = new object[props.Count];
        foreach (T item in data)
        {
            for (int arg = 0; arg < values.Length; arg++)
            {
                values[arg] = props[arg].GetValue(item);
            }
            table.Rows.Add(values);
        }
        return table;
    }

    /// <summary>
    /// map property class as same property
    /// </summary>
    /// <typeparam name="T">Parent Class</typeparam>
    /// <param name="contractData">Class Destination</param>
    /// <param name="enjectData">Class Source</param>
    /// <param name="isCheckValue"></param>
    public static void BindDataClass<T>(this T contractData, T enjectData, bool isCheckValue = false)
    {
        foreach (PropertyInfo item in contractData.GetType().GetProperties())
        {
            if (!item.CanRead)
                continue;
            var val = item.GetValue(contractData, null);
            if (isCheckValue)
            {
                if (item.PropertyType.IsGenericType
            && item.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (object.Equals(null, val))
                        continue;
                }


                if (object.Equals(item.PropertyType.DefaultValue(), val))
                    continue;


            }
            try
            {
                var property = enjectData.GetType().GetProperty(item.Name);
                var pval = item.GetValue(enjectData, null);
                if (property != null)
                {

                    if (isCheckValue)
                    {
                        if (item.PropertyType.IsGenericType
                    && item.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                        {
                            if (object.Equals(null, pval))
                                continue;
                        }


                        if (!object.Equals(item.PropertyType.DefaultValue(), pval))
                            continue;


                    }


                    if (property.CanWrite)
                        property.SetValue(contractData, pval, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Write(ex.ToString());
            }



        }
    }

    /// <summary>
    /// return default value
    /// </summary>
    /// <param name="type">type of staruct</param>
    /// <returns>value</returns>
    private static object? DefaultValue(this Type type)
    {

        if (type.Name == "Guid")
            return Guid.Empty;

        if (type == typeof(int) || type == typeof(Int32) || type == typeof(Int16) || type == typeof(long) || type == typeof(short) || type == typeof(float) || type == typeof(decimal))
            return default;

        return default;
    }


    /// <summary>
    /// craate instance as assembly name
    /// </summary>
    /// <param name="ClassName">object name</param>
    /// <returns>object</returns>
    public static object CreateInstance(this string ClassName)
    {
        Type type = Type.GetType(ClassName);

        var item = Activator.CreateInstance(type);
        return item;

    }

    /// <summary>
    /// create instance as class
    /// </summary>
    /// <typeparam name="T">type of object</typeparam>
    /// <param name="ClassName"></param>
    /// <returns>object</returns>
    public static T CreateInstance<T>(this T ClassName) where T : class
    {
        Type type = typeof(T); ;

        var item = Activator.CreateInstance(type) as T;
        return item;

    }

    public static DateTime ToDateTime(this long unixTime)
    {
        DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
        return dtDateTime;
    }

    public static long GetTime()
    {
        long epochTicks = new DateTime(1970, 1, 1).Ticks;
        long nowTicks = DateTime.UtcNow.Ticks;
        long tmStamp = ((nowTicks - epochTicks) / TimeSpan.TicksPerSecond);
        return tmStamp;
    }

    public static T ToEnum<T>(this string enumString) where T : Enum
    {
        if (Enum.IsDefined(typeof(T), enumString))
            return (T)Enum.Parse(typeof(T), enumString);
        throw new ArgumentException();
    }

    public static string ToJsonString<T>(this T @object,
         JsonSerializerSettings jsonSerializerSettings = null,
      Formatting  formatting= Formatting.None  )
    {
        if(jsonSerializerSettings == null)
            jsonSerializerSettings = new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore};
        if (@object == null)
        {
            throw new ArgumentNullException("object");
        }

        return JsonConvert.SerializeObject(@object,formatting, jsonSerializerSettings );
    }

    public static T FromJsonString<T>(this string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            throw new ArgumentNullException("json");
        }

        return JsonConvert.DeserializeObject<T>(json);
    }
}