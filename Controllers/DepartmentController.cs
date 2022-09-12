
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace spmserp.Controllers
{
    [Area("Master")]
    public class DepartmentController : Controller
    {
        private readonly IDepartment _department;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="department"></param>
        public DepartmentController(IDepartment department)
        {
            this._department = department;
        }

        /// <summary>
        /// department list.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            return await Task.Run(() =>
            {
                return View();
            });
        }

        /// <summary>
        /// get department list.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> GetDepartmentList()
        {
            DataTableResultModel<DepartmentModel> resultModel = await _department.GetDepartmentList();

            return await Task.Run(() =>
            {
                return Json(new { draw = 1, data = resultModel.ResultList });
            });
        }

        /// <summary>
        /// add new department.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PartialViewResult> AddDepartment()
        {
            return await Task.Run(() =>
            {
                return PartialView("_AddDepartment", new DepartmentModel());
            });
        }

        /// <summary>
        /// edit department.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<PartialViewResult> EditDepartment(int departmentId)
        {
            DepartmentModel departmentModel = await _department.GetDepartmentById(departmentId);

            return PartialView("_AddDepartment", departmentModel);
        }

        /// <summary>
        /// save department.
        /// </summary>
        /// <param name="departmentModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> SaveDepartment(DepartmentModel departmentModel)
        {
            JsonData<JsonStatus> data = new JsonData<JsonStatus>(new JsonStatus());

            if (ModelState.IsValid)
            {
                if (departmentModel.DepartmentId > 0)
                {
                    // update record.
                    if (true == await _department.UpdateDepartment(departmentModel))
                    {
                        data.Result.Status = true;
                        data.Result.Message = "Record saved successfully";
                    }
                    else
                    {
                        data.Result.Message = "Record not saved";
                    }
                }
                else
                {
                    // add new record.
                    if (await _department.CreateDepartment(departmentModel) > 0)
                    {
                        data.Result.Status = true;
                        data.Result.Message = "Record saved successfully";
                    }
                    else
                    {
                        data.Result.Message = "Record not saved";
                    }
                }
            }

            return Json(data);
        }

        

        /// <summary>
        /// delete department by departmentid.
        /// </summary>
        /// <param name="departmentId"></param>
        /// <returns>
        /// return json.
        /// </returns>
        [HttpPost]
        public async Task<JsonResult> DeleteDepartment(int departmentId)
        {
            JsonData<JsonStatus> data = new JsonData<JsonStatus>(new JsonStatus());
            if (true == await _department.DeleteDepartment(departmentId))
            {
                data.Result.Status = true;
                data.Result.Message = "Record deleted successfully";
            }
            else
            {
                data.Result.Message = "Record not deleted";
            }

            return Json(data); // returns.
        }
    }
}