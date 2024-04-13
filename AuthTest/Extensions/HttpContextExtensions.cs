namespace AuthTest.Extensions;

public static class HttpContextExtensions
{
    public static string GetUserId(this HttpContext httpContext)
    {
        return httpContext.User.Claims.ToList().Find(claim => claim.Type == "sub")?.Value!;
    } 

}