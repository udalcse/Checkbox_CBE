using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Prezza.Framework.Security.Principal;
using Checkbox.Common;
using Checkbox.Management;
using Checkbox.Forms.Items;
using Checkbox.Forms.Piping.Tokens;

namespace Checkbox.Forms.Piping
{
    /// <summary>
    /// Delegate for handling changes to token values.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TokenValueUpdated(object sender, TokenValueUpdatedEventArgs e);

    /// <summary>
    /// Mediator for handling item piping
    /// </summary> 
    [Serializable]
    public class PipeMediator : IDisposable
    {
        //Response Properties
        private readonly ResponseProperties _responseProperties;
        private ExtendedPrincipal _currentPrincipal;

        private Regex _regExp;

        //Internal cache of item texts

        private bool _initialized;

        /// <summary>
        /// Default constructor
        /// </summary>
        internal PipeMediator()
        {
            ProcessedTexts = new Dictionary<string, ProcessedText>();
            TokenValues = new Dictionary<string, TokenValue>();
            StaticTexts = new Dictionary<string, string>();
            ResponsePipes = new List<ItemToken>();
            _responseProperties = new ResponseProperties();

            _regExp = CreateRegEx();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsInitialized
        {
            get { return _initialized; }
        }

        /// <summary>
        /// Get/set processed texts
        /// </summary>
        protected Dictionary<string, ProcessedText> ProcessedTexts { get; set; }

        /// <summary>
        /// Get/set token values
        /// </summary>
        protected Dictionary<string, TokenValue> TokenValues { get; set; }

        /// <summary>
        /// Get/set static texts
        /// </summary>
        protected Dictionary<string, string> StaticTexts { get; set; }

        /// <summary>
        /// Get the regular expression for parsing pipe texts
        /// </summary>
        protected Regex RegExp
        {
            get { return _regExp; }
            set { _regExp = value; }
        }

        /// <summary>
        /// Get the list of response pipes for the survey response.
        /// </summary>
        protected List<ItemToken> ResponsePipes { get; private set; }

        /// <summary>
        /// Get reg ex
        /// </summary>
        /// <returns></returns>
        protected virtual Regex CreateRegEx()
        {
            return new Regex(ApplicationManager.AppSettings.PipePrefix + @"\w*\b", RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Initialize the pipe mediater with a response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="principal"></param>
        internal virtual void Initialize(Response response, ExtendedPrincipal principal)
        {
            //Always initialize response properties so the latest values can be retrieved
            _responseProperties.Initialize(response);
            _currentPrincipal = principal;

            var composites = response.Items.OfType<ICompositeItem>();
            var items = response.Items.Select(i => i.Value).Where(i => !(i is ICompositeItem)).ToList();
            items.AddRange(composites.SelectMany(c => c.Items));

            //Hook item changed events & create value tokens for current page items
            foreach (var item in items.OfType<IAnswerable>())
            {
                item.AnswerChanged += item_AnswerChanged;

                var tokens = response.ResponsePipes.Where(t => t.ItemID == (item as Item).ID).ToList();
                foreach (var token in tokens)
                {
                    if (!TokenValues.ContainsKey(token.TokenName))
                    {
                        var tv = new TokenValue(token) { Value = null, IsDirty = true };
                        CacheValue(token.TokenName, tv);
                    }
                }
            }

            _initialized = true;

            foreach (var item in items.OfType<IAnswerable>())
            {
                item.UpdateMergedText();
            }
        }

        /// <summary>
        /// Handle and item's answer changing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void item_AnswerChanged(object sender, EventArgs e)
        {
            if (sender is ResponseItem && sender is IAnswerable)
            {
                //In some cases, getting a response token's value could cause
                // the TokenValues dictionary to be modified.  So, first build
                // a list of tokens to get the value of, then assign values.
                var tokensToGet = (from tokenValue in TokenValues.Values
                                   where tokenValue.Token.Type == TokenType.Answer && ((ItemToken) tokenValue.Token).ItemID == ((Item) sender).ID
                                   select tokenValue.Token).ToList();

                foreach (Token token in tokensToGet.Where(token => TokenValues.ContainsKey(token.TokenName)))
                {
                    TokenValues[token.TokenName].Value = PipeManager.GetTokenValue(token, ((ResponseItem)sender).Response);
                }
            }
        }

        #region Consumer Methods

        /// <summary>
        /// Register a pipeable text
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="key"></param>
        /// <param name="text"></param>
        public virtual void RegisterText(int itemID, string key, string text)
        {
            if (Utilities.IsNotNullOrEmpty(key) && Utilities.IsNotNullOrEmpty(text))
            {
                string cacheKey = itemID + "_" + key;

                if (TextCacheContainsKey(cacheKey))
                {
                    ProcessedText processedText = GetTextFromCache(cacheKey);
                    processedText.OriginalText = text;
                }
                else
                {
                    bool tokensFound = false;

                    //Create a processed text to hold items
                    var processedText = new ProcessedText();

                    //Parse the text for tokens
                    Match m = _regExp.Match(text);

                    while (m.Success)
                    {
                        tokensFound = true;

                        if (ValueCacheContainsKey(m.Value))
                        {
                            processedText.AddTokenValue(GetValueFromCache(m.Value));
                        }
                        else
                        {
                            Token token = PipeManager.GetToken(m.Value, null);

                            if (token != null && token.Type != TokenType.Other)
                            {
                                TokenValues[m.Value] = new TokenValue(token);
                                processedText.AddTokenValue(TokenValues[m.Value]);
                            }
                            else
                            {
                                foreach (ItemToken itemToken in
                                    ResponsePipes.Where(itemToken => string.Compare(itemToken.TokenName, m.Value, true) == 0))
                                {
                                    TokenValues[m.Value] = new TokenValue(itemToken);
                                    processedText.AddTokenValue(TokenValues[m.Value]);
                                    break;
                                }
                            }
                        }

                        m = m.NextMatch();
                    }

                    //If this item has tokens, cache the processed text, otherwise put it in the static store
                    if (tokensFound && _initialized)
                    {
                        processedText.OriginalText = text;
                        CacheText(cacheKey, processedText);
                    }
                    else
                    {
                        StaticTexts[cacheKey] = text;
                    }
                }
            }
        }

        /// <summary>
        /// Get the specified text, registering it with the initial value
        /// if it has not been registered.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="key"></param>
        /// <param name="initialValue"></param>
        /// <returns></returns>
        public virtual string GetText(int itemId, string key, string initialValue)
        {
            string cacheKey = string.Format("{0}_{1}", itemId, key);

            if (!StaticTexts.ContainsKey(cacheKey) && !ProcessedTexts.ContainsKey(key))
            {
                RegisterText(itemId, key, initialValue);
            }

            return Utilities.RemoveScript(GetText(itemId, key, true));
        }

        /// <summary>
        /// Get the text with optional parameter to prevent removal of empty tokens
        /// </summary>
        /// <param name="itemID"></param>
        /// <param name="key"></param>
        /// <param name="removeEmpty"></param>
        /// <returns></returns>
        protected virtual string GetText(int itemID, string key, bool removeEmpty)
        {
            if (Utilities.IsNullOrEmpty(key))
            {
                return string.Empty;
            }

            string cacheKey = string.Format("{0}_{1}", itemID, key);

            //Check static cache first
            if (StaticTexts.ContainsKey(cacheKey))
            {
                return StaticTexts[cacheKey];
            }

            ProcessedText text = GetTextFromCache(cacheKey);

            if (text != null)
            {
                ProcessTokens(text.TokenValues, removeEmpty);
                return text.Text;
            }

            return string.Empty;
        }

        /// <summary>
        /// Process token values
        /// </summary>
        protected virtual void ProcessTokens(ReadOnlyCollection<TokenValue> tokenValues, bool removeEmpty)
        {
            foreach (TokenValue tokenValue in tokenValues)
            {
                //Item tokens will not be marked dirty, so they shouldn't be processed here.
                //Instead, they'll be processed in the item answer changed event handler
                if (tokenValue.IsDirty)
                {
                    if (tokenValue.Token.Type == TokenType.Response || tokenValue.Token.Type == TokenType.ResponseTemplate)
                    {
                        tokenValue.Value = PipeManager.GetTokenValue(tokenValue.Token, _responseProperties);
                    }
                    else if (tokenValue.Token.Type == TokenType.Profile)
                    {
                        tokenValue.Value = PipeManager.GetTokenValue(tokenValue.Token, _currentPrincipal);
                    }
                    else
                    {
                        tokenValue.Value = string.Empty;
                    }
                }

                if (!removeEmpty)
                {
                    if (Utilities.IsNullOrEmpty(tokenValue.Value))
                    {
                        tokenValue.Value = tokenValue.Token.TokenName;
                    }
                }
            }
        }

        #endregion

        #region Cache Utility


        /// <summary>
        /// Cache the processed text
        /// </summary>
        /// <param name="key"></param>
        /// <param name="text"></param>
        protected void CacheText(string key, ProcessedText text)
        {
            ProcessedTexts[key] = text;
        }

        /// <summary>
        /// Cache the token value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        protected void CacheValue(string key, TokenValue value)
        {
            TokenValues[key] = value;
        }

        /// <summary>
        /// Get a cached value
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected TokenValue GetValueFromCache(string key)
        {
            if (TokenValues.ContainsKey(key))
            {
                return TokenValues[key];
            }

            return null;
        }

        /// <summary>
        /// Get the processed text from the cache
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected ProcessedText GetTextFromCache(string key)
        {
            if (ProcessedTexts.ContainsKey(key))
            {
                return ProcessedTexts[key];
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool TextCacheContainsKey(string key)
        {
            return ProcessedTexts.ContainsKey(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected bool ValueCacheContainsKey(string key)
        {
            return TokenValues.ContainsKey(key);
        }


        #endregion

        #region Processed Text

        /// <summary>
        /// Processed Text
        /// </summary>
        [Serializable]
        protected class ProcessedText
        {
            private string _text;
            private readonly List<TokenValue> _tokenValues;

            /// <summary>
            /// Constructor
            /// </summary>
            public ProcessedText()
            {
                OriginalText = string.Empty;
                _text = string.Empty;
                IsDirty = true;
                _tokenValues = new List<TokenValue>();
            }

            /// <summary>
            /// Get/set the original text
            /// </summary>
            public string OriginalText { get; set; }

            /// <summary>
            /// Get/set the text.  Setting the text clears the the dirty text
            /// </summary>
            public string Text
            {
                get
                {
                    if (IsDirty)
                    {
                        ProcessText();
                    }

                    return _text;
                }
                set
                {
                    _text = value;
                    IsDirty = false;
                }
            }

            /// <summary>
            /// Get/set whether the text is dirty
            /// </summary>
            public bool IsDirty { get; set; }

            /// <summary>
            /// Add a token value
            /// </summary>
            /// <param name="tokenValue"></param>
            public void AddTokenValue(TokenValue tokenValue)
            {
                tokenValue.ValueUpdated += tokenValue_ValueUpdated;
                _tokenValues.Add(tokenValue);
            }

            /// <summary>
            /// Process text
            /// </summary>
            private void ProcessText()
            {
                _text = OriginalText;

                foreach (TokenValue tokenValue in _tokenValues)
                {
                    _text = _text.Replace(tokenValue.Token.TokenName, tokenValue.Value);
                }

                IsDirty = false;
            }

            /// <summary>
            /// Handle token value updated
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void tokenValue_ValueUpdated(object sender, TokenValueUpdatedEventArgs e)
            {
                IsDirty = true;
            }

            /// <summary>
            /// Get the token values for this text
            /// </summary>
            public ReadOnlyCollection<TokenValue> TokenValues
            {
                get { return new ReadOnlyCollection<TokenValue>(_tokenValues); }
            }
        }

        #endregion

        #region TokenValue class

        /// <summary>
        /// Simple container for token values that supports a dirty flag
        /// </summary>
        [Serializable]
        protected class TokenValue
        {
            private string _value;

            /// <summary>
            /// Event fired when token value is updated.
            /// </summary>
            public event TokenValueUpdated ValueUpdated;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="token"></param>
            public TokenValue(Token token)
            {
                Token = token;
                IsDirty = true;
            }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="token"></param>
            /// <param name="value"></param>
            public TokenValue(Token token, string value)
            {
                Token = token;
                _value = value;
                IsDirty = false;
            }

            /// <summary>
            /// Get the token 
            /// </summary>
            public Token Token { get; private set; }

            /// <summary>
            /// Get/set whether the value is dirty and needs to be re-evaluated
            /// </summary>
            public bool IsDirty { get; set; }

            /// <summary>
            /// Get/set the value of the token.  When the value is set, the dirty flag is cleared
            /// </summary>
            public string Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;

                    //Response tokens are always dirty, since we always want to get 
                    // latest values.
                    if (Token.Type != TokenType.Response)
                    {
                        IsDirty = false;
                    }

                    if (ValueUpdated != null)
                    {
                        ValueUpdated(this, new TokenValueUpdatedEventArgs(Token.TokenName, value));
                    }
                }
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of the response and any child items.  
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            //Instruct GC not to finialize this object
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Overridable dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }

        #endregion
    }

    #region TokenValueChangedEventArgs

    /// <summary>
    /// Event args for token updated event
    /// </summary>
    public class TokenValueUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="tokenName"></param>
        /// <param name="newValue"></param>
        internal TokenValueUpdatedEventArgs(string tokenName, string newValue)
        {
            TokenName = tokenName;
            NewValue = newValue;
        }

        /// <summary>
        /// Get the token name
        /// </summary>
        internal string TokenName { get; private set; }

        /// <summary>
        /// Get the token's new value
        /// </summary>
        internal string NewValue { get; private set; }
    }

    #endregion
}
