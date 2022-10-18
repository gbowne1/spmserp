using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json; // Not sure about adding Newton
using spmserp.Data;
using spmserp.Models;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Security.Claims;
using System.ServiceModel.Syndication;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace spmserp.Controllers
{
    [Authorize(Roles = "Admin, SuperUser")]
    [Area("Admin")]
    [ExceptionFilter]
    [Authorize(Policy = "RequireAdministratorRole")]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]")]
    [RequireHttps]
    [RoutePrefix("admin")]
    [ApiController]

    public class AdminController : AdminBaseController
    {
        #region 
        private readonly IConfiguration _configuration;
        private readonly ISysAdminService _adminService;
        private readonly ILoggerHelper _logger;
        private readonly ICache _cache;
        private readonly IMapper _mapper;
        private readonly ILogger<AdminController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IRoleService _roleService;
        private readonly IStringLocalizer<AdminController> _stringLocalizer;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
        
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ConfirmOperation()
        {
            return View();
        }

        public ActionResult OperationFailed()
        {
            return View();
        }
        public ActionResult ListUsers()
        {
            var model = _adminService.ListAllUsers();
            return View(model);
        }

        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }
        
        private readonly IHostingEnvironment hostingEnvironment;
        
        public AdminController(IHostingEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        [HttpGet(Name = "IsAlive")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        
        public ActionResult<int> IsAlive()
        {
            var response = "Admin API is in good health.";
            _logger.LogInformation(response);
            return Ok(response);
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View(new Admin());
        }

        [HttpPost]

        public ActionResult LogIn(Admin admin)
        {
            if (_admin.CanLogin(admin))
            {
               Admin newAdmin=  _admin.Get(a=>a.UserName==admin.UserName);
                AddCookie(newAdmin);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", ""); 
            return View(admin);
        }

        private void AddCookie(Admin admin)
        {

            if ( string.IsNullOrEmpty(Request.Cookies["id"]))
            {
                CookieOptions options = new CookieOptions();
                options.Expires = DateTime.Now.AddMinutes(40);
               
                Response.Cookies.Append("id",  admin.Id.ToString(), options);
            }
        }

        public AdminController(UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            ApplicationDbContext context,
            ICvStorageService cvStorageService,
            ILogger<AdminController> logger, 
            IStringLocalizer<AdminController> stringLocalizer, 
            IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _cvStorageService = cvStorageService;
            _logger = logger;
            _stringLocalizer = stringLocalizer;
            _mapper = mapper;
        }

        public AdminController(ILogger<AdminController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public AdminController(RoleManager<IdentityRole> roleManager)
        {
            this.roleManager = roleManager;
        }
        public AdminController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager) {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private readonly IWebHostEnvironment _web;

        // GET: AdminController
       

        public AdminController(IWebHostEnvironment web)
        {
            db = new spmsContext();
        }
        
        // GET: AdminController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AdminController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        // GET: AdminController/createrole
        public ActionResult CreateRole()
        {
            return View();
        }

        // GET: Admin
        private readonly ApplicationDbContext _dbContext;
        private readonly IHostingEnvironment _hosting;

        public ActionResult AdminDashBoard(Admin model)
        {
            List<Admin> lst = new List<Admin>();
            DataSet ds = model.GetAdminDashBoardDetails();
            if (ds != null && ds.Tables[0].Rows.Count > 0 && ds.Tables.Count > 0)
            {
                ViewBag.TotalEmployee = ds.Tables[0].Rows[0]["TotalEmployee"].ToString();
                ViewBag.TotalCustomer = ds.Tables[1].Rows[0]["TotalCustomer"].ToString();
                ViewBag.TotalVendor = ds.Tables[2].Rows[0]["TotalVendor"].ToString();
                ViewBag.TotalSaleOrder = ds.Tables[3].Rows[0]["TotalSaleOrder"].ToString();
            }
            if (ds != null && ds.Tables[4].Rows.Count > 0 && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[4].Rows)
                {
                    Admin obj = new Admin();
                    obj.FirstName = dr["FirstName"].ToString();
                    obj.LastName = dr["LastName"].ToString();
                    obj.SalesOrderNo = dr["SalesOrderNo"].ToString();
                    obj.BillNo = dr["BillNo"].ToString();
                    obj.SaleOrderDate = dr["AddedOn"].ToString();
                    lst.Add(obj);
                }
                model.lstsaleorder = lst;
            }
            return View(model);
        }

        public ActionResult AdminProfile(Admin model)
        {
            model.EmployeeId = Session["Pk_EmployeeId"].ToString();
            DataSet ds = model.GetAdminProfileDetails();
            if (ds != null && ds.Tables.Count > 0)
            {
                ViewBag.LoginId = ds.Tables[0].Rows[0]["LoginId"].ToString();
                ViewBag.Password = ds.Tables[0].Rows[0]["Password"].ToString();
                ViewBag.Name = ds.Tables[0].Rows[0]["Name"].ToString();
                ViewBag.Address = ds.Tables[0].Rows[0]["Address"].ToString();
                ViewBag.DOB = ds.Tables[0].Rows[0]["DOB"].ToString();
                ViewBag.ContactNo = ds.Tables[0].Rows[0]["ContactNo"].ToString();
                ViewBag.Email = ds.Tables[0].Rows[0]["Email"].ToString();
                ViewBag.Gender = ds.Tables[0].Rows[0]["Gender"].ToString();
                ViewBag.ProfilePic = ds.Tables[0].Rows[0]["ProfilePic"].ToString();
            }
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(Admin model)
        {
            try
            {
                model.AddedBy = Session["Pk_EmployeeId"].ToString();
                DataSet ds = model.ChangePassword();
                if (ds != null && ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows[0][0].ToString() == "1")
                    {
                        TempData["ChangePassword"] = "Password Changed Successfully!";
                    }
                    else
                    {
                        TempData["ChangePassword"] = ds.Tables[0].Rows[0]["ErrorMessage"].ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ChangePassword"] = ex.Message;
            }
            return RedirectToAction("ChangePassword", "Admin");
        }


        [Plugin("AdminController", "0.0.1")]
        public class AdminController
        {
            private readonly ILogger<AdminController> _logger;
            private readonly List<string> _admins = new();

            public AdminController(ILogger<AdminController> logger)
            {
            _logger = logger;
            }

            public bool IsAdmin(string login)
            {
            return _admins.Contains(login);
            }

            public void AddAdmin(string login)
            {
                if (!_admins.Contains(login))
                {
                    _admins.Add(login);
                    _logger.LogWarning("Added login {} as an admin", login);
                }
            }
        }
    }
}
