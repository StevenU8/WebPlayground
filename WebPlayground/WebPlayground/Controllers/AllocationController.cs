using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
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
            var allocations = GetFakeAllocations();

            return View(allocations);
        }
        
        public ActionResult Upload2()
        {
            var allocations = GetFakeAllocations().ToList();

            var accountAllocations = allocations
                .GroupBy(a => a.AccountId).Select(b =>
                {
                    return new AccountAllocation
                    {
                        AccountId = b.Key,
                        HasErrors =
                            allocations.Where(a => a.AccountId == b.Key).Any(a => !a.ValidationMessage.IsEmpty())
                    };
                });
                    
            var allocationViewModel = new AllocationViewModel2
            {
                AccountAllocations = accountAllocations,
                Allocations =  allocations
            };

            return View(allocationViewModel);
        }
        
        public ActionResult Upload3()
        {
            var allocations = GetFakeAllocations().ToList();

            var accountAllocations = allocations
                .GroupBy(a => a.AccountId).Select(b =>
                {
                    return new AccountAllocation
                    {
                        AccountId = b.Key,
                        HasErrors = allocations.Where(a => a.AccountId == b.Key).Any(a => !a.ValidationMessage.IsEmpty())
                    };
                })
                .ToList();

            var validAccountIds = accountAllocations.Where(a => !a.HasErrors).Select(a => a.AccountId);
            var invalidAccountIds = accountAllocations.Where(a => a.HasErrors).Select(a => a.AccountId);



            var allocationViewModel = new AllocationViewModel3
            {
                ValidAllocations = allocations.Where(a => validAccountIds.Contains(a.AccountId)),
                InvalidAllocations = allocations.Where(a => invalidAccountIds.Contains(a.AccountId)),
            };

            return View(allocationViewModel);
        }

        private static IEnumerable<Allocation> GetFakeAllocations()
        {
            return Builder<Allocation>
                .CreateListOfSize(1000)
                .All().Do(a => a.AccountId = (((int)a.Weight % 20 + 1) * 12345).ToString())
                .All().Do(a => a.ValidationMessage = (int)a.Weight % 8 == 0 ? "Validation Error" : "")
                .Build()
                .AsEnumerable();
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
        public string ValidationMessage { get; set; }
    }

    public class AllocationViewModel2
    {
        public IEnumerable<AccountAllocation> AccountAllocations { get; set; }
        public IEnumerable<Allocation> Allocations { get; set; }
    }

    public class AllocationViewModel3
    {
        public IEnumerable<Allocation> ValidAllocations { get; set; }
        public IEnumerable<Allocation> InvalidAllocations { get; set; }
    }

    public class AccountAllocation
    {
        public string AccountId { get; set; }
        public bool HasErrors { get; set; }
    }
}