using System.Globalization;

using static System.Formats.Asn1.AsnWriter;

namespace On4Net.Extensions.Common;

/// <summary>
/// This Extension Helper for developers
/// </summary>
public static class StringExtension
{
    /// <summary>
    /// check string
    /// </summary>
    /// <param name="obj"> string object</param>
    /// <param name="isNullOrWhiteSpace">check space</param>
    /// <returns></returns>
    public static bool IsNullOrEmpty(this string obj, bool isNullOrWhiteSpace = true)
    {
        if (!isNullOrWhiteSpace)
            return string.IsNullOrEmpty(obj);

        return string.IsNullOrWhiteSpace(obj);
    }

    /// <summary>
    /// check string
    /// </summary>
    /// <param name="obj"> string object</param>
    /// <param name="isNotNullOrWhiteSpace">check space</param>
    /// <returns></returns>
    public static bool IsNotNullOrEmpty(this string obj, bool isNullOrWhiteSpace = true)
    {
        if (!isNullOrWhiteSpace)
            return !string.IsNullOrEmpty(obj);

        return !string.IsNullOrWhiteSpace(obj);
    }
    /// <summary>
    /// Added zero to string for accounting system 
    /// </summary>
    /// <param name="InputStr"> string </param>
    /// <param name="OutLen">count of zero </param>
    /// <returns></returns>
    public static string Add_Zero_Before(this string InputStr, int OutLen)
    {
        string Output = "";
        InputStr = InputStr.Trim();
        for (int i = 1; i <= OutLen - InputStr.Length; i++)
        {
            Output += "0";
        }
        Output += InputStr;
        return Output;
    }
    /// <summary>
    /// Added zero to string for accounting system 
    /// </summary>
    /// <param name="InputStr"> string </param>
    /// <param name="OutLen">count of zero </param>
    /// <returns></returns>
    public static string Add_Zero_After(this string InputStr, int OutLen)
    {
        string Output = "";
        InputStr = InputStr.Trim();
        for (int i = 1; i <= OutLen - InputStr.Length; i++)
        {
            Output += "0";
        }
        Output = InputStr + Output;
        return Output;
    }
    /// <summary>
    /// Added space to string for accounting system 
    /// </summary>
    /// <param name="InputStr"> string </param>
    /// <param name="OutLen">count of space </param>
    /// <returns></returns>
    public static string Add_SpaceAfter(this string InStr, int OutLen)
    {
        string Output = "";
        InStr = InStr.Trim();
        for (int i = 1; i <= OutLen - InStr.Length; i++)
        {
            Output += " ";
        }
        Output = InStr + Output;
        return Output;

    }


    /// <summary>
    /// string to decimal null able
    /// </summary>
    /// <param name="arg">string</param>
    /// <returns>decimal</returns>
    public static System.Nullable<decimal> ToDecimalNull(this string arg)
    {
        decimal no = 0;

        if (decimal.TryParse(arg, out no))
        {
            return no;
        }
        return default;
    }
    /// <summary>
    /// string to decimal
    /// </summary>
    /// <param name="arg">string</param>
    /// <returns>decimal</returns>
    public static decimal ToDecimal(this string arg)
    {
        decimal? no = arg.ToDecimalNull();
        return no.HasValue ? no.Value : default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static Int16 ToInt16(this string arg)
    {
        Int16? no = arg.ToInt16Null();
        return no.HasValue ? no.Value : default;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public static System.Nullable<Int16> ToInt16Null(this string arg)
    {
        Int16 no = default(Int16);

        if (Int16.TryParse(arg, out no))
        {
            return no;
        }
        return default;
    }

    public static System.Nullable<byte> ToByteNull(this string arg)
    {
        byte no = 0;
        if (byte.TryParse(arg, out no))
        {
            return no;
        }
        return default;
    }
    public static byte ToByte(this string arg)
    {
        byte? no = arg.ToByteNull();
        return no.HasValue ? no.Value : default;
    }

    public static System.Nullable<int> ToInt32Null(this string arg)
    {
        int no = 0;

        if (int.TryParse(arg, out no))
        {
            return no;
        }
        return default;
    }
    public static int ToInt32(this string arg)
    {
        int? no = arg.ToInt32Null();
        return no.HasValue ? no.Value : default;
    }

    public static System.Nullable<Int64> ToInt64Null(this string arg)
    {
        Int64 no = default(Int64);

        if (Int64.TryParse(arg, out no))
        {
            return no;
        }

        return default;
    }
    public static Int64 ToInt64(this string arg)
    {
        Int64? no = arg.ToInt64Null();
        return no.HasValue ? no.Value : default;
    }

    public static System.Nullable<double> ToDoubleNull(this string arg)
    {
        double no = default(double);

        if (double.TryParse(arg, out no))
        {
            return no;
        }
        return default;
    }

    public static float ToFloat(this string arg)
    {
        float? no = arg.ToFloatNull();
        return no.HasValue ? no.Value : default;
    }

    public static System.Nullable<float> ToFloatNull(this string arg)
    {
        float no = default(float);

        if (float.TryParse(arg, out no))
        {
            return no;
        }
        return default;
    }

    public static double ToDouble(this string arg)
    {
        double? no = arg.ToDoubleNull();
        return no.HasValue ? no.Value : default;
    }

    public static Guid ToGuid(this string arg)
    {
        Guid? guid = arg.ToGuidNull();
        return guid.HasValue ? guid.Value : Guid.Empty;

    }

    public static Guid? ToGuidNull(this string arg)
    {
        Guid guid = Guid.Empty;

        if (Guid.TryParse(arg, out guid))
        {
            return guid;
        }
        return guid;
    }

    public static bool IsRtl(this string culture)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));
        var cultureInfo = CultureInfo.GetCultureInfo(culture);
        return cultureInfo.TextInfo.IsRightToLeft;
    }
    public static string GetDirection(this string culture)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));
        if (IsRtl(culture))
            return "rtl";
        return "ltr";
    }

    public static string GetLanguageCulture(this string culture)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));

        if (!culture.Split('-').Any())
            throw new ArgumentException("The culture not valid");
        return culture.Split('-')[0];
    }

    public static string GetCountryCulture(this string culture)
    {
        ArgumentNullException.ThrowIfNull(culture, nameof(culture));

        if (!culture.Split('-').Any())
            throw new ArgumentException("The culture not valid");
        return culture.Split('-')[1];
    }
}
