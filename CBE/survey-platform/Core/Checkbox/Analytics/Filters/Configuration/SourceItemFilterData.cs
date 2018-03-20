using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Checkbox.Common;

namespace Checkbox.Analytics.Filters.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class SourceItemFilterData : FilterData
    {
        private IEnumerable<int> _sourceItemIds;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceItemIds"></param>
        public SourceItemFilterData(IEnumerable<int> sourceItemIds )
        {
            _sourceItemIds = sourceItemIds;
        }

        public override string ObjectTypeName
        {
            get { throw new NotImplementedException(); }
        }

        protected override string LoadSprocName
        {
            get { throw new NotImplementedException(); }
        }

        protected override PersistedDomainObjectDataSet CreateConfigurationDataSet()
        {
            throw new NotImplementedException();
        }

        protected override Filter CreateFilterObject()
        {
            return new SourceItemFilter(_sourceItemIds);
        }

        protected override string GetFilterLeftOperandText(string languageCode)
        {
            throw new NotImplementedException();
        }
    }
}
