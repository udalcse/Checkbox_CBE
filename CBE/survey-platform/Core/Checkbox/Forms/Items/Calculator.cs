using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using Checkbox.Forms.Items.Configuration;
using Checkbox.Management;
using Checkbox.Wcf.Services.Proxies;
using Prezza.Framework.Data;
using Prezza.Framework.ExceptionHandling;

namespace Checkbox.Forms.Items
{
    /// <summary>
    /// Input item that allows selection of one of a number of options from a range or a list.
    /// </summary>
    [Serializable]
    public class Calculator : TextItem
    {
        /// <summary>
        /// Get/Set Round Mode
        /// </summary>
        public int RoundToPlaces { get; set; }

        /// <summary>
        /// Get/Set Formula
        /// </summary>
        public string Formula { get; set; }

        /// <summary>
        /// Configure the item
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="languageCode"></param>
        /// <param name="templateId"></param>
        public override void Configure(ItemData configuration, string languageCode, int? templateId)
        {
            base.Configure(configuration, languageCode, templateId);

            var data = (CalculatorItemData)configuration;
            RoundToPlaces = data.RoundToPlaces;
            Formula = data.Formula;
        }
        
        /// <summary>
        /// 
        /// </summary>
        protected override void OnPageLoad()
        {
            base.OnPageLoad();
            string formulaWithResolvedContants = GetPipedText("Formula", Formula);
            double res = double.NaN;
            try
            {
                res = Logic.FormulaCalculator.Calculate(formulaWithResolvedContants);

                if (RoundToPlaces > 0)
                    res = Math.Round(res, RoundToPlaces);
                else
                    res = Math.Round(res);
            }
            catch (Exception ex)
            {
                ExceptionPolicy.HandleException(ex, "BusinessPublic");
            }

            SetAnswer(res.ToString(Logic.FormulaCalculator.ConversionCI));
        }

        /// <summary>
        /// Get instance collection
        /// </summary>
        /// <returns></returns>
        protected override NameValueCollection GetInstanceDataValuesForSerialization()
        {
            NameValueCollection collection = base.GetInstanceDataValuesForSerialization();

            collection["RoundToPlaces"] = RoundToPlaces.ToString();
            collection["Formula"] = Formula;

            return collection;
        }

        /// <summary>
        /// Calculator doesn't support posting answers from client, so do nothing here
        /// </summary>
        /// <param name="dto"></param>
        public override void UpdateFromDataTransferObject(IItemProxyObject dto)
        {
            return;
        }
    }
}
