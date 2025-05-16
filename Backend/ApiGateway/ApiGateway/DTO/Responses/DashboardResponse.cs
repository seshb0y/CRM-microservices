using CRMSolution.Grpc.Tasks;
using TaskStatus = CRMSolution.Grpc.Tasks.GrpcTaskStatus;

namespace ApiGateway.DTO.Responses;

public class DashboardResponse
{
    public decimal OrdersTotalAmount { get; set; }
    public int OrdersCount { get; set; }
    
    public List<DateTime> OrdersCreatedDates { get; set; }
    public int ClientsAmount { get; set; }
    
    public List<TaskStatus> TasksStatuses { get; set; }
    public int TasksCount { get; set; }
    
}