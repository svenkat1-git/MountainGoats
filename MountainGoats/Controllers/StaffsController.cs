using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using MountainGoatsBikes.Repositories;

namespace MountainGoatsBikes.Controllers
{
    public class StaffsController : Controller
    {
        private readonly Repository _repository;

        public StaffsController(Repository repository)
        {
            _repository = repository;
        }

        public IActionResult Index()
        {
            var staffs = _repository.GetStaffs();
            return View(staffs);
        }
    }
}
