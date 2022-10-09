using FluentValidation;
using Helpers.Pagination;

namespace Helpers.WebApi.Extensions;

public static class FluentValidationExtensions
{
    public static IRuleBuilderOptions<T, Paginator> PaginatorValidate<T>(this IRuleBuilder<T, Paginator> ruleBuilder)
    {
        return ruleBuilder
            .NotNull()
            .Must(val => val.PageNumber > 0).WithMessage("Page number must > 0")
            .Must(val => val.PageSize > 0).WithMessage("Page size must > 0")
            .Must(val => val.PageSize <= 20).WithMessage("Page size must <= 20");
    }
}