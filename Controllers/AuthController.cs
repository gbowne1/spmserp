using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace spmserp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly spmserpContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        private readonly CookieOptions _cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        };

        public AuthController(spmserpContext context, IPasswordHasher passwordHasher, ITokenService tokenService, IConfiguration configuration, ILogger<AuthController> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _configuration = configuration;
            _logger = logger;

        }

        [HttpPost("token")]
        public IActionResult Token(string username, string password)
        {
            var authEmployee = GetAuthEmployee(username, password);

            if (authEmployee == null)
            {
                return Unauthorized(new { errorText = "Invalid username or password." });
            }

            var accessToken = ProduceAccessToken(authEmployee);

            _logger.LogInformation("{requestMethod}:{requestPath}: User \"{username}\" got new token", Request.Method, Request.Path, username);

            return Json(new { accessToken, employeeId = authEmployee.Id });
        }

        [HttpPost("refreshToken")]
        public IActionResult RefreshToken(string accessToken)
        {
            if (string.IsNullOrEmpty(Request.Cookies[CookiesKeys.EMPLOYEE_ID]) || string.IsNullOrEmpty(Request.Cookies[CookiesKeys.REFRESH_TOKEN]))
            {
                return Unauthorized();
            }

            var refreshToken = Request.Cookies[CookiesKeys.REFRESH_TOKEN];
            var employeeId = Guid.Parse(Request.Cookies[CookiesKeys.EMPLOYEE_ID]);
            var employee = _context.Employees.Find(employeeId);

            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken) || employee == null)
            {
                return Unauthorized();
            }

            try
            {
                var accessJwtsecurToken = _tokenService.GetJwtSecurityToken(accessToken, validateLifetime: false);
                var refreshJwtSecurToken = _tokenService.GetJwtSecurityToken(refreshToken, validateLifetime: true);

                if (!Equals(employee.Name, accessJwtsecurToken?.Payload["name"] ?? "") || !Equals(employee.Name, refreshJwtSecurToken?.Payload["name"] ?? ""))
                {
                    return Unauthorized("Invalid token");
                }
                var newAccessToken = ProduceAccessToken(employee);

                return Json(new { accessToken = newAccessToken, employeeId });
            }
            catch (SecurityTokenException stex)
            {
                return Unauthorized(stex.Message);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("signout")]
        public IActionResult Signout()
        {
            var username = Request.Cookies[CookiesKeys.EMPLOYEE_NAME];
            Response.Cookies.Delete(CookiesKeys.REFRESH_TOKEN, _cookieOptions);
            Response.Cookies.Delete(CookiesKeys.EMPLOYEE_NAME, _cookieOptions);
            Response.Cookies.Delete(CookiesKeys.EMPLOYEE_ID, _cookieOptions);

            _logger.LogInformation("{requestMethod}:{requestPath}: User \"{username}\" was sign out", Request.Method, Request.Path, username);
            return Ok();
        }

        private string ProduceAccessToken(Employee employee)
        {
            var accessToken = _tokenService.GenerateToken(employee.Name, employee.Role.ToString());
            var refreshToken = _tokenService.GenerateRefreshToken(employee.Name);

            Response.Cookies.Append(CookiesKeys.REFRESH_TOKEN, refreshToken, _cookieOptions);
            Response.Cookies.Append(CookiesKeys.EMPLOYEE_NAME, employee.Name, _cookieOptions);
            Response.Cookies.Append(CookiesKeys.EMPLOYEE_ID, employee.Id.ToString(), _cookieOptions);

            return accessToken;
        }

        private Employee GetAuthEmployee(string name, string password)
        {
            var employee = _context.Employees.FirstOrDefault(x => x.Name == name);

            if (employee != null && _passwordHasher.IsPasswordValid(employee.Password, password))
            {
                return employee;
            }

            return null;
        }
    }
}