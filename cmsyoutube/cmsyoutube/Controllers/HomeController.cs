using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using cmsyoutube.Models;

namespace cmsyoutube.Controllers
{
    public class HomeController : Controller
    {
        private cmsyoutuEntities db = new cmsyoutuEntities();

        public ActionResult Index(int? pg, string search, int? catid)
        {
            int pageSize = 25;
            if (pg == null) pg = 1;
            int pageNumber = (pg ?? 1);
            ViewBag.pg = pg;
            var data = (from q in db.videos orderby q.id descending select q).ToList();
            if (data == null)
            {
                return View(data);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                //data = data.Where(x => x.name.Contains(search)).ToList();
                var aa = search.Split(' ');
                data = data.Where(o => aa.Any(w => o.name.Contains(w))).ToList();
                ViewBag.search = search;
            }
            if (catid != null)
            {
                data = data.Where(x => x.cat_id == catid).ToList();
                ViewBag.catid = catid;
            }

            return View(data.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult decodejs(string input)
        {
            return Json("");
        }

        public ActionResult About()
        {
            ViewBag.Message = "cms Youtu - ngnam";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string getdanhmuc()
        {
            string carsize = "";
            try
            {
                var p = (from q in db.cats orderby q.name ascending select q).Distinct().ToList();
                for (int i = 0; i < p.Count(); i++)
                {
                    carsize += "<option value=\"" + p[i].id + "\">" + p[i].name + "</option>";
                }

            }
            catch (Exception ex)
            {
            }
            return carsize;
        }
    }
}