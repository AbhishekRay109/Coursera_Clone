using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using Jose;
using Kitana.Core.Logger;

namespace Kitana.Api.Middleware
{
    /// <summary>
    /// Custom attribute derived from <see cref="AuthorizeAttribute"/> to represent permissions-based authorization.
    /// </summary>
    /// <remarks>
    /// The <c>PermissionsAttribute</c> is designed to extend the functionality of the standard <c>AuthorizeAttribute</c>
    /// by incorporating permissions-based authorization logic. 
    /// </remarks>
    public class PermissionsAttribute : AuthorizeAttribute {

        const string POLICY_PREFIX = "Permissions";

        /// <summary>
        /// Initializes a new instance of the <see cref="PermissionsAttribute"/> class with the specified permission.
        /// </summary>
        /// <param name="permission">The permission required for access.</param>
        /// <remarks>
        /// This constructor allows the <c>PermissionsAttribute</c> to be instantiated with a specific permission.
        /// </remarks>
        public PermissionsAttribute(string permission) {
            Permission = permission;
        }

        /// <summary>
        /// Gets or sets the permission associated with the <see cref="PermissionsAttribute"/>.
        /// </summary>
        /// <value>The permission required for access.</value>
        /// <remarks>
        /// The <c>Permission</c> property represents the specific permission required for accessing the associated
        /// controller or action method.
        /// </remarks>
        public string Permission
        {
            get
            {
                if (!String.IsNullOrEmpty(Policy.Substring(POLICY_PREFIX.Length)))
                {
                    return Policy.Substring(Policy.Length);
                }
                return default(string);
            }
            set
            {
                Policy = $"{POLICY_PREFIX}{value.ToString()}";
            }
        }
    }

    internal class PermissionsPolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "Permissions";
        
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            var abc = Task.FromResult(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme).RequireAuthenticatedUser().Build());
            return abc;
        }
        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() =>
            Task.FromResult<AuthorizationPolicy>(null);
        

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var Permissions = policyName.Substring(POLICY_PREFIX.Length);
                var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);
                policy.AddRequirements(new PolicyRequirement(Permissions));
                var abc = Task.FromResult(policy.Build());
                return abc;
            }
            return Task.FromResult<AuthorizationPolicy>(null);
        }
    }

    /// <summary>
    /// Represents a custom authorization policy requirement, implementing the <see cref="IAuthorizationRequirement"/> interface.
    /// </summary>
    /// <remarks>
    /// The <c>PolicyRequirement</c> class defines a custom authorization requirement that can be used in conjunction with
    /// authorization policies.
    /// </remarks>
    public class PolicyRequirement : IAuthorizationRequirement {

        /// <summary>
        /// Gets or sets the permission associated with the <see cref="PermissionsAttribute"/>.
        /// </summary>
        /// <value>The permission required for access.</value>
        /// <remarks>
        /// The <c>Permission</c> property represents the specific permission required for accessing the associated
        /// controller or action method.
        /// </remarks>
        public string Permission { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyRequirement"/> class with the specified permission.
        /// </summary>
        /// <param name="permission">The permission required for authorization.</param>
        /// <remarks>
        /// This constructor allows the <c>PolicyRequirement</c> to be instantiated with a specific permission.
        /// </remarks>
        public PolicyRequirement(string permission) {
            this.Permission = permission;
        }
    }

    /// <summary>
    /// Custom authorization handler for processing policies based on the <see cref="PolicyRequirement"/> class.
    /// Implements both <see cref="AuthorizationHandler{TRequirement}"/> and <see cref="IAuthorizationFilter"/>.
    /// </summary>
    /// <remarks>
    /// The <c>PolicyAuthorizationHandler</c> class is responsible for handling authorization requests based on
    /// the defined <c>PolicyRequirement</c>.
    /// </remarks>
    public class PolicyAuthorizationHandler : AuthorizationHandler<PolicyRequirement>, IAuthorizationFilter
    {
        private readonly ValidationMiddleware middleware;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolicyAuthorizationHandler"/> class with a reference to the specified <see cref="ValidationMiddleware"/>.
        /// </summary>
        /// <param name="middleware">The <see cref="ValidationMiddleware"/> instance used for additional validation within the authorization handler.</param>
        /// <remarks>
        /// This constructor allows the <c>PolicyAuthorizationHandler</c> to be instantiated with a reference to a specific <c>ValidationMiddleware</c>,
        /// enabling additional validation logic to be integrated into the authorization process.
        /// </remarks>
        public PolicyAuthorizationHandler(ValidationMiddleware middleware)
        {
            this.middleware = middleware;
        }

        /// <summary>
        /// Performs authorization logic as part of the <see cref="IAuthorizationFilter"/> interface.
        /// </summary>
        /// <param name="context">The <see cref="AuthorizationFilterContext"/> containing information about the authorization request.</param>
        /// <remarks>
        /// This method is invoked as part of the <see cref="IAuthorizationFilter"/> interface and is responsible for
        /// performing authorization logic based on the provided <see cref="AuthorizationFilterContext"/>.
        /// </remarks>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var isAuthenticated = context.HttpContext.User.Identity.IsAuthenticated;
            if (!isAuthenticated)
            {
                context.Result = new UnauthorizedResult();
                return;
            }
        }

        /// <summary>
        /// Handles the asynchronous processing of a policy requirement within the <see cref="AuthorizationHandler{TRequirement}"/> class.
        /// </summary>
        /// <param name="context">The <see cref="AuthorizationHandlerContext"/> containing information about the authorization request.</param>
        /// <param name="requirement">The <see cref="PolicyRequirement"/> representing the policy requirement for authorization.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous completion of the authorization handling process.</returns>
        /// <remarks>
        /// This method overrides the base class's implementation to handle the specific policy requirement asynchronously.
        /// </remarks>
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PolicyRequirement requirement)
        {
            try
            {
                if (context.Resource is not HttpContext httpCtx)
                {
                    context.Fail();
                    return Task.CompletedTask;
                }

                //var appCode = httpCtx.Request.RouteValues["appCode"]?.ToString();
                //var orgCode = httpCtx.Request.RouteValues["orgCode"]?.ToString();
                var jwtToken = httpCtx.Request.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
                //var jwtToken = DecryptJwt(jweToken);

                if (CheckValidty(jwtToken, requirement.Permission))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
                context.Fail();
                return Task.CompletedTask;
            }
            catch (Exception)
            {
                context.Fail();
                return Task.CompletedTask;
            }
        }

        /// <summary>
        /// Decrypts a JWT (JSON Web Token) using the specified encrypted token.
        /// </summary>
        /// <param name="encryptedtoken">The encrypted JWT token to be decrypted.</param>
        /// <returns>The decrypted JWT token as a string.</returns>
        /// <remarks>
        /// This method is responsible for decrypting a JWT token that has been encrypted or encoded.
        /// </remarks>
       
        private bool CheckValidty(string JwtToken, string validation)
        {
            var parameters = middleware.IsValid(JwtToken, validation);
            return parameters;
        }
    }
}
