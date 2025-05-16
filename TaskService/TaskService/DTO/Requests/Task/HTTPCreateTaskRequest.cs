
namespace TaskService.DTO.Requests.Task;

public record HTTPCreateTaskRequest(string title, string description, DateTime endDate, string userName, int orderId);