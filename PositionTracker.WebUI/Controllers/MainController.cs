using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PositionTracker.WebUI.Hub;
using PositionTracker.WebUI.Models;

namespace PositionTracker.WebUI.Controllers
{
    public class MainController : Controller
    {
        public static IHubContext<MainHub> MainHub;

        public MainController(IHubContext<MainHub> mainHub, PositionController positionController)
        {
            MainHub = mainHub;
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Position()
        {
            return View("Position");
        }
    }
}