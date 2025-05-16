using CRMSolution.Grpc.Tasks;
using TaskStatus = CRMSolution.Grpc.Tasks.GrpcTaskStatus;

namespace ApiGateway.DTO.Responses;

public class HttpGetAllTasksResponse
{
    public List<TaskDto> Tasks { get; set; }
}

public class TaskDto
{
    public int TaskId { get; set; }
    public int OrderId { get; set; }
    public string Title { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime DueDate { get; set; }
    public string Username { get; set; }
}
