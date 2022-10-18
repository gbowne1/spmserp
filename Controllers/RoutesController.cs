using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using spmserp.ApplicationServices.DTOs;
using System.Reflection;
using spmserp.ApplicationServices.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace spmserp.Admin.Controllers
{
    [Authorize(Roles = "Admin, SuperUser")]
    public class RoutesController : BaseMvcController
    {
        private readonly IRouteService routeService;
        private readonly IStationService stationService;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public RoutesController(IRouteService routeService, IStationService stationService)
        {
            this.routeService = routeService;
            this.stationService = stationService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            log.Info(nameof(RoutesController.Index));

            try
            {
                var routes = routeService.GetRoutes();

                return View(routes);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Details(int? id)
        {
            log.Info(nameof(RoutesController.Details));

            try
            {
                if (id == null)
                {
                    log.Warn(nameof(RoutesController.Details) + " id is null");

                    return NotFound();
                }

                var route = routeService.GetRouteById(id.Value);

                if (route == null)
                {
                    log.Warn(nameof(RoutesController.Details) + " route is null");

                    return NotFound();
                }

                return View(route);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            log.Info(nameof(RoutesController.Create));

            try
            {
                var stationNames = stationService.GetAll();

                ViewData["StationId"] = new SelectList(stationNames, "Id", "Name");

                return View();
            }
            catch (Exception e)
            {
                log.Error(e);

                throw;
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(RouteDto routeDto)
        {
            log.Info(nameof(RoutesController.Create));

            try
            {
                if (ModelState.IsValid)
                {
                    routeService.Create(routeDto);

                    return RedirectToAction(nameof(Index));
                }

                var stationNames = stationService.GetAll();

                ViewData["StationId"] = new SelectList(stationNames, "Id", "Name", routeDto.Id);

                return View(routeDto);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            log.Info(nameof(RoutesController.Edit));

            if (id == null)
            {
                log.Warn(nameof(RoutesController.Edit) + " id is null");

                return NotFound();
            }

            try
            {
                var route = routeService.GetRouteById(id.Value);

                if (route == null)
                {
                    log.Warn(nameof(RoutesController.Edit) + " route is null");

                    return NotFound();
                }

                var stationNames = stationService.GetAll();

                ViewData["StationId"] = new SelectList(stationNames, "Id", "Name", route.Id);

                return View(route);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, RouteDto routeDto)
        {
            log.Info(nameof(RoutesController.Edit));

            if (id != routeDto.Id)
            {
                log.Warn(nameof(RoutesController.Edit) + " id isn't equal to routeDto.Id");

                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    routeService.Update(routeDto);

                    return RedirectToAction(nameof(Index));
                }

                var stationNames = stationService.GetAll();

                ViewData["StationId"] = new SelectList(stationNames, "Id", "Name", routeDto.Id);

                return View(routeDto);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            log.Info(nameof(RoutesController.Delete));

            if (id == null)
            {
                log.Warn(nameof(RoutesController.Delete) + " id is null");

                return NotFound();
            }

            try
            {
                var route = routeService.GetRouteById(id.Value);

                if (route == null)
                {
                    return NotFound();
                }

                return View(route);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            log.Info(nameof(RoutesController.DeleteConfirmed));

            try
            {
                routeService.Delete(id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpGet]
        public IActionResult GetRoutesList()
        {
            log.Info(nameof(RoutesController.GetRoutesList));

            try
            {
                var routes = routeService.GetRoutes();

                return Json(routes);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }
    }
}