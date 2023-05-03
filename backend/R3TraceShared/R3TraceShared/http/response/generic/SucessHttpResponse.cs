namespace R3TraceShared.http.response.generic;

public class SuccessHttpResponse : GenericHttpResponse
{
    public SuccessHttpResponse(object? response = null, int statusCode = 200) : base(true, response, statusCode)
    {
    }
}