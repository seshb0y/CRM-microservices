namespace ApiGateway.DTO.Requests.Twilio;

public record HttpSaveRecordRequest(int orderId, string callSid);