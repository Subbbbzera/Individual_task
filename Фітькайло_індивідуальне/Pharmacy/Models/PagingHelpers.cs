using Pharmacy.Models;
using System;
using System.Text;
using System.Web.Mvc;

namespace Pharmacy.Helpers
{
    public static class PagingHelpers
    {
        public static MvcHtmlString PageLinks(this HtmlHelper html,
        PageInfo pageInfo, Func<int, string> pageUrl)
        {
            StringBuilder result = new StringBuilder();
            TagBuilder ulTag = new TagBuilder("ul");
            ulTag.AddCssClass("pagination");

            for (int i = 1; i <= pageInfo.TotalPages; i++)
            {
                TagBuilder liTag = new TagBuilder("li");

                if (i == pageInfo.PageNumber)
                    liTag.AddCssClass("active");

                TagBuilder aTag = new TagBuilder("a");
                aTag.MergeAttribute("href", pageUrl(i));
                aTag.SetInnerText(i.ToString());

                liTag.InnerHtml = aTag.ToString();
                result.Append(liTag.ToString());
            }

            ulTag.InnerHtml = result.ToString();
            return MvcHtmlString.Create(ulTag.ToString());
        }
    }
}
