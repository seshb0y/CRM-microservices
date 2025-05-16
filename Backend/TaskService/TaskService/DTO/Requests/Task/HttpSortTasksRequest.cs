namespace TaskService.DTO.Requests.Task;

public class HttpSortTasksRequest
{
    public string? sortBy { get; set; } 
    public bool Descending { get; set; } = false; 
}