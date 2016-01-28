using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSC.Shorturl.Web.Business
{
    public interface IUrlManager
    {
        Task<string> ShortenUrl(string longUrl);
    }
}