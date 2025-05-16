namespace TwilioService.Services;

public interface ITwilioService
{
    public string MakeCall(string to);
    public string GetRecordingUrl(string callSid);
    public Task SaveCallRecording(int orderId, string callSid);
}