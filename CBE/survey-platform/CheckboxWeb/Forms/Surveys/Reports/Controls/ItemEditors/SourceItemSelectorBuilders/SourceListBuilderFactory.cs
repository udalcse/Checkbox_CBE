using System;

namespace CheckboxWeb.Forms.Surveys.Reports.Controls.ItemEditors.SourceItemSelectorBuilders
{

    public class SourceItemBuilderFactory
    {
        /// <summary>
        /// Creates the builder based on builder type.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">An invalid format: " + builder.ToString()</exception>
        public static ISourceItemBuilder CreateBuilder(SourceListBuilderFormat builder)
        {
            switch (builder)
            {
                case SourceListBuilderFormat.GovernancePriority:
                    return new GovernancePrioritySourceItemBuilder();
                case SourceListBuilderFormat.HeatMap:
                    return new HeatMapSourceItemBuilder();
                case SourceListBuilderFormat.GradientColorDirectorMatrix:
                    return new GradienColorMatrixSourceItemBuilder();
                case SourceListBuilderFormat.Default:
                    return new DefaultSourceItemBuilder();
                default:
                    throw new ArgumentException("An invalid format: " + builder);
            }
        }
    }

    public enum SourceListBuilderFormat { Default, GovernancePriority, HeatMap , GradientColorDirectorMatrix }
}