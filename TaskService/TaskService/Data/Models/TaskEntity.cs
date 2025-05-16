namespace TaskService.Data.Models;

public class TaskEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TasksStatus Status { get; set; } = TasksStatus.New;
    public DateTime DueDate { get; set; }

    public int OrderId { get; set; }
    public int UserId { get; set; }
}

public enum TasksStatus
{
    New,
    InProgress,
    Completed
}