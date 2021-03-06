﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;

namespace Serilog.Ui.Web.Filters
{
    internal class AuthorizationFilter : IAuthorizationFilter
    {
        private readonly AuthorizationOptions _authorizationOptions;

        public AuthorizationFilter(AuthorizationOptions authorizationOptions)
        {
            _authorizationOptions = authorizationOptions;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!_authorizationOptions.Enabled)
                return;

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var userName = context.HttpContext.User.Identity.Name.ToLower();

            if (_authorizationOptions.Usernames != null &&
                _authorizationOptions.Usernames.Any(u => u.ToLower() == userName))
                return;

            if (_authorizationOptions.Roles != null &&
                _authorizationOptions.Roles.Any(role => context.HttpContext.User.IsInRole(role)))
                return;

            context.Result = new ForbidResult();
        }
    }
}