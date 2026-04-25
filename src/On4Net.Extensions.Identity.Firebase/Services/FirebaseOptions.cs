

using System.ComponentModel;

using Newtonsoft.Json;


public class FirebaseOptions
{
     
    public string Url { get { return "https://identitytoolkit.googleapis.com/v1/"; } }

   
    public string ApiKey { set; get; }

    
    public string Audience { set; get; }

     
    public string Validator { get { return $"https://securetoken.google.com/{Audience}"; } }

   
}
