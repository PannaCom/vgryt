using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace cmsyoutube
{
    public static class Configs
    {
        public static void setCookie(string field, string value)
        {
            HttpCookie MyCookie = new HttpCookie(field);
            MyCookie.Value = value;
            MyCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(MyCookie);
            //Response.Cookies.Add(MyCookie);   

        }
        public static string getCookie(string v)
        {
            try
            {
                return HttpContext.Current.Request.Cookies[v].Value.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static void RemoveCookie(string v)
        {
            if (HttpContext.Current.Request.Cookies[v] != null)
            {
                var c = new HttpCookie(v);
                c.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Current.Response.Cookies.Add(c);
            }
        }

        public static string GetMd5Hash(MD5 md5Hash, string input)
        {
            if (input == "" || input == null) input = "chanhniem";
            // Convert the input string to a byte array and compute the hash. 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string. 
            return sBuilder.ToString();
        }
    }
}