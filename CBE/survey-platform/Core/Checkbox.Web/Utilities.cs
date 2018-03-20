//===============================================================================
// Prezza Technologies Application Framework
// Copyright © Checkbox Survey Inc  All rights reserved.
//===============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml;
using Checkbox.Common;
using Checkbox.Management;
using Prezza.Framework.Data;

namespace Checkbox.Web
{
    /// <summary>
    /// Common routines for CheckBox web applications.
    /// </summary>
    public static class WebUtilities
    {
        #region Static Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUserEncodedName()
        {
            if (HttpContext.Current == null || HttpContext.Current.User == null)
                return string.Empty;

            return EncodeJsString(Utilities.AdvancedHtmlEncode(HttpContext.Current.User.Identity.Name));
        }

        /// <summary>
        /// Return a string containing paths to strip from rad editor inputs in certain circumstances.
        /// </summary>
        /// <param name="pathSuffix">Path suffix to append to paths to strip.</param>
        /// <returns></returns>
        /// <remarks>Internet Explorer edit mode, which is used by Rad editor or any other HTML WYSIWYG editor
        /// automatically converts URLS to absolute URLs by appending the host and path information based on
        /// the current users requests.  This can cause things like HTML invitations to fail if the user accesses
        /// the system using URL other than the app root/url in the web.config.</remarks>
        public static string GetPathsToStrip(string pathSuffix)
        {
            if (HttpContext.Current != null
                && HttpContext.Current.Request != null
                && HttpContext.Current.Request.Url != null)
            {
                //Figure out current request path
                string reqestPath = string.Format(
                    "http{0}://{1}{2}{3}",
                    HttpContext.Current.Request.IsSecureConnection ? "s" : string.Empty,
                    HttpContext.Current.Request.Url.Host,
                    HttpContext.Current.Request.ApplicationPath,
                    pathSuffix);

                //Add application path from web.config to paths to strip
                return string.Format("{0} {1}{2}", reqestPath, ApplicationManager.ApplicationPath, pathSuffix);
            }

            return string.Empty;
        }

        /// <summary>
        /// Remove app relative path from URL.  Useful for cases where WYSIWYG editors add incorrect path
        /// information to relative URLS used for token replacement in places like editing invitation messages.
        /// </summary>
        /// <param name="urlToStrip">Path with extra information to strip.</param>
        /// <param name="appRelativePath">App relative path, such as /Foo/Bar or ~/Foo/Bar.</param>
        /// <returns></returns>
        public static string StripAppRelativePathFromUrl(string urlToStrip, string appRelativePath)
        {
            //Do nothing for null values
            if (string.IsNullOrEmpty(urlToStrip)
                || string.IsNullOrEmpty(appRelativePath)
                || HttpContext.Current == null
                || HttpContext.Current.Request == null
                || HttpContext.Current.Request.UrlReferrer == null
                || HttpContext.Current.Request.UrlReferrer.Segments == null 
                || HttpContext.Current.Request.UrlReferrer.Segments.Length < 1)
            {
                return urlToStrip;
            }

            string fullRequestPath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
            string lastURLSegment = HttpContext.Current.Request.Url.Segments[HttpContext.Current.Request.Url.Segments.Length - 1];
            string httpUrlFolder = null;
            int idx = fullRequestPath.LastIndexOf(lastURLSegment);
            if (idx > 0)
            {
                httpUrlFolder = fullRequestPath.Substring(0, idx);
            }
            else
            {
                //Convert relative path to absolute form
                httpUrlFolder =
                    string.Format(
                        "http://{0}{1}",
                        HttpContext.Current.Request.Url.Host,
                        ResolveUrl(appRelativePath));
            }

            //Now strip
            var res = Regex.Replace(
                urlToStrip,
                Regex.Escape(httpUrlFolder),
                string.Empty,
                RegexOptions.IgnoreCase);

            //case of SSL Offloading: client interacts with HTTPS whereas the server recieves HTTP requests from the proxy:
            httpUrlFolder = httpUrlFolder.Replace("http://", "https://");
            res = Regex.Replace(
                            res,
                            Regex.Escape(httpUrlFolder),
                            string.Empty,
                            RegexOptions.IgnoreCase);
            return res;
        }

        /// <summary>
        /// Populates a <see cref="ListControl"/> using an <see cref="XmlNodeList"/> 
        /// to derive the <see cref="ListItem"/> text and value properties
        /// </summary>
        /// <param name="list">The <see cref="ListControl"/> to populate</param>
        /// <param name="xml">The <see cref="XmlNodeList"/> to derive values from</param>
        /// <param name="textAttribute">The string name of the <see cref="XmlAttribute"/> within the node to use for the 
        /// <see cref="ListItem"/> Text property</param>
        /// <param name="valueAttribute">The string name of the <see cref="XmlAttribute"/> within the node to use for the 
        /// <see cref="ListItem"/> Value property</param>
        /// <returns>The list populated by the Xml</returns>
        public static ListItemCollection PopulateListControlFromXml(ListItemCollection list, XmlNodeList xml, string textAttribute, string valueAttribute)
        {
            foreach (XmlNode x in xml)
            {
                list.Add(new ListItem(x.Attributes[textAttribute].Value, x.Attributes[valueAttribute].Value));
            }
            return list;
        }

        /// <summary>
        /// Populates a <see cref="ListControl"/> using an <see cref="XmlNodeList"/> 
        /// to derive the <see cref="ListItem"/> text and value properties
        /// </summary>
        /// <param name="list">The <see cref="ListControl"/> to populate</param>
        /// <param name="xml">The <see cref="XmlNodeList"/> to derive values from</param>
        /// <param name="textNode">the name of the node to use for the ListItem Text property</param>
        /// <param name="textNodeType">the <see cref="XmlNodeType"/> of the node to use for the ListItem Text property</param>
        /// <param name="valueNode">the name of the node to use for the ListItem Value property</param>
        /// <param name="valueNodeType">the <see cref="XmlNodeType"/> of the node to use for the ListItem Value property</param>
        /// <returns>The populated ListControl</returns>
        public static ListItemCollection PopulateListControlFromXml(ListItemCollection list, XmlNodeList xml, string textNode, XmlNodeType textNodeType,
            string valueNode, XmlNodeType valueNodeType)
        {
            foreach (XmlNode x in xml)
            {
                string liText = String.Empty;
                string liValue = String.Empty;

                if (textNodeType == XmlNodeType.Attribute)
                    liText = x.Attributes[textNode].Value;
                else if (textNodeType == XmlNodeType.Element)
                    liText = x.SelectSingleNode(textNode).InnerText;

                if (valueNodeType == XmlNodeType.Attribute)
                    liValue = x.Attributes[valueNode].Value;
                else if (valueNodeType == XmlNodeType.Element)
                    liValue = x.SelectSingleNode(valueNode).InnerText;

                list.Add(new ListItem(liText, liValue));
            }
            return list;
        }


        /// <summary>
        /// Helper to get a property from a data grid item
        /// </summary>
        /// <param name="property">Name of property to retrieve.</param>
        /// <param name="item"><see cref="DataGridItem"/> to retrieve value from.</param>
        /// <returns>Value of item.</returns>
        public static string GetPropertyFromDataGridItem(string property, DataGridItem item)
        {
            DataRowView row = item.DataItem as DataRowView;

            if (row != null)
            {
                return DbUtility.GetValueFromDataRow<object>(row.Row, property, string.Empty).ToString();
            }

            return String.Empty;
        }

        /// <summary>
        /// Helper to get a property from a grid view item
        /// </summary>
        /// <param name="property">Name of property to retrieve.</param>
        /// <param name="item"><see cref="DataGridItem"/> to retrieve value from.</param>
        /// <returns>Value of item.</returns>
        public static string GetPropertyFromGridViewItem(string property, GridViewRow item)
        {
            DataRowView row = item.DataItem as DataRowView;

            if (row != null)
            {
                return DbUtility.GetValueFromDataRow<object>(row.Row, property, string.Empty).ToString();
            }

            return String.Empty;
        }

        /// <summary>
        /// Get the root image path for checkbox
        /// </summary>
        /// <returns></returns>
        public static string GetImageRoot()
        {
            return ApplicationManager.ApplicationRoot + "/Images/";
        }


        /// <summary>
        /// Returns the first <see cref="HtmlForm"/> in a  <see cref="ControlCollection"/> of controls.
        /// If no <see cref="HtmlForm"/> is found null is returned.
        /// </summary>
        /// <param name="controls"></param>
        /// <returns></returns>
        public static HtmlForm FindHtmlForm(ControlCollection controls)
        {
            HtmlForm form = null;

            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i].GetType() == typeof(HtmlForm))
                {
                    form = (HtmlForm)controls[i];
                    i = controls.Count;
                }
            }

            return form;
        }

        /// <summary>
        /// Performs a breath first search on a <see cref="ControlCollection"/> and returns the 
        /// first <see cref="Control"/> with a matching ID.
        /// Null is returned if no match is found.
        /// </summary>
        /// <param name="controls">Collection of controls to search.</param>
        /// <param name="controlID">The control ID searched for.</param>
        /// <returns></returns>
        public static Control FindControlByID(ControlCollection controls, String controlID)
        {
            ControlCollection workingSet = null;
            var queuedControls = new Queue<ControlCollection>();

            queuedControls.Enqueue(controls);

            while (queuedControls.Count > 0)
            {
                workingSet = queuedControls.Dequeue();
                if (workingSet != null)
                {
                    foreach (Control control in workingSet)
                    {
                        if (control.ID == controlID) { return control; }
                        else if (control.Controls != null && control.Controls.Count > 0)
                        {
                            queuedControls.Enqueue(control.Controls);
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Convert the dateTime to the client's time zone
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ConvertToClientTimeZone(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return ConvertToClientTimeZone(dateTime.Value);
        }

        /// <summary>
        /// Convert the dateTime to the client's time zone
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertToClientTimeZone(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime.AddHours(ApplicationManager.AppSettings.TimeZone - ApplicationManager.ServersTimeZone), DateTimeKind.Unspecified);
        }

        /// <summary>
        /// Convert the dateTime from client's side time zone to the server's side.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime? ConvertFromClientToServerTimeZone(DateTime? dateTime)
        {
            if (!dateTime.HasValue)
                return null;

            return ConvertFromClientToServerTimeZone(dateTime.Value);
        }

        /// <summary>
        /// Convert the dateTime from client's side time zone to the server's side.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTime ConvertFromClientToServerTimeZone(DateTime dateTime)
        {
            return dateTime.AddHours(-(ApplicationManager.AppSettings.TimeZone - ApplicationManager.ServersTimeZone));
        }

        /// <summary>
        /// Returns a site relative HTTP path from a partial path starting out with a ~.
        /// Same syntax that ASP.Net internally supports but this method can be used
        /// outside of the Page framework.
        /// 
        /// Works like Control.ResolveUrl including support for ~ syntax
        /// but returns an absolute URL.
        /// </summary>
        /// <param name="originalUrl">Any Url including those starting with ~</param>
        /// <returns>relative url</returns>
        public static string ResolveUrl(string originalUrl)
        {
            if (string.IsNullOrEmpty(originalUrl))
            {
                return null;
            }

            // *** Absolute path - just return    
            if (originalUrl.IndexOf("://") != -1)
            {
                return originalUrl;
            }

            // *** Fix up image path for ~ root app dir directory   
            if (originalUrl.StartsWith("~"))
            {
                string newUrl;
                if (HttpContext.Current != null)
                {
                    // *** Just to be sure fix up any double slashes        
                    newUrl = (HttpContext.Current.Request.ApplicationPath + originalUrl.Substring(1)).Replace("//", "/");
                }
                else
                {
                    // *** Not context: assume current directory is the base directory            
                    throw new ArgumentException("Invalid URL: Relative URL not allowed.");
                }

                return newUrl;
            }

            return originalUrl;
        }

        ///<summary>
        /// Convert c# string with special symbols to javascript
        ///</summary>
        ///<param name="s"></param>
        ///<returns></returns>
        public static string EncodeJsString(string s)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in s)
            {
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    default:
                        int i = c;
                        if (i < 32 || i > 127)
                            sb.AppendFormat("\\u{0:X04}", i);
                        else
                            sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }


        /// <summary>
        /// Use to detect if client's browser is mobile 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsBrowserMobile(HttpRequest request)
        {
            if (request == null)
                return false;

            string u = request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"(android|bb\d+|meego).+mobile|sm-t[0-9]*|avantgo|bada\/|blackberry|nexus|ipad|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-,)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return b.IsMatch(u) || v.IsMatch(u.Substring(0, 4));
        }

        /// <summary>
        /// Check if ajaxifying should be enabled
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static bool IsAjaxifyingSupported(HttpRequest request)
        {
            if (request == null)
                return false;

            //ajaxifying causes issues for old IEs
            return request.Browser.Browser != "IE" || request.Browser.MajorVersion >= 9;
        }

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public static void SetCookie(string name, string value)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[name] ?? new HttpCookie(name);

            cookie.Value = value;

            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        /// <summary>
        /// Gets the request form value by substring.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static string GetRequestFormValueBySubstring(string name)
        {
            if (HttpContext.Current != null)
            {
                var context = HttpContext.Current;

                var key = context.Request.Form.AllKeys.FirstOrDefault(item => item.Contains(name));

                if (!string.IsNullOrEmpty(key))
                    return context.Request.Form[key];
            }

            return null;
        }



        /// <summary>
        /// Gets the request form value by substring.
        /// </summary>
        /// <param name="substring">The substring.</param>
        /// <returns></returns>
        public static List<string> GetRequestFormValuseBySubstring(string substring)
        {
            if (HttpContext.Current != null)
            {
                var request = HttpContext.Current.Request;

                return
                    request.Form.AllKeys.Where(
                        key =>
                            key.Contains(substring)).Select(key => request.Form[key]).ToList();
            }

            return new List<string>();
        }

        /// <summary>
        /// Determines whether [has request form value] [the specified name].
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>
        ///   <c>true</c> if [has request form value] [the specified name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool HasRequestFormValue(string name)
        {
            if (HttpContext.Current != null)
            {
                var context = HttpContext.Current;

                var key = context.Request.Form.AllKeys.FirstOrDefault(item => item.Contains(name));

                return key != null;

            }

            return false;
        }

        /// <summary>
        /// Converts the relative URL to absolute URL.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <returns></returns>
        public static Uri ConvertRelativeUrlToAbsoluteUrl(string relativeUrl)
        {
            var applicationUrl = $"http{((HttpContext.Current.Request.IsSecureConnection) ? "s" : "")}://{HttpContext.Current.Request.Url.Host}";

            return new Uri(new Uri(applicationUrl),relativeUrl);
        }

        /// <summary>
        /// Determines whether [is valid URI] [the specified URI].
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="uriKind">Kind of the URI.</param>
        /// <returns>
        ///   <c>true</c> if [is valid URI] [the specified URI]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidUri(string uri , UriKind uriKind)
        {
            if (!Uri.IsWellFormedUriString(uri, uriKind))
                return false;
            Uri tmp;
            if (!Uri.TryCreate(uri, uriKind, out tmp))
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether [is absolute URL] [the specified URL].
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        ///   <c>true</c> if [is absolute URL] [the specified URL]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        #endregion

    }
}
