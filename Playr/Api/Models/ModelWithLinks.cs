using System.Collections.Generic;

namespace Playr.Api.Models
{
    public abstract class ModelWithLinks
    {
        protected ModelWithLinks()
        {
            Links = new List<Link>();
        }

        public List<Link> Links { get; set; }

        public void AddLink(string rel, string href)
        {
            Links.Add(new Link { Rel = rel, Href = href });
        }
    }
}
