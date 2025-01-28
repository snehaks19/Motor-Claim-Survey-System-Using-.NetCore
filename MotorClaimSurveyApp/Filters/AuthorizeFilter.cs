using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MotorClaimSurveyApp.Filters
{
    public class AuthorizeFilter:IAuthorizationFilter
    {
        private readonly ISession _session;
        public AuthorizeFilter(IHttpContextAccessor httpContextAccessor)
        {
            _session = httpContextAccessor.HttpContext.Session;
        }
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            if (string.IsNullOrEmpty(_session.GetString("UserId")))
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"Area","" },
                    {"Controller","Login" },
                    {"Action","Login" }
                });
            }

        }

        public sealed class AuthorizeAttribute : TypeFilterAttribute
        {
            public AuthorizeAttribute() : base(typeof(AuthorizeFilter))
            {
                Arguments = new object[] { };
            }

        }
    }
}
