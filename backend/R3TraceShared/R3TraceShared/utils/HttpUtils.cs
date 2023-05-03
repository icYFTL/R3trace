using System;
using System.Net;

namespace R3TraceShared.utils;

public static class HttpUtils
{
    public static HttpStatusCode HttpStatusCodeFromNumber(int statusCode)
    {
        if (Enum.IsDefined(typeof(HttpStatusCode), statusCode))
            return (HttpStatusCode)statusCode;

        throw new ArgumentException("Invalid value for HttpStatusCode enum");
    }

    public static int NumberFromHttpStatusCode(HttpStatusCode code)
    {
        return (int)code;
    }
}