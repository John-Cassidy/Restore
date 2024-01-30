using Microsoft.AspNetCore.Http;

namespace Restore.API;

public static class HttpContextExtensions
{
    public static string GetBuyerId(this HttpContext context)
    {
        if ((context?.User?.Identity?.IsAuthenticated ?? false) &&
        !string.IsNullOrEmpty(context?.User?.Identity?.Name))
        {
            context.Response.Cookies.Delete("buyerId");
            return context.User.Identity.Name;
        }
        var buyerId = context?.Request.Cookies["buyerId"];
        if (buyerId == null)
        {
            buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
            context?.Response.Cookies.Append("buyerId", buyerId, cookieOptions);
        }
        return buyerId;
    }
}
