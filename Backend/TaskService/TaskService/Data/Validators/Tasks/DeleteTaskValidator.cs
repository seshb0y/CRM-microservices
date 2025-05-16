
using CRMSolution.Grpc.Tasks;
using FluentValidation;
using TaskService.Data.Repository.Interface;
using TaskService.Data.Repository.TasksRep;

namespace CRMSolution.Data.Validators.Tasks;

public class DeleteTaskValidator : AbstractValidator<DeleteTaskRequest>
{
    ITasksRep _taskRepository;

    public DeleteTaskValidator(ITasksRep taskRepository)
    {
        _taskRepository = taskRepository;
        
        RuleFor(r => r.Id)
            .NotEmpty()
            .WithMessage("Task Id cannot be empty")
            .MustAsync(IsTaskExist)
            .WithMessage("Task not found");
    }

    private async Task<bool> IsTaskExist(int id, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(id);
        return task != null;
    }
}