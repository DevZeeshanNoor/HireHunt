
using System.Text.Json;

namespace HireHuntBackend.Common
{
    public static class Helper
    {
        public static void SetCookie(HttpResponse response, string key, string value, int expireDays = 7)
        {
            response.Cookies.Append(key, value, new CookieOptions
            {
                HttpOnly = true,       // JS cannot read cookie
                Secure = true,         // Only sent over HTTPS
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(expireDays)
            });
        }

        // Method specifically for auth cookies (refresh token + user info)
        public static void SetAuthCookies(HttpResponse response, string refreshToken, object userInfo)
        {
            SetCookie(response, "refreshToken", refreshToken);  // set refresh token
            SetCookie(response, "userInfo", JsonSerializer.Serialize(userInfo)); // set user info
        }
    }
}
