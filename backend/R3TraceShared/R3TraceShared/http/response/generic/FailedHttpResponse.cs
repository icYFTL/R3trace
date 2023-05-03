namespace R3TraceShared.http.response.generic;

public class FailedHttpResponse : GenericHttpResponse
{
    public FailedHttpResponse(object? response = null, int statusCode = 520) : base(false, response, statusCode)
    {
        
    }
}