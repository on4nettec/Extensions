using System.Globalization;

namespace On4Net.Extensions.Common;

public struct Culture
{
    public static readonly Culture English = new Culture
    {
        CurrentCulture = "en-US",
        DefaultCulture = "en-US"
    };

    public string CurrentCulture { get; set; }

    public string DefaultCulture { get; set; }

    public string[] OtherCultures { get; set; }

    public string CurrentLanguage { get { return CurrentCulture.GetLanguageCulture(); } }
    public string CurrentCountry { get { return CurrentCulture.GetCountryCulture(); } }


}
