using System.Text.RegularExpressions;

namespace On4Net.Extensions.Common;

public static class RegExExtesion
{
    private static bool IsMatch(this string RegExString, string InputString)
    {
        Regex aRegex = new Regex(RegExString);
        return aRegex.IsMatch(InputString);
    }

    public const string RegEx_Digit = "^\\d+";
    public const string RegEx_Decimal = "^[+-]?[1-9]+\\d*[.]?\\d+";
    public const string RegEx_Email = "^[-a-zA-Z0-9][-.a-zA-Z0-9_]*@[-.a-zA-Z0-9]+(\\.[-.a-zA-Z0-9]+)*\\.(com|edu|info|gov|int|mil|net|org|biz|name|museum|coop|aero|pro|tv|[a-zA-Z]{2})$";
    public const string RegEx_WebAddress = "^((ht|f)tp(s?)\\:\\/\\/|~/|/)?([\\w]+:\\w+@)?([a-zA-Z]{1}([\\w\\-]+\\.)+([\\w]{2,5}))(:[\\d]{1,5})?((/?\\w+/)+|/?)(\\w+\\.[\\w]{3,4})?((\\?\\w+=\\w+)?(&\\w+=\\w+)*)?";
    public const string RegEx_Time12 = "^[0-1][1-2]:[0-5][0-9]:[0-5][0-9]$|^[0-1][1-2]:[0-5][0-9]$";
    public const string RegEx_Time24 = "^[0-1][0-9]|[2][0-3]:[0-5][0-9]:[0-5][0-9]$|^[0-1][0-9]|[2][0-3]:[0-5][0-9]$";
    public const string RegEx_TimeDuration = "^\\d*:[0-5][0-9]:[0-5][0-9]$|^\\d*:[0-5][0-9]";
    public const string RegEx_IPAddress = "[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}\\.[0-9]{1,3}";
    public const string RegEx_Password = @"^((?=.*[a-z])(?=.*[A-Z])(?=.*\d)).+$";


    /// <summary>
    /// Integer format validation string
    /// </summary>
    public static bool IsMatch_Digit(this string InputString)
    {
        return IsMatch(RegEx_Digit, InputString);
    }
    /// <summary>
    /// Validated string of decimal number format
    /// </summary>
    public static bool IsMatch_Decimal(this string InputString)
    {
        return IsMatch(RegEx_Decimal, InputString);
    }
    /// <summary>
    /// Email format validation string
    /// </summary>
    public static bool IsMatch_Email(this string InputString)
    {
        return IsMatch(RegEx_Email, InputString);
    }
    /// <summary>
    /// Web address format validation string
    /// </summary>
    public static bool IsMatch_WebAddress(this string InputString)
    {
        return IsMatch(RegEx_WebAddress, InputString);
    }
    /// <summary>
    /// 12-hour time format validation string
    /// </summary>
    public static bool IsMatch_Time12(this string InputString)
    {
        return IsMatch(RegEx_Time12, InputString);
    }
    /// <summary>
    /// 24-hour time format validation string
    /// </summary>
    public static bool IsMatch_Time24(this string InputString)
    {
        return IsMatch(RegEx_Time24, InputString);

    }
    /// <summary>
    /// IP format validation string
    /// </summary>
    public static bool IsMatch_IPAddress(this string InputString)
    {
        return IsMatch(RegEx_IPAddress, InputString);
    }
    /// <summary>
    /// Duration format validation string
    /// </summary>
    public static bool IsMatch_TimeDuration(this string InputString)
    {
        return IsMatch(RegEx_TimeDuration, InputString);
    }
    /// <summary>
    /// The method that performs the validation operation of a string format based on a validation string
    /// </summary>
    /// <param name = "RegExString"> Input string that specifies the validation format </par>
    /// <param name = "InputString"> Input string to be validated </par>
    /// <returns> indicates the match between the input string and the format expressed in the validation string </returns>
    public static bool IsMatch_Password(this string InputString)
    {
        return IsMatch(RegEx_Password, InputString);
    }
    public static bool IsExpressionValid(this string stringForValidate, string pattern)
    {

        if (string.IsNullOrEmpty(stringForValidate))
        {
            return false;
        }

        return IsMatch(pattern, stringForValidate);
    }

}
