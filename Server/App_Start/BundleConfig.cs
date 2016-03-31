using System.Web.Optimization;

namespace Server
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/loading-bar.min.css",
                      "~/Content/normalize.min.css",
                      "~/Content/main.css"));

            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                "~/Scripts/jquery-2.2.1.min.js",
                "~/Scripts/angular.js",
                "~/Scripts/ng-infinite-scroll.min.js",
                "~/Scripts/angular-sanitize.min.js",
                "~/Scripts/angular-animate.min.js",
                "~/Scripts/loading-bar.min.js",
                "~/Scripts/angular-route.js",
                "~/Scripts/shufflecode.js",
                "~/lib/app.js"));

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
        }
    }
}
