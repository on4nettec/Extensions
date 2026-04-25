using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using On4Net.Extensions.Identity.Firebase.Models;

namespace On4Net.Extensions.Identity.Firebase.Services.Infostruct;

public interface IFirebaseServices
{
    Task<string> SendVerifyEmail(string idToken, CancellationToken cancellationToken);
    //Task<bool> ConfirmVerifyEmail(string oobCode, CancellationToken cancellationToken);

    Task<VerifyCustomTokenModel> VerifyCustomToken(string token, bool returnSecureToken, CancellationToken cancellationToken);
    Task<LoginModel> SignupNewUser(string email, string password,bool returnSecureToken, CancellationToken cancellationToken);   
    Task<LoginModel> Login(string username, string password,CancellationToken cancellationToken);

    //Task<bool> ChangePassword(string email, string password, CancellationToken cancellationToken);
    //Task<string> SendPasswordReset(string email, CancellationToken cancellationToken);

}
