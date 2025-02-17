using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace VersuriAPI.Utils
{
    public static class Authorization
    {

        public static async Task<GoogleJsonWebSignature.Payload?> Validate(string idToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken);
                return payload;
            }
            catch
            {
                //return true; //this is for not having to add the idtoken when testing
                return null;
            }
        }
    }
}
