using ApiGateway.DTO.Requests.Twilio;
using CRMSolution.Grpc.Twilio;
using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/v1/twilio")]
public class TwilioController : ControllerBase
{

    private readonly TwilioGrpcService.TwilioGrpcServiceClient _twilioGrpcServiceClient;
    
    public TwilioController(TwilioGrpcService.TwilioGrpcServiceClient twilioGrpcServiceClient)
    {
        _twilioGrpcServiceClient = twilioGrpcServiceClient;
    }
    
    [HttpPost("calls")]
    public IActionResult MakeCall([FromBody] HttpCallRequest request)
    {
        var grpcRequest = new MakeCallRequest
        {
            To = request.To
        };
        var grpcResponse = _twilioGrpcServiceClient.MakeCall(grpcRequest);
        return Ok(new { CallSid = grpcResponse.CallSid });
    }

    [HttpGet("recordings/{callSid}")]
    public IActionResult GetRecordingUrl([FromRoute] string callSid)
    {
        var grpcRequest = new GetRecodingUrlRequest
        {
            CallSid = callSid
        };
        var grpcResponse = _twilioGrpcServiceClient.GetRecordingUrl(grpcRequest);
        return Ok(grpcResponse);
    }
    
    [HttpPost("recordings")]
    public IActionResult SaveRecording([FromBody] HttpSaveRecordRequest request)
    {
        var grpcRequest = new SaveCallRecordingRequest
        {
            CallSid = request.callSid,
            OrderId = request.orderId
        };
        var grpcResponse = _twilioGrpcServiceClient.SaveCallRecording(grpcRequest);
        return Ok("url added");
    }
}