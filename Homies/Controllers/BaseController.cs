using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Homies.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        public string GetCurrentUserId()
        {
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return currentUserId;
        }
    }
}
