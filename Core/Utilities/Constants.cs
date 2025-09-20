namespace Core;

public static class Constants
{
    public static class CustomClaims
    {
        public const string EMAIL_VERIFIED = "email-verified";
    }
    public static class Tokens
    {
        public const string ACCESS_TOKEN_COOKIE_NAME  = "access-token";
        public const string REFRESH_TOKEN_COOKIE_NAME = "refresh-token";
        public const string CSRF_TOKEN_COOKIE_NAME    = "XSRF-TOKEN";
        public const string CSRF_TOKEN_HEADER_NAME    = "X-CSRF-TOKEN";
    
        public static readonly TimeSpan REFRESH_TOKEN_EXPIRY_TIME = TimeSpan.FromDays(7);
        public static readonly TimeSpan ACCESS_TOKEN_EXPIRY_TIME  = TimeSpan.FromMinutes(10);
    }
    public static class Cors
    {
        public const string POLICY_NAME = "AppCorsPolicy";
    }
    public static class Roles
    {
        public const string ADMIN = "Admin";
        public const string GUEST = "Guest";
    }
}