//using System.Data;

//using Prezza.Framework.Security;
//using Prezza.Framework.Data.Sprocs;

//namespace Checkbox.Forms
//{
//    /// <summary>
//    /// Lightweight representation of a response template suitable for
//    /// authorization purposes.
//    /// </summary>
//    [FetchProcedure("ckbx_sp_Template_GetAccessControllableResource")]
//    public class LightweightTemplate : LightweightAccessControllable
//    {
//        private int _templateID;

//        /// <summary>
//        /// Internal contructor
//        /// </summary>
//        internal LightweightTemplate(int templateID)
//        {
//            _templateID = templateID;
//        }

//        /// <summary>
//        /// Get/set the template id
//        /// </summary>
//        [FetchParameter(Name="TemplateID", DbType=DbType.Int32, Direction=ParameterDirection.Input)]
//        public int TemplateID
//        {
//            get { return _templateID; }
//            set { _templateID = value; }
//        }
//    }
//}
