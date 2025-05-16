using CRMSolution.Data.Repository.Interface;
using CRMSolution.Grpc.Orders;
using CRMSolution.Grpc.Twilio;
using Grpc.Core;
using TwilioService.Services;

namespace TwilioService.GrpcServices;

public class TwilioGrpcService : CRMSolution.Grpc.Twilio.TwilioGrpcService.TwilioGrpcServiceBase
{
    private readonly ITwilioService _twilioService;
    private readonly OrderGrpcService.OrderGrpcServiceClient _orderService;

    public TwilioGrpcService(ITwilioService twilioService, OrderGrpcService.OrderGrpcServiceClient orderService)
    {
        _twilioService = twilioService;
        _orderService = orderService;
    }

    public override Task<MakeCallResponse> MakeCall(MakeCallRequest request, ServerCallContext context)
    { 
        var callSid =  _twilioService.MakeCall(request.To);
        return Task.FromResult(new MakeCallResponse { CallSid = callSid });
    }

    public override Task<GetRecordingUrlResponse> GetRecordingUrl(GetRecodingUrlRequest request,
        ServerCallContext context)
    {
        var response = _twilioService.GetRecordingUrl(request.CallSid);
        return Task.FromResult(new GetRecordingUrlResponse
        {
            RecordingUrl = response
        });
    }

    public override Task<SaveCallRecordingResponse> SaveCallRecording(SaveCallRecordingRequest request,
        ServerCallContext context)
    {
        _twilioService.SaveCallRecording(request.OrderId, request.CallSid);
        return Task.FromResult(new SaveCallRecordingResponse
        {
            Message = "call recording success"
        });
    }
}