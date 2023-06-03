using ProjectManagement.Models;
using ProjectManagement.ViewModels;
using ProjectManagement.Controllers;
namespace ProjectManagement.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ProjectManagementContext _context;

        public ExceptionHandlingMiddleware(ProjectManagementContext context)
        {
            _context = context;
        }
        public async System.Threading.Tasks.Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                //Call the next middleware in the pipeline
                await next(context);
            }
            catch (Exception ex)
            {

                // Get the current logged-in user's username
                string username = context.User.Identity.Name;

                // Create the log object
                Log log = new Log
                {
                    Username = username,
                    LogTimestamp = DateTime.Now,
                    LogType = "Exception",
                    Source = "Web",
                    Message = ex.Message,
                    PageSource = context.Request.Path
                };

                //insert log record in DB
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();

                var errorMessage = "An error occurred: " + ex.Message;
                var errorUrl = "/Home/Error?errorMessage=" + Uri.EscapeDataString(errorMessage);
                context.Response.Redirect(errorUrl);
            }
        }
    }

}
