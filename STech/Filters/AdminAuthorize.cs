﻿using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace STech.Filters
{
    public class AdminAuthorize : Attribute, IAuthorizationFilter
    {
        public string? Code { get; set; }
        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var requestPath = context.HttpContext.Request.Path.Value;

            if (context.HttpContext.User.Identity?.IsAuthenticated == false || !context.HttpContext.User.IsInRole("admin"))
            {
                if (requestPath != null && requestPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                else
                {
                    context.HttpContext.Response.Redirect("/admin/login");
                    return;
                }
            }
            
            if (Code == null)
            {
                return;
            }
               
            bool hasAllPermissions = context.HttpContext.User
                .FindFirst("HasAllPermissions")?.Value == "True";

            if (hasAllPermissions)
            {
                return;
            }
            
            string? permissionsJson = context.HttpContext.User
                .FindFirst("authorized")?.Value;

            if (permissionsJson != null)
            {
                IEnumerable<string> permissions = JsonSerializer
                    .Deserialize<IEnumerable<string>>(permissionsJson) ?? new List<string>();

                if(!permissions.Contains(Code))
                {
                    if (requestPath != null && requestPath.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
                    {
                        context.Result = new UnauthorizedResult();
                        return;
                    }
                    else
                    {
                        context.HttpContext.Response.Redirect("/admin/error/unauthorized");
                        return;
                    }
                }
            }
        }
    }
}
