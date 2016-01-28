using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC.Shorturl.Web.Business.Implementations
{
    public class UrlManager : IUrlManager
    {
        public Task<string> ShortenUrl(string longUrl)
        {
            return Task.Run(() =>
            {
                return "http://www.google.com";
            });
        }
    }
}