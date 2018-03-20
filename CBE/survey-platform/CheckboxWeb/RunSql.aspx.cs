using System;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Checkbox.Web.Page;
using Prezza.Framework.Data;

namespace CheckboxWeb
{
    public partial class RunSql : SecuredPage
    {
        protected override string PageRequiredRole => "System Administrator";

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void RunSqlEventHandler(object sender, EventArgs e)
        {
            var sql = _sqlScript.Text;

          if (string.IsNullOrWhiteSpace(sql))// || !ValidateQuery(sql))
            {
                string script = "<script>alert('Sql script is not valid. It must contain not blank SELECT statement');</script>";

                if (!ClientScript.IsStartupScriptRegistered("errorScript"))
                   ClientScript.RegisterStartupScript(GetType(), "errorScript", script);

                return;
            }

            Database db = DatabaseFactory.CreateDatabase();

            
            using (var dataSet = db.ExecuteDataSet(CommandType.Text, sql))
            {
                _sqlResult.DataSource = dataSet;
                _sqlResult.DataBind();

            }

            if (db.GetConnection() != null)
                db.GetConnection().Close();
        }


        private static bool ValidateQuery(string query)
        {
            return !ValidateRegex("exec", query) && !ValidateRegex("insert", query) && !ValidateRegex("alter", query) &&
                   !ValidateRegex("create", query) && !ValidateRegex("drop", query) && !ValidateRegex("truncate", query);
        }
        private static bool ValidateRegex(string term, string query)
        {
            // this regex finds all keywords {0} that are not leading or trailing by alphanumeric 
            return new Regex(string.Format(term, RegexOptions.IgnoreCase)).IsMatch(query);
        }


        protected void gvEmployee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            _sqlResult.PageIndex = e.NewPageIndex;
            RunSqlEventHandler(sender,e);
        }
    }
}