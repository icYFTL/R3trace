using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace R3TraceShared.attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ForAdminAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        context.HttpContext.Request.Headers.TryGetValue("Identify", out var userUid);
        if (!userUid.Any()) // Bad nginx conf
        {
            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Unauthorized;
            context.Result = new JsonResult("NotAuthorized")
            {
                Value = new
                {
                    Status = "Error",
                    Response = "Invalid Token"
                },
            };
            return;
        }

        context.HttpContext.Request.Headers.TryGetValue("IsAdmin", out var isAdminVal);
        var isAdmin = isAdminVal.FirstOrDefault() == "true"; // xd
        if (!isAdmin)
        {
            context.Result = new JsonResult("Forbidden")
            {
                Value = new
                {
                    Status = "Error",
                    Response = "Where is your flag?"
                },
            };
            context.HttpContext.Response.StatusCode = (int) HttpStatusCode.Forbidden;
        }
    }
}