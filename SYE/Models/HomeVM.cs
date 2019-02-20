using System.Collections.Generic;
using GDSHelpers.Models;

namespace SYE.Models
{
    public class HomeVM
    {
        public HomeVM()
        {
            var crumbs = new List<Crumb>
            {
                new Crumb {Url = "#", Text = "Home"},
                new Crumb {Url = "#", Text = "Share Your Experience"}
            };

            var breadCrumbs = new BreadCumbs
            {
                Crumbs = crumbs
            };
            Breadcrumbs = breadCrumbs;
        }

        public BreadCumbs Breadcrumbs { get; set; }
    }
}
