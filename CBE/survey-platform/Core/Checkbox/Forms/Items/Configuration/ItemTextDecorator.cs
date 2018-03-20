using System;
using System.Data;
using Checkbox.Globalization.Text;

namespace Checkbox.Forms.Items.Configuration
{
    /// <summary>
    /// Decorator for item data to use to store/set localized data.  Localized fields will
    /// be exposed as public accessors on the classes that extend the item
    /// </summary>
    [Serializable]
    public class ItemTextDecorator : TextDecorator
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="language"></param>
        public ItemTextDecorator(ItemData data, string language) : base(language)
        {
            Data = data;
        }

        /// <summary>
        /// Set localized texts...does nothing for base item editor
        /// </summary>
        protected override void SetLocalizedTexts()
        {
        }

        /// <summary>
        /// Item data getter
        /// </summary>
        public ItemData Data { get; protected set; }

        /// <summary>
        /// Copy the localized text for an item
        /// </summary>
        /// <param name="data"></param>
        protected virtual void CopyLocalizedText(ItemData data)
        {
            
        }

        /// <summary>
        /// Copy the underlying item text into a new copy
        /// </summary>
        /// <param name="data">The ItemData copy</param>
        public void Copy(ItemData data)
        {
            if (Data != null)
            {
                Data.Save();

                if (Data.ID > 0)
                {
                    CopyLocalizedText(data);
                }
            }
        }

        /// <summary>
        /// Save the item
        /// </summary>
        public override void Save()
        {
            if (Data != null)
            {
                Data.Save();

                if (Data.ID > 0)
                {
                    SetLocalizedTexts();
                }
            }
        }

        /// <summary>
        /// Save the item as part of a transaction
        /// </summary>
        /// <param name="t"></param>
        public override void Save(IDbTransaction t)
        {
            if (Data != null)
            {
                Data.Save(t);

                if (Data.ID > 0)
                {
                    SetLocalizedTexts();
                }
            }
        }

        /// <summary>
        /// Call commit action on child 
        /// </summary>
        protected override void OnCommit(object sender, EventArgs e)
        {
            if (Data != null)
            {
                Data.NotifyCommit(sender, e);
            }
        }
        
        /// <summary>
        /// Call abort on children
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void OnAbort(object sender, EventArgs e)
        {
            if(Data != null)
            {
                Data.NotifyAbort(sender, e);
            }
        }

        /// <summary>
        /// Rollback
        /// </summary>
        protected override void OnRollback()
        {
            if(Data != null)
            {
                Data.Rollback();
            }
        }
    }
}
