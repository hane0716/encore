using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

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

        public string KaiinMessage { get; set; }

        public string strUserName { get; set; }

        public string KirokuName { get; set; }
        public DateTime LiveDate { get; set; }

        public string Message { get; set; }
        public string WelcomeMessage { get; set; }

        public string Name { get; set; }

        public string DelName { get; set; }

    }
}
