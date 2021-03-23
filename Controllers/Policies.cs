namespace Vega.Controllers
{
    // Use this class to store the policy names so that we don't need to type it repetitively when we need it like
    // adding authentication policies and putting the policy name into the Authorize annotation of the controller method
    // and so on...
    public static class Policies
    {
        public const string RequireAdminRole = "RequireAdminRole";
    }
}