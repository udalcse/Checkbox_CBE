using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Checkbox.Analytics;
using Checkbox.Forms;
using Checkbox.Globalization.Text;
using Checkbox.Users;
using Checkbox.Web.Page;
using Prezza.Framework.Security;

namespace CheckboxWeb.Controls.Navigation
{
    public partial class BreadCrumbNavigator : Checkbox.Web.Common.UserControlBase
    {
        private ResponseTemplate _responseTemplate;
        private AnalysisTemplate _analysisTemplate;
        private int _responseTemplateId;
        private int _analysisTemplateId;

        /// <summary>
        /// Get a reference to the response template.
        /// </summary>
        private ResponseTemplate ResponseTemplate
        {
            get
            {
                if (_responseTemplateId <= 0 && Page is ResponseTemplatePage)
                {
                    _responseTemplateId = (Page as ResponseTemplatePage).ResponseTemplateId;
                }

                if (_responseTemplate == null && _responseTemplateId > 0)
                {
                    _responseTemplate = ResponseTemplateManager.GetResponseTemplate(_responseTemplateId);
                }
                
                return _responseTemplate;
            }
        }

        /// <summary>
        /// Get a reference to the response template.
        /// </summary>
        private AnalysisTemplate AnalysisTemplate
        {
            get
            {
                if (_analysisTemplate == null && _analysisTemplateId > 0)
                {
                    _analysisTemplate = AnalysisTemplateManager.GetAnalysisTemplate(_analysisTemplateId, true);
                }

                return _analysisTemplate;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            //try to parse analysis template id 
            int.TryParse(Request.Params["r"], out _analysisTemplateId);

            //try to parse template template id 
            if (!int.TryParse(Request.Params["s"], out _responseTemplateId))
            {
                if (AnalysisTemplate != null)
                    _responseTemplateId = AnalysisTemplate.ResponseTemplateID;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool ShowPanel
        {
            get
            {
                var node = CurrentNode;
                if (node == null)
                    return false;

                do
                {
                    node = node.ParentNode;

                    if (node != null && node["key"] == "dashboard")
                        return true;

                } while (node != null);

                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> BuildCrumbs()
        {
            Dictionary<string, string> crumbs = new Dictionary<string, string>();

            if (!ShowPanel)
                return crumbs;

            List<SiteMapNode> nodes = new List<SiteMapNode>();

            var currentNode = CurrentNode;
            while (currentNode != null && currentNode.ParentNode != null)
            {
                nodes.Add(currentNode);
                currentNode = currentNode.ParentNode;
            }

            if (nodes.Any(n => n["key"] == "dashboard"))
            {
                nodes.Reverse();
                
                foreach (SiteMapNode node in nodes)
                {
                    string nodeKey = node["key"];
                    
                    //if there is not single survey, skip it
                    if (nodeKey == "editing" && (_responseTemplateId <= 0 ||
                        !AuthorizationFactory.GetAuthorizationProvider().Authorize(UserManager.GetCurrentPrincipal(), ResponseTemplate, "Form.Edit")))
                        continue;
                    
                    //if we are in the report editor, will use its name 
                    string name = string.Empty;
                    if (nodeKey == "reportedit" && AnalysisTemplate != null)
                        name = AnalysisTemplate.Name;
                    else if (ResponseTemplate != null) // use survey name
                        name = ResponseTemplate.Name;

                    string text;

                    bool usePrefix;
                    if ((bool.TryParse(node["prefix"], out usePrefix) && usePrefix) 
                        || node == nodes.Last()) //always use the prefix if the crumb is the last
                    {
                        string prefix = TextManager.GetText("/controlText/breadcrumbnavigator/" + nodeKey);
                        /*if (node == nodes.Last())
                            prefix = "<span class=\"prefix\">" + prefix + "</span>";*/
                        text = prefix + name;
                    }
                    else if (nodeKey == "dashboard" || string.IsNullOrEmpty(name)) //use single prefix if there is no name or it's the dashboard
                        text = TextManager.GetText("/controlText/breadcrumbnavigator/" + nodeKey);
                    else
                        text = name;

                    string param = _responseTemplateId > 0 ?
                        "?s=" + _responseTemplateId : string.Empty;

                    crumbs.Add(node.Url + param, text);
                }
            }

            return crumbs;
        }

        /// <summary>
        /// 
        /// </summary>
        private SiteMapNode CurrentNode
        {
            get
            {
                try
                {
                    return FindNodeByPath(SiteMap.Provider.RootNode, Request.Path);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Recursively finds site map node by given url
        /// </summary>
        /// <param name="node"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private SiteMapNode FindNodeByPath(SiteMapNode node, string path)
        {
            SiteMapProvider map = SiteMap.Provider;

            if (path.Contains(node.Url))
                return node;
            
            foreach (SiteMapNode child in map.GetChildNodes(node))
            {
                var subchild = FindNodeByPath(child, path);
                if (subchild != null && path.Contains(subchild.Url))
                    return subchild;
            }

            return null;
        }
    }
}