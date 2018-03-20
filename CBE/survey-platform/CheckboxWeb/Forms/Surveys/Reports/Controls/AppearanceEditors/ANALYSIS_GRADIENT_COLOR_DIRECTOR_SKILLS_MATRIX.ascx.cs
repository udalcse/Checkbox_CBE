using System.Drawing;
using Checkbox.Analytics.Items.UI;
using Checkbox.Common;
using Checkbox.Forms.Items.UI;
using Checkbox.Web.Forms.UI.Editing;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.AppearanceEditors
{
    public partial class ANALYSIS_GRADIENT_COLOR_DIRECTOR_SKILLS_MATRIX : UserControlAppearanceEditorBase
    {
        /// <summary>
        /// Gets the appearance data.
        /// </summary>
        /// <value>
        /// The appearance data.
        /// </value>
        private GradientColorDirectorSkillsMatrixAppearanceData GradientColorMatrixAppearanceData
            => this.AppearanceData as GradientColorDirectorSkillsMatrixAppearanceData;

        /// <summary>
        /// Initialize the control
        /// </summary>
        /// <param name="data"></param>
        public override void Initialize(AppearanceData data)
        {
            base.Initialize(data);

            _directorAverages.Checked = GradientColorMatrixAppearanceData.DirectorAverages;
            _titleSize.Text = GradientColorMatrixAppearanceData.TitleFontSize.ToString();
            _font.Text = GradientColorMatrixAppearanceData.FontFamily;
            _gridLine.Checked = GradientColorMatrixAppearanceData.GridLine;
            _itemColumnHeader.Text = GradientColorMatrixAppearanceData.ItemColumnHeader;
            _averagesColumnHeader.Text = GradientColorMatrixAppearanceData.AveragesColumnHeader;
            _ratingDetailsHeader.Text = GradientColorMatrixAppearanceData.RatingDetailsHeader;
            _summaryHeader.Text = GradientColorMatrixAppearanceData.SummaryHeader;
            Utilities.BindList(_titleSize, Utilities.GetFontSizeListItems(8, 36, 1, " pt"), GradientColorMatrixAppearanceData.TitleFontSize.ToString());
            Utilities.BindList(_font, Utilities.GetFontFamilyNameListItems(FontStyle.Bold, FontStyle.Regular), GradientColorMatrixAppearanceData.FontFamily);
        }


        /// <summary>
        /// Update data
        /// </summary>
        public override void UpdateData()
        {
            base.UpdateData();

            GradientColorMatrixAppearanceData.DirectorAverages = _directorAverages.Checked ;
            GradientColorMatrixAppearanceData.TitleFontSize = int.Parse(_titleSize.Text);
            GradientColorMatrixAppearanceData.FontFamily = _font.Text;
            GradientColorMatrixAppearanceData.GridLine = _gridLine.Checked;
            GradientColorMatrixAppearanceData.ItemColumnHeader = _itemColumnHeader.Text;
            GradientColorMatrixAppearanceData.AveragesColumnHeader = _averagesColumnHeader.Text;
            GradientColorMatrixAppearanceData.RatingDetailsHeader = _ratingDetailsHeader.Text;
            GradientColorMatrixAppearanceData.SummaryHeader = _summaryHeader.Text;
        }
    }
}