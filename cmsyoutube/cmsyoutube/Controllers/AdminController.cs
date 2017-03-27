using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using cmsyoutube.Models;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace cmsyoutube.Controllers
{
    public class AdminController : Controller
    {
        private cmsyoutuEntities db = new cmsyoutuEntities();
        // GET: Admin
        public ActionResult Index()
        {
            if (Configs.getCookie("logincms") == "")
            {
                return RedirectToAction("login");
            }
            return View();
        }

        public ActionResult login()
        {
            if (Configs.getCookie("logincms") != "")
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        

        [HttpPost]
        public ActionResult Login(user login)
        {
            int ok = 0;
            if (login != null)
            {
                MD5 md5Hash = MD5.Create();
                var pass = Configs.GetMd5Hash(md5Hash, login.pass);
                var p = (from q in db.users where q.root.Contains(login.root) && q.pass.Contains(pass) select q.root).SingleOrDefault();
                if (p != null && p != "")
                {
                    //Ghi ra cookie
                    Configs.setCookie("logincms", "login" + p);
                    ok = 1;
                }
            }
            return Json(ok, JsonRequestBehavior.AllowGet);
        }

        //title - eow-title
        //thumbnail - thumbnaiUrl
        //description - <p id="eow-description"
        //tags - keywords":
        //views count watch-view-count
        //dislike count - dislike this video along with
        //published Date class=\"watch-time-text\"

        public class result
        {
            public string link { get; set; }
            public string name { get; set; }
            public string code { get; set; }
            public string img { get; set; }
            public string des { get; set; }
            public string tags { get; set; }
            public string date { get; set; }
            public string view { get; set; }
        }

        public string getcatvideo(string keyword)
        {
            if (keyword == null) keyword = "";
            
            var p = (from q in db.cats orderby q.name ascending select new { label = q.name, value = q.id }).ToList().Distinct();
            return JsonConvert.SerializeObject(p);
        }

        [HttpPost]
        public ActionResult getLink(int? id,string catvideo, string linktube)
        {
            int? newcatid = 0;
            if (id == 0)
            {
                cat newcat = new cat();
                newcat.name = catvideo ?? null;
                db.cats.Add(newcat);
                db.SaveChanges();
                newcatid = newcat.id;
            }
            else
            {
                newcatid = id;
            }
            int? user_id = 0;
            if (Configs.getCookie("logincms") != "")
            {
                string xx = Configs.getCookie("logincms").Replace("login", "").Trim();
                var user = db.users.Where(x => x.root == xx).FirstOrDefault();
                if (user != null)
                {
                    user_id = user.id;
                }
            }

            if (linktube == null) linktube = "https://www.youtube.com/watch?v=Gro7jsVFNSE";
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(linktube);
            HttpWebResponse res = (HttpWebResponse)req.GetResponse();
            StreamReader sr = new StreamReader(res.GetResponseStream());
            var data = sr.ReadToEnd();

            //get video
            // NO view-source:https://www.youtube.com/watch?v=Gro7jsVFNSE
            // YES view-source:https://www.youtube.com/v/Gro7jsVFNSE

            string video = linktube.Replace("watch?v=", "v/");
            string codevideo = video.Replace("https://www.youtube.com/v/", "").Trim();
            //get image

            // Thừa thì cộng thêm thiếu thì trừ đi
            string imgpath = Search(data, "thumbnailUrl", 20, "\">");
            //get title
            string title = Search(data, "eow-title", 48, "\">");
            if (title.Contains("\""))
                title = Search(data, "eow-title", 60, "\">");

            //get description
            string description = Search(data, "<p id=\"eow-description\"", 34, "</p>");

            //get tags
            string tags = Search(data, "keywords\":", 11, "\"");

            //get view count
            string viewcount = Search(data, "watch-view-count", 18, "</");

            // get date
            string date = Search(data, "class=\"watch-time-text\"", 24, "</");

            var _result = new result()
            {
                link = linktube,
                name = title,
                code = codevideo,
                img = imgpath,
                des = description,
                date = date,
                tags = tags,
                view = viewcount
            };
            video newvideo = new video();
            newvideo.cat_id = newcatid ?? null;
            newvideo.code = _result.code ?? null;
            newvideo.create_date = DateTime.Now;
            newvideo.date = _result.date ?? null;
            newvideo.des = _result.des ?? null;
            newvideo.img = _result.img ?? null;
            newvideo.link = _result.link ?? null;
            newvideo.name = _result.name ?? null;
            newvideo.tag = _result.tags ?? null;
            newvideo.user_id = user_id ?? null;
            newvideo.viewcount = _result.view ?? null;
            db.videos.Add(newvideo);
            db.SaveChanges();

            return Json(_result, JsonRequestBehavior.AllowGet);
        }

        public string Search(String data, string key, int i, string stop)
        {
            string source = data;
            var x = source.IndexOf(key);
            string extract = source.Substring(source.IndexOf(key) + i);
            string result = extract.Substring(0, extract.IndexOf(stop));
            return result;
        }

        public string DeleteHtmlTags(string value)
        {
            var str1 = Regex.Replace(value, @"<[^>]+|&nbsp;", "").Trim();
            var str2 = Regex.Replace(str1, @"\s{2,}", " ");
            return str2;
        }
    }
}