using FeatureToggle.Domain.Entity.FeatureManagementSchema;
using FeatureToggle.Infrastructure.Models;
using MediatR;

namespace FeatureToggle.Application.Requests.Commands.LogCommands
{
    public class AddLogCommandHandler(FeatureManagementContext featureContext) : IRequestHandler<AddLogCommand>
    {
        public async Task Handle(AddLogCommand request, CancellationToken cancellationToken)
        {
            Log log = new(request.UserId,
                          request.FeatureId,
                          request.FeatureName,
                          request.BusinessId,
                          request.BusinessName,
                          request.action
                       );

            await featureContext.Logs.AddAsync(log,cancellationToken);   
            await featureContext.SaveChangesAsync(cancellationToken);
        }
    }
}
