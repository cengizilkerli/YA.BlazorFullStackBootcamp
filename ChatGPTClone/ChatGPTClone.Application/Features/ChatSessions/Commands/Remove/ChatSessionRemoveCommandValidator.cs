using ChatGPTClone.Application.Common.Interfaces;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace ChatGPTClone.Application.Features.ChatSessions.Commands.Remove;

public class ChatSessionRemoveCommandValidator : AbstractValidator<ChatSessionRemoveCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserServices _currentUserService;

    public ChatSessionRemoveCommandValidator(IApplicationDbContext dbContext, ICurrentUserServices currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        
        RuleFor(p => p.Id)
            .NotEmpty()
            .NotNull()
            .MustAsync(BeValidIdAsync)
            .WithMessage("The selected Chat was not found.");
    }

    private Task<bool> BeValidIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return _dbContext
            .ChatSessions
            .AnyAsync(x => x.Id == id && x.AppUserId == _currentUserService.UserId, cancellationToken);
    }
}
