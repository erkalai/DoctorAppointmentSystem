using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppointmentSystem.Attributes
{
    public class DoctorOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            if (role != "Doctor")
            {
                context.Result = new RedirectToActionResult("Login", "Account", "");
            }
        }
    }
}
