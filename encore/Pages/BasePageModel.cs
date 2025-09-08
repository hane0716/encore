using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

namespace encore.Pages
{
    public class BasePageModel : PageModel
    {
        protected void SetUserSession(string key, string value)
        {
            HttpContext.Session.SetString(key, value);
        }

        protected string GetUserSession(string key)
        {
            return HttpContext.Session.GetString(key);
        }

        protected void ClearSession(string key)
        {
            HttpContext.Session.Remove(key);
        }
    }
}
