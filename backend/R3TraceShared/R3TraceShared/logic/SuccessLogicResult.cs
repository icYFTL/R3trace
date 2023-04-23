using System.Net;

namespace R3TraceShared.logic;

public class SuccessLogicResult : GenericLogicResult
{
    public override bool Status => true;
    public override Exception? Exception => null;
    public override HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
}