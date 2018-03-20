using System;
using System.Collections.Generic;
using System.Web.Security;
using Checkbox.Configuration;
using Checkbox.Globalization.Text;
using Checkbox.Security.Providers;

namespace Checkbox.Web.Providers
{
    ///<summary>
    ///</summary>
    public static class MembershipProviderManager
    {
        public const string CHAINING_MEMBERSHIP_PROVIDER_NAME = "ChainingMembershipProvider";
        public const string CHECKBOX_MEMBERSHIP_PROVIDER_NAME = "CheckboxMembershipProvider";

        private static IList<MembershipProviderInfo> _providersInfo;

        /// <summary>
        /// 
        /// </summary>
        public static bool DisableForeignProviders
        {
            get { return StaticConfiguration.DisableForeighMembershipProviders; }
        }

        ///<summary>
        ///</summary>
        public static MembershipProviderCollection Providers { set; get; }

        ///<summary>
        ///</summary>
        public static ICheckboxMembershipProvider DefaultProvider { set; get; }

        ///<summary>
        ///</summary>
        public static MembershipProvider FirstAvailableProvider
        { 
            get
            {
                if (DisableForeignProviders || DefaultProvider.ProviderName == CHECKBOX_MEMBERSHIP_PROVIDER_NAME ||
                    (DefaultProvider.ProviderName == CHAINING_MEMBERSHIP_PROVIDER_NAME &&
                    (Providers[CHAINING_MEMBERSHIP_PROVIDER_NAME] as ChainingMembershipProvider)
                        .ProviderList.Contains(CHECKBOX_MEMBERSHIP_PROVIDER_NAME)))
                {
                    return Providers[CHECKBOX_MEMBERSHIP_PROVIDER_NAME];
                }

                foreach (var provider in Providers)
                {
                    if (!(provider is ICheckboxMembershipProvider))
                        return (MembershipProvider)provider;
                }

                return null;
            }
        }

        ///<summary>
        ///</summary>
        public static bool IsCheckboxProviderInChain { set; get; }

        ///<summary>
        ///</summary>
        public static void Initialize(MembershipProviderCollection providers)
        {
            lock (typeof(MembershipProviderManager))
            {               
                if (DisableForeignProviders)
                {
                    Providers = new MembershipProviderCollection();
                    if (providers[CHECKBOX_MEMBERSHIP_PROVIDER_NAME] != null)
                        Providers.Add(providers[CHECKBOX_MEMBERSHIP_PROVIDER_NAME]);
                }
                else
                    Providers = providers;

                if (Providers[CHAINING_MEMBERSHIP_PROVIDER_NAME] != null)
                    DefaultProvider = (ICheckboxMembershipProvider)Providers[CHAINING_MEMBERSHIP_PROVIDER_NAME];
                else if (Membership.Providers[CHECKBOX_MEMBERSHIP_PROVIDER_NAME] != null)
                    DefaultProvider = (ICheckboxMembershipProvider)Providers[CHECKBOX_MEMBERSHIP_PROVIDER_NAME];
                else
                    throw new Exception(string.Format("At least one of {0} or {1} must be defined in membership section of web.config", CHECKBOX_MEMBERSHIP_PROVIDER_NAME, CHAINING_MEMBERSHIP_PROVIDER_NAME));
            }
        }

        ///<summary>
        ///</summary>
        ///<returns></returns>
        public static IList<MembershipProviderInfo> ProvidersInfo
        {
            get
            {
                if (_providersInfo == null)
                {
                    _providersInfo = new List<MembershipProviderInfo>();
                    
                    //add checkbox membership provider at first
                    if(Providers[CHECKBOX_MEMBERSHIP_PROVIDER_NAME] != null)
                        _providersInfo.Add(new MembershipProviderInfo(CHECKBOX_MEMBERSHIP_PROVIDER_NAME, Providers[CHECKBOX_MEMBERSHIP_PROVIDER_NAME].GetType()));
                    
                    //add all others
                    foreach (var providerObj in Providers)
                    {
                        var provider = providerObj as MembershipProvider;
                        if (provider != null && !(provider is ICheckboxMembershipProvider))
                            _providersInfo.Add(new MembershipProviderInfo(provider.Name, provider.GetType()));
                    }
                }
                return _providersInfo;
            }
        }

        ///<summary>
        ///</summary>
        public static bool IsChainingProviderDefault
        {
            get { return DefaultProvider.ProviderName == CHAINING_MEMBERSHIP_PROVIDER_NAME; }
        }

    }

    public class MembershipProviderInfo
    {
        public string Name { set; get; }
        public string Title { set; get; }
        public Type Type { set; get; }

        public MembershipProviderInfo(string name, Type type)
        {
            Name = name;
            Title = GetProviderCustomName(name);
            Type = type;
        }

        private string GetProviderCustomName(string providerName)
        {
            var name = TextManager.GetText("/membershipProvider/" + providerName);
            if (string.IsNullOrEmpty(name))
                name = providerName;

            return name;
        }
    }
}
