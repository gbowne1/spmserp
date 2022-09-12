using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace OnlineDataBuilder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [Authorize(Role.Admin)]
        [HttpPost("GetSystemDashboard")]
        public IResponse<ApiResponse> GetSystemDashboard(AttendenceDetail userDetail)
        {
            var result = _dashboardService.GetSystemDashboardService(userDetail);
            return BuildResponse(result, HttpStatusCode.OK);
        }
    }
}
Footer
