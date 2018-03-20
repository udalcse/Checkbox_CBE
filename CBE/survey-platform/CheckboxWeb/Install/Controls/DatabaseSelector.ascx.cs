using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;

namespace CheckboxWeb.Install.Controls
{
    /// <summary>
    /// Database selector allows to create a connection string and edit it
    /// </summary>
    public partial class DatabaseSelector : Checkbox.Web.Common.UserControlBase
    {
        /// <summary>
        /// Connection strin types
        /// </summary>
        public enum ConnectionStringType { SQLAuth, WindowsAuto, FreeForm };

        #region Control Texts
        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string SQLServerAuthenticationCaption
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string WindowsAuthenticationCaption
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string FreeformConnectionStringCaption
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string ServerTxtLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string DbNameTxtLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string UsernameTxtLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string PasswordTxtLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string TrustedServerLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string TrustedDbNameLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string FreeformConnectionStringLabel
        {
            get;
            set;
        }

        /// <summary>
        /// Caption for the web control (label, checkbox or something else)
        /// </summary>
        public string RequiredFieldValidatorMessage
        {
            get;
            set;
        }
        #endregion Control Texts


        /// <summary>
        /// Password
        /// </summary>
        public string Password
        {
            get
            {
                return _passwordTxt.Text.Trim();
            }
        }
        
        /// <summary>
        /// The way how tabs should be situated on the page
        /// </summary>
        public bool HorizontalTabs
        {
            get;
            set;
        }

        /// <summary>
        /// Connection string. Can be readed and written. 
        /// </summary>
        public string ConnectionString
        {
            get
            {
                String connectionString = String.Empty;
                if (_currentTabIndex.Text == "0")
                {
                    connectionString = "Server=" + EscapeValue(_serverTxt.Text) + ";Database=" + EscapeValue(_dbNameTxt.Text) + ";User Id=" + EscapeValue(_usernameTxt.Text) + ";Password=" + EscapeValue(_passwordTxt.Text) + ";";
                }
                else if (_currentTabIndex.Text == "1")
                {
                    connectionString = "Server=" + EscapeValue(_trustedServer.Text) + ";Trusted_Connection=yes;Database=" + EscapeValue(_trustedDbName.Text) + ";";
                }
                else
                {
                    connectionString = _freeformConnectionString.Text;
                }
                return connectionString;
            }
            set
            {
                var builder = new SqlConnectionStringBuilder(value);
                if (builder.IntegratedSecurity)
                {
                    ConnectionType = ConnectionStringType.WindowsAuto;
                    _trustedServer.Text = builder.DataSource;
                    _trustedDbName.Text = builder.InitialCatalog;
                }
                else if (!string.IsNullOrEmpty(builder.UserID) && !string.IsNullOrEmpty(builder.Password) &&
                    !string.IsNullOrEmpty(builder.InitialCatalog) && !string.IsNullOrEmpty(builder.DataSource) &&
                    value.Split(new char[] { ';' }).Length <= 6) //no more other parameters
                {
                    ConnectionType = ConnectionStringType.SQLAuth;
                    _passwordTxt.Text = builder.Password;
                    _passwordTxt.Attributes["value"] = builder.Password;
                    _serverTxt.Text = builder.DataSource;
                    _dbNameTxt.Text = builder.InitialCatalog;
                    _usernameTxt.Text = builder.UserID;
                }
                else
                {
                    ConnectionType = ConnectionStringType.FreeForm;
                    _freeformConnectionString.Text = value;
                }                
            }
        }

        /// <summary>
        /// Connection Type
        /// </summary>
        public ConnectionStringType ConnectionType
        {
            get
            {
                return string.IsNullOrEmpty(_currentTabIndex.Text) ? ConnectionStringType.SQLAuth : (ConnectionStringType)int.Parse(_currentTabIndex.Text);
            }
            set
            {
                _currentTabIndex.Text = ((int)value).ToString();
            }
        }

        /// <summary>
        /// Escapes any reserved characters contained in a specified value.
        /// </summary>
        /// <param name="value">The database configuration value to sanities.</param>
        /// <returns>The escaped value.</returns>
        private static string EscapeValue(string value)
        {
            //see: for more information. http://msdn2.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring(VS.80).aspx
            //The document details all the possible cases where a value needs to be escaped.
            //The worst case scenario is when a value contains semicolons, double quotes and single quotes.
            //In this situation you need to replace " with two "s and surrounded the value with double quotes.
            //Addressing the worst case scenario handles all other cases.

            if (value == null)
            {
                return value;
            }

            if (value.Contains(";") || value.Contains("\"") || value.Contains("'"))
            {
                return string.Format("\"{0}\"", value.Replace("\"", "\"\""));
            }

            return value;
        }

        /// <summary>
        /// OnLoad
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            foreach (Control c in Controls)
            {
                if (c is RequiredFieldValidator)
                {
                    ((RequiredFieldValidator)c).ErrorMessage = RequiredFieldValidatorMessage;
                }
            }
            
        }

    }
}