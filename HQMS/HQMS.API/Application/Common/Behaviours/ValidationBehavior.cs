using FluentValidation;
using HQMS.Application;
using HQMS.Application.Common;
using HQMS.Application.Common.Models;
using MediatR;

namespace HQMS.Application.Common.Behaviors 
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators) 
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);

                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                {
                    var errors = failures.Select(f => f.ErrorMessage);

                    // Return Result<T> with validation errors
                    if (typeof(TResponse).IsGenericType &&
                        typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var resultType = typeof(TResponse).GetGenericArguments()[0];
                        var failureMethod = typeof(Result<>)
                            .MakeGenericType(resultType)
                            .GetMethod("Failure", new[] { typeof(IEnumerable<string>) });

                        return (TResponse)failureMethod.Invoke(null, new object[] { errors });
                    }
                }
            }

            return await next();
        }
    }
}
