using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using UniversityRegistrar.Models;

namespace UniversityRegistrar.Controllers
{
    public class HomeController : Controller
    {
      [HttpGet("/")]
      public ActionResult Index()
      {
        return View();
      }
    }
}
