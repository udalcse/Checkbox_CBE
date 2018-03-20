using System;
using System.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Prezza.Framework.Common;

namespace Checkbox.Globalization.Text
{
    /// <summary>
    /// Abstract base class for decorating an item with localized text.  An implemented
    /// decorator will expose public text properties and internally call the protected get/set
    /// methods when appropriate.
    /// This class is designed to handle localization for items with text ids that depend on the
    /// item id, even though the item may not have been committed to the database.
    /// </summary>
    [Serializable]
    public abstract class TextDecorator : ITransactional
    {
        private string _language;
        private List<string> _alternateLanguages;

        /// <summary>
        /// Constructor.  Initialize with the language for this decorator
        /// </summary>
        /// <param name="language"></param>
        protected TextDecorator(string language)
        {
            _language = string.IsNullOrEmpty(language) ? TextManager.DefaultLanguage : language;

            _alternateLanguages = new List<string>();
        }

        /// <summary>
        /// Add alternate languages to look text up in
        /// </summary>
        /// <param name="alternateLanguages">List of alternate languages to use when Getting text</param>
        public void AddAlternateLanguages(List<string> alternateLanguages)
        {
            if (alternateLanguages == null)
            {
                _alternateLanguages.Clear();
            }
            else
            {
                _alternateLanguages = alternateLanguages;
            }
        }

        /// <summary>
        /// Public accessor to get an array of the alternate languages that have been set.
        /// </summary>
        /// <returns></returns>
        public ReadOnlyCollection<string> GetAlternateLanguages()
        {
            return new ReadOnlyCollection<string>(AlternateLanguages);
        }

        /// <summary>
        /// Get alternate languages
        /// </summary>
        protected List<String> AlternateLanguages
        {
            get
            {
                if (_alternateLanguages == null)
                {
                    _alternateLanguages = new List<string>();
                }

                return _alternateLanguages;
            }
        }

        /// <summary>
        /// Get/set the language for the decorator
        /// </summary>
        public string Language
        {
            get { return _language; }
            set { _language = value; }
        }

        /// <summary>
        /// Get the specified localized text using the configured language for this decorator.  If
        /// the text is not found, will try to get the text using the app default language, then will
        /// search for text using any specified alternate languages.
        /// </summary>
        /// <param name="textID">ID of the text to rectrieve.</param>
        protected virtual string GetText(string textID)
        {
            string text = string.Empty;

            if (!string.IsNullOrEmpty(_language))
            {
                text = TextManager.GetText(textID, _language);
            }

            if (string.IsNullOrEmpty(text))
            {
                text = TextManager.GetText(textID, TextManager.DefaultLanguage);

                if (text != null && text.Trim() != string.Empty)
                {
                    text = "[" + _language + "] " + text;
                    return text;
                }
            }

            if (string.IsNullOrEmpty(text))
            {
                foreach (string altLanguage in AlternateLanguages)
                {
                    text = TextManager.GetText(textID, altLanguage);

                    if (text != null && text.Trim() != string.Empty)
                    {
                        text = "[" + _language + "] " + text;
                        return text;
                    }
                }
            }

            return text;
        }

        /// <summary>
        /// Get text in all languages for the specified text id.
        /// </summary>
        /// <param name="textID">ID of the text to get.</param>
        /// <returns>Dictionary keyed by language code containing texts.</returns>
        public virtual Dictionary<string, string> GetAllTexts(string textID)
        {
            return TextManager.GetAllTexts(textID);
        }

        /// <summary>
        /// Set localized text. If there is no language set, will do nothing.
        /// </summary>
        /// <param name="textID">ID of the text.</param>
        /// <param name="textValue">Text to store.</param>
        protected virtual void SetText(string textID, string textValue)
        {
            if (!string.IsNullOrEmpty(_language))
            {
                TextManager.SetText(textID, _language, textValue);
            }
        }

        /// <summary>
        /// Set localized text.  All arguments are mandatory.
        /// </summary>
        /// <param name="textID"></param>
        /// <param name="textValue"></param>
        /// <param name="language"></param>
        /// <exception cref="NullReferenceException">When textID or language argument is null.</exception>
        /// <exception cref="ArgumentException">When textID or language argument is an empty string.</exception>
        protected virtual void SetText(string textID, string textValue, string language)
        {
            ArgumentValidation.CheckForNullReference(textID, "textID");
            ArgumentValidation.CheckForNullReference(language, "language");
            ArgumentValidation.CheckForEmptyString(textID, "textID");
            ArgumentValidation.CheckForEmptyString(language, "language");

            TextManager.SetText(textID, language, textValue);
        }

        /// <summary>
        /// Protected method where child classes will do the work of
        /// calling the protected setters
        /// </summary>
        protected abstract void SetLocalizedTexts();

        /// <summary>
        /// Save the decorated object and associated localized text
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Save the decorated object and associated text.  The object will be saved
        /// as part of the specified transaction
        /// </summary>
        /// <param name="t"></param>
        public abstract void Save(IDbTransaction t);
        
        #region ITransactional Members

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler TransactionAborted;

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler TransactionCommitted;

        /// <summary>
        /// Handle rollback
        /// </summary>
        public void Rollback()
        {
            OnRollback();
        }

        /// <summary>
        /// Notify decorator transaction has been aborted
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NotifyAbort(object sender, EventArgs e)
        {
            OnAbort(sender, e);
        }

        /// <summary>
        /// Notify decorator transaction has been committed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NotifyCommit(object sender, EventArgs e)
        {
            OnCommit(sender, e);
        }

        #endregion

        /// <summary>
        /// Rollback, does nothing unless overridden
        /// </summary>
        protected virtual void OnRollback()
        {
        }

        /// <summary>
        /// Abort, does nothing unless overridden
        /// </summary>
        /// <param name="sender">Object firing event</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnAbort(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// Commit, does nothing unless overridden
        /// </summary>
        /// <param name="sender">Object firing event.</param>
        /// <param name="e">Event arguments</param>
        protected virtual void OnCommit(object sender, EventArgs e)
        {
        }
    }
}