using CRMSolution.Grpc.Orders;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using TwilioService.Services;

namespace CRMSolution.Services.Classes;

public class TwilioService : ITwilioService
{
    private readonly string _accountSid;
    private readonly string _authToken;
    private readonly string _twilioNumber;
    private readonly string _twimlUrl;
    private readonly OrderGrpcService.OrderGrpcServiceClient _orderGrpcService;
    
    public TwilioService(IConfiguration configuration, OrderGrpcService.OrderGrpcServiceClient orderGrpcService)
    {
        _accountSid = configuration["Twilio:AccountSid"];
        _authToken = configuration["Twilio:AuthToken"];
        _twilioNumber = configuration["Twilio:PhoneNumber"];
        _twimlUrl = configuration["Twilio:TwiMLUrl"];
        TwilioClient.Init(_accountSid, _authToken);
        _orderGrpcService = orderGrpcService;
    }
    
    
    public string MakeCall(string to)
    {
        var call = CallResource.Create(
            to: new PhoneNumber(to),
            from: new PhoneNumber(_twilioNumber),
            url: new Uri($"https://your-crm.com/api/twilio/twiml?to={to}"),
            record: true
        );

        return call.Sid;
    }

    public string GetRecordingUrl(string callSid)
    {
        var recording = RecordingResource.Read(callSid: callSid).FirstOrDefault();
        
        return $"https://api.twilio.com{recording.Uri.Replace(".json", ".mp3")}";
    }
    
    public async Task SaveCallRecording(int orderId, string callSid)
    {
        var recordingUrl = GetRecordingUrl(callSid);

        var grpcRequest = new SaveCallRecordRequest
        {
            OrderId = orderId,
            RecordUrl = recordingUrl,
        };
        var grpcResponse = await _orderGrpcService.SaveCallRecordAsync(grpcRequest);
    }
}