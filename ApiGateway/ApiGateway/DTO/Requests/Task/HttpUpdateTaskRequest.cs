namespace ApiGateway.DTO.Requests.Task;

public record HttpUpdateTaskRequest(string status, string description, int taskId);