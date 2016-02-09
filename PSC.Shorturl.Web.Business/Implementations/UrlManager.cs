using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSC.Shorturl.Web.Data;
using PSC.Shorturl.Web.Entities;
using PSC.Shorturl.Web.Exceptions;

namespace PSC.Shorturl.Web.Business.Implementations
{
    public class UrlManager : IUrlManager
    {
        public Task<ShortUrl> ShortenUrl(string longUrl, string ip, string segment = "")
        {
            return Task.Run(() =>
            {
                using (var ctx = new ShorturlContext())
                {
                    ShortUrl url;

                    url = ctx.ShortUrls.Where(u => u.LongUrl == longUrl).FirstOrDefault();
                    if (url != null)
                    {
                        return url;
                    }

                    if (!string.IsNullOrEmpty(segment))
                    {
                        if (ctx.ShortUrls.Where(u => u.Segment == segment).Any())
                        {
                            throw new ShorturlConflictException();
                        }
                    }
                    else
                    {
                        segment = this.NewSegment();
                    }

                    if (string.IsNullOrEmpty(segment))
                    {
                        throw new ArgumentException("Segment is empty");
                    }

                    url = new ShortUrl()
                    {
                        Added = DateTime.Now,
                        Ip = ip,
                        LongUrl = longUrl,
                        NumOfClicks = 0,
                        Segment = segment
                    };

                    ctx.ShortUrls.Add(url);

                    ctx.SaveChanges();

                    return url;
                }
            });
        }

        public Task<Stat> Click(string segment, string referer, string ip)
        {
            return Task.Run(() =>
            {
                using (var ctx = new ShorturlContext())
                {
                    ShortUrl url = ctx.ShortUrls.Where(u => u.Segment == segment).FirstOrDefault();
                    if (url == null)
                    {
                        throw new ShorturlNotFoundException();
                    }

                    url.NumOfClicks = url.NumOfClicks + 1;

                    Stat stat = new Stat()
                    {
                        ClickDate = DateTime.Now,
                        Ip = ip,
                        Referer = referer,
                        ShortUrl = url
                    };

                    ctx.Stats.Add(stat);

                    ctx.SaveChanges();

                    return stat;
                }
            });
        }

        private string NewSegment()
        {
            using (var ctx = new ShorturlContext())
            {
                int i = 0;
                while (true)
                {
                    string segment = Guid.NewGuid().ToString().Substring(0, 6);
                    if (!ctx.ShortUrls.Where(u => u.Segment == segment).Any())
                    {
                        return segment;
                    }
                    if (i > 30)
                    {
                        break;
                    }
                    i++;
                }
                return string.Empty;
            }
        }
    }
}