using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AppointmentSystem.Attributes
{
    public class ReceptionistOnlyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var role = context.HttpContext.Session.GetString("Role");
            if (role != "Receptionist")
            {
                context.Result = new RedirectToActionResult("Login", "Account", "");
            }
        }
    }
}
