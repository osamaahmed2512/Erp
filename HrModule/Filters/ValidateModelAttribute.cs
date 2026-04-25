using Application.Dtos.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HrModule.Filters
{
    public class ValidateModelAttribute: ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var errors = context.ModelState
                    .Where(m => m.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).AsEnumerable()
                    );

                var errorResponse = new ValidationErrorResponse(400, "Invalid data. Please correct your input and try again.", errors);
                context.Result = new BadRequestObjectResult(errorResponse);
            }
        }
    }
}
