namespace CRMSolution.DTO.Requests.Task;

public record HttpCreateTaskRequest(string title, string description, DateTime endDate, string userName, int orderId);