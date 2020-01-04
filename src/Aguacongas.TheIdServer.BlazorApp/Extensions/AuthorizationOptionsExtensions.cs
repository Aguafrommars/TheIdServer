namespace Microsoft.AspNetCore.Authorization
{
    public static class AuthorizationOptionsExtensions
    {
        public const string WRITER = "Is4-Writer";
        public const string READER = "Is4-Reader";
        public static void AddIdentityServerPolicies(this AuthorizationOptions options)
        {
            options.AddPolicy(WRITER, policy =>
                   policy.RequireAssertion(context =>
                       context.User.IsInRole(WRITER)));
            options.AddPolicy(READER, policy =>
               policy.RequireAssertion(context =>
                   context.User.IsInRole(READER)));
        }
    }
}
