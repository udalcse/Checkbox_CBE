using System.Linq;
using System.Web.Optimization;

namespace CheckboxWeb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            BundleTable.EnableOptimizations = true;

            RegisterPageView(bundles);
        }


        private static void RegisterPageView(BundleCollection bundles)
        {
            Bundle pageViewBundle = new ScriptBundle("~/bundles/PageView.js").Include(
              "~/Resources/json2.min.js",
              "~/Resources/jquery-ui-1.10.2.custom.min.js",
              "~/Resources/jquery.tmpl.min.js",
              "~/Resources/DialogHandler.js",
              "~/Resources/jquery.watermark.js",
              "~/Resources/jquery.uniform.min.js",
              "~/Resources/jquery.simplemodal.1.4.1.min.js",
              "~/Resources/jquery.tinyscrollbar.min.js",
              "~/Resources/jquery.mousewheel.min.js",
              "~/Resources/jquery-ui-timepicker-addon.js",
              "~/Resources/jquery-ui-combobox.js",
              "~/Services/js/serviceHelper.js",
              "~/Services/js/svcSearch.js",
              "~/Resources/search.js",
              "~/Resources/custom.js",
              "~/Resources/gridLiveSearch.js",
              "~/Resources/tiny_mce/jquery.tinymce.min.js",
              "~/Resources/globalHelper.js",
              "~/Resources/surveyBindedMatrix.js");

            bundles.Add(pageViewBundle);
        }
    }
}