using System;

namespace Checkbox.PdfExport
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PdfExportSettings
    {
        public string ApplicationPath { set; get; }
        public string ProgressKey { set; get; }
        public PdfExportOrientation Orientation { set; get; }
        public string Language { set; get; }
        public string Name { set; get; }
        public string AuthTicket { set; get; }
        public string SurveyTemplateGUID { set; get; }
        public string Session { get; set; }
        public string AuthenticationCookie { get; set; }
        public bool PrintClientPdf { get; set; }
        public string InvitationId { get; set; }
        public Guid? CustomResponseGUID { set; get; }
        public string ResumeId { set; get; }
    }
}
