using System.Web;
using System.Web.Mvc;
using Token.Methods;

namespace Token
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
