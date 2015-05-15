using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FizzWare.NBuilder;
using Microsoft.SqlServer.Server;

namespace WebPlayground.Controllers
{
    public class AllocationController : Controller
    {
        // GET: Allocations
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Upload()
        {
            var allocations = Builder<Allocation>
                .CreateListOfSize(1000)
                .All().Do(a => a.AccountId = ((int)a.Weight % 20).ToString())
                .All().Do(a => a.Message = (int)a.Weight % 8 == 0 ? "Validation Error" : "")
                .Build()
                .AsEnumerable();

            return View(allocations);
        }

        public ActionResult Process()
        {
            return View();
        }
    }


    public class Allocation
    {
        public string AccountId { get; set; }
        public string Model { get; set; }
        public double Weight { get; set; }
        public bool PreserveDrift { get; set; }
        public string Message { get; set; }
    }
}