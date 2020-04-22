using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SimpleAdsNew.Data;
using SimpleAdsNew.Models;

namespace SimpleAdsNew.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString =
            "Data Source=.\\sqlexpress;Initial Catalog=SimpleAds;Integrated Security=True";
        public IActionResult Index()
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            UserDb userDB = new UserDb(_connectionString);
            IEnumerable<SimpleAd> ads = db.GetAds();
            List<AdViewModel> a = new List<AdViewModel>();
            bool Delete = false;
            if (User.Identity.Name != null)
            {
                foreach (SimpleAd s in ads)
                {
                    if(s.UserId == userDB.GetByEmail(User.Identity.Name).Id)
                    {
                        Delete = true;
                    }
                    AdViewModel avm = new AdViewModel
                    {
                        Ad = s,
                        CanDelete = Delete
                    };
                    a.Add(avm);

                }
            }
            else
            {
                foreach (SimpleAd s in ads)
                {
                    AdViewModel avm = new AdViewModel
                    {
                        Ad = s,
                        CanDelete = false
                    };
                    a.Add(avm);
                }
                
            }
            
        return View(a);
        }

        public IActionResult NewAd()
        {
            UserDb userDB = new UserDb(_connectionString);
            int Id = userDB.GetByEmail(User.Identity.Name).Id;
            return View(Id);
        }
        [Authorize]
        [HttpPost]
        public ActionResult NewAd(SimpleAd ad)
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            UserDb userDB = new UserDb(_connectionString);
            ad.UserId = userDB.GetByEmail(User.Identity.Name).Id;
            db.AddSimpleAd(ad);

            return Redirect("/");
        }
        [Authorize]
        [HttpPost]
        public IActionResult DeleteAd(int id)
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            db.Delete(id);
            return Redirect("/");
        }
        [Authorize]
        public IActionResult MyAccount()
        {
            SimpleAdDb db = new SimpleAdDb(_connectionString);
            UserDb userDB = new UserDb(_connectionString);
            List<SimpleAd> myAds = new List<SimpleAd>();
            IEnumerable<SimpleAd> ads = db.GetAds();
            foreach(SimpleAd a in ads)
            {
                if(a.UserId == userDB.GetByEmail(User.Identity.Name).Id)
                {
                    myAds.Add(a);
                }
            }
            return View(myAds);
        }
    }

    
}
