using FluentValidation;
using MediatR;
using SoulViet.Modules.Marketplace.Marketplace.Application.Exceptions;

namespace SoulViet.Modules.Marketplace.Marketplace.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest: IRequest<TResponse>
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
                // Gom tất cả câu thông báo lỗi thành 1 chuỗi để ném ra BadRequest
                var errorMessages = string.Join(" | ", failures.Select(e => e.ErrorMessage));

                // Ném BadRequestException (chính là Exception bạn hay dùng)
                throw new BadRequestException(errorMessages);
            }
        }

        return await next();
    }
}