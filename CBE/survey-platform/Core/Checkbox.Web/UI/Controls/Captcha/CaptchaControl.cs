using System.IO;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Checkbox.Globalization.Text;
using Checkbox.Common.Captcha.Sound;

namespace Checkbox.Web.UI.Controls.Captcha
{
    /// <summary>
    /// Captcha display control
    /// </summary>
    public class CaptchaControl : CompositeControl
    {
        private ImageButton _playSound;
        private Image _captchaImage;
        private readonly string _languageCode;

        /// <summary>
        /// Create a new captcha control.
        /// </summary>
        /// <param name="languageCode">Language code to use for image tooltips</param>
        public CaptchaControl(string languageCode)
        {
            _languageCode = languageCode;
        }

        /// <summary>
        /// Get/set the code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Get/set image url
        /// </summary>
        public string ImageUrl
        {
            get 
            {
                EnsureChildControls();
                return _captchaImage.ImageUrl;
            }
            set 
            {
                EnsureChildControls();
                _captchaImage.ImageUrl = value;
            }
        }

        /// <summary>
        /// Get/set whether sound is enabled
        /// </summary>
        public bool EnableSound
        {
            get
            {
                EnsureChildControls();
                return _playSound.Visible;
            }
            set
            {
                EnsureChildControls();
                _playSound.Visible = value;
            }
        }

        /// <summary>
        /// Create child controls for the control
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            Table t = new Table {CellSpacing = 2, CellPadding = 2};

            TableRow row = new TableRow {VerticalAlign = VerticalAlign.Top};

            TableCell imageCell = new TableCell();
            TableCell soundCell = new TableCell();

            _playSound = new ImageButton();
            _playSound.Click += _playSound_Click;
            _playSound.ID = "PlaySound";
            _playSound.ImageUrl = Management.ApplicationManager.ApplicationRoot + "/Images/speaker.gif";
            _playSound.ToolTip = TextManager.GetText("/controlText/captchaControl/soundTip", _languageCode);

            _captchaImage = new Image {ID = "CaptchaImage", ToolTip = TextManager.GetText("/controlText/captchaControl/imageTip", _languageCode)};

            imageCell.Controls.Add(_captchaImage);
            soundCell.Controls.Add(_playSound);

            row.Cells.Add(imageCell);
            row.Cells.Add(soundCell);

            t.Rows.Add(row);

            Controls.Add(t);            
        }

        /// <summary>
        /// Play the sound
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _playSound_Click(object sender, ImageClickEventArgs e)
        {
            HttpContext context = HttpContext.Current;

            context.Response.Clear();

            context.Response.AddHeader("content-disposition", "attachment; filename=" + "captcha.wav");
            context.Response.AddHeader("content-transfer-encoding", "binary");

            context.Response.ContentType = "audio/wav";

            SoundGenerator soundGenerator = new SoundGenerator(Code);

            MemoryStream sound = new MemoryStream();

            // Write the sound to the response stream 
            soundGenerator.Sound.Save(sound, SoundFormatEnum.Wav);

            sound.WriteTo(context.Response.OutputStream);
        }
    }
}
