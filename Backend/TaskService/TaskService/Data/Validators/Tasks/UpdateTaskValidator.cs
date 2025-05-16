
using CRMSolution.Grpc.Tasks;
using FluentValidation;
using TaskService.Data.Repository.Interface;
using TaskService.Data.Repository.TasksRep;

namespace CRMSolution.Data.Validators.Tasks;

public class UpdateTaskValidator : AbstractValidator<UpdateTaskRequest>
{
    ITasksRep _taskRepository;

    public UpdateTaskValidator(ITasksRep taskRepository)
    {
        _taskRepository = taskRepository;
        
        RuleFor(r => r.TaskId)
            .NotEmpty()
            .WithMessage("Task Id cannot be empty")
            .MustAsync(IsTaskExist)
            .WithMessage("Task not found");
        
        RuleFor(r => r.Description)
            .NotEmpty()
            .WithMessage("Description cannot be empty")
            .NotNull()
            .WithMessage("Description cannot be null");

        RuleFor(r => r.Status)
            .IsInEnum()
            .WithMessage("Status cannot be empty");

    }

    private async Task<bool> IsTaskExist(int id, CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetById(id);
        return task != null;
    }
}