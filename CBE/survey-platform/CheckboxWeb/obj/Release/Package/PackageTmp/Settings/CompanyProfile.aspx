<%@ Page Title="" Language="C#" MasterPageFile="~/Dialog.Master" AutoEventWireup="false" CodeBehind="CompanyProfile.aspx.cs" Inherits="CheckboxWeb.Settings.CompanyProfile" %>
<%@ Import Namespace="Checkbox.Web"%>
<%@ MasterType VirtualPath="~/Dialog.Master" %>

<asp:Content ID="head" ContentPlaceHolderID="_headContent" runat="server">
    <ckbx:ResolvingCssElement ID="CheckboxCss" runat="server" Source="~/App_Themes/CheckboxTheme/Checkbox.css" />
</asp:Content>

<asp:Content ID="settingContent" ContentPlaceHolderID="_pageContent" runat="server">
<div class="padding10">
    <h3 runat="server" id="_subtitle"><%= WebTextManager.GetText("/pageText/settings/navigation.ascx/companyProfile")%></h3>
        
    <asp:Panel runat="server" class="dashStatsWrapper border999 shadow999" ID="_selectCompanyPanel">
        <div class="dashStatsHeader">
            <span class="mainStats left"><%= WebTextManager.GetText("/pageText/settings/companyProfile.aspx/selectCompany")%></span>
        </div>
        <div class="dashStatsContent">
            <div class="fixed_125 left"><ckbx:MultiLanguageLabel ID="_validationPageLanguageLbl" runat="server" TextId="/pageText/settings/companyProfile.aspx/selectCompany" /></div>
            <div class="left input">
                <asp:DropDownList ID="_profileList" runat="server" AutoPostBack="true" />
            </div>
            <br class="clear" />
        </div>             
    </asp:Panel>
    
    <div class="dashStatsWrapper border999 shadow999">
        <div class="dashStatsHeader">
            <span class="mainStats fixed_150 left"><%= WebTextManager.GetText("/pageText/settings/companyProfile.aspx/profileProperties")%></span>                    
            <br class="clear" />
        </div>

        <div class="dashStatsContent zebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/profileName' /></div>
            <div class="left input">
                <asp:TextBox ID="_profileName" runat="server" Width="250px"/>
            </div>
            <div class="left profile-properties-validation" >
                <asp:CustomValidator ValidateEmptyText="True" OnServerValidate="Validate" Display="Dynamic" ID="_profileNameValidator" runat="server" ControlToValidate="_profileName" />
            </div>
            <br class="clear" />
        </div>
        
        <div class="dashStatsContent detailZebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/isDefault' /></div>
            <div class="left input">
                <asp:CheckBox ID="_isDefaultCheckbox" runat="server" />
            </div>
            <br class="clear" />
        </div>

        <div class="dashStatsContent detailZebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/companyName' /></div>
            <div class="left input">
                <asp:TextBox ID="_companyName" runat="server" Width="250px"/>
            </div>
            <div class="left profile-properties-validation">
                <asp:CustomValidator ValidateEmptyText="True" OnServerValidate="Validate" Display="Dynamic" ID="_companyNameValidator" runat="server" ControlToValidate="_companyName" />
            </div>
            <br class="clear" />
        </div>

        <div class="dashStatsContent zebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/address1' /></div>
            <div class="left input">
                <asp:TextBox ID="_address1" runat="server" Width="250px"/>
            </div>
            <div class="left profile-properties-validation">
                <asp:CustomValidator ValidateEmptyText="True" OnServerValidate="Validate" Display="Dynamic" ID="_address1Validator" runat="server" ControlToValidate="_address1" />
            </div>
            <br class="clear" />
        </div>

        <div class="dashStatsContent detailZebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/address2' /></div>
            <div class="left input">
                <asp:TextBox ID="_address2" runat="server" Width="250px"/>
            </div>
            <br class="clear" />
        </div>

        <div class="dashStatsContent zebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/city' /></div>
            <div class="left input">
                <asp:TextBox ID="_city" runat="server" Width="250px"/>
            </div>
            <div class="left profile-properties-validation">
                <asp:CustomValidator ValidateEmptyText="True" OnServerValidate="Validate" Display="Dynamic" ID="_cityValidator" runat="server" ControlToValidate="_city" />
            </div>
            <br class="clear" />
        </div>

        <div class="dashStatsContent detailZebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/state' /></div>
            <div class="left input">
                <asp:TextBox ID="_state" runat="server" Width="250px"/>
            </div>
            <br class="clear" />
        </div>
            
        <div class="dashStatsContent zebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/postcode' /></div>
            <div class="left input">
                <asp:TextBox ID="_postcode" runat="server" Width="250px"/>
            </div>
            <div class="left profile-properties-validation">
                <asp:CustomValidator ValidateEmptyText="True" OnServerValidate="Validate" Display="Dynamic" ID="_postcodeValidator" runat="server" ControlToValidate="_postcode" />
            </div>
            <br class="clear" />
        </div>
    
        <div class="dashStatsContent detailZebra">
            <div class="fixed_150 left input"><ckbx:MultiLanguageLabel runat="server" TextId='/pageText/settings/companyProfile.aspx/country' /></div>
            <div class="left input">
                <asp:DropDownList ID="_country" runat="server" Width="250px">
                    <asp:ListItem Text="United States" Value="United States"></asp:ListItem>
                    <asp:ListItem Text="United Kingdom" Value="United Kingdom"></asp:ListItem>
                    <asp:ListItem Text="Afghanistan" Value="Afghanistan"></asp:ListItem>
                    <asp:ListItem Text="Albania" Value="Albania"></asp:ListItem>
                    <asp:ListItem Text="Algeria" Value="Algeria"></asp:ListItem>
                    <asp:ListItem Text="American Samoa" Value="American Samoa"></asp:ListItem>
                    <asp:ListItem Text="Andorra" Value="Andorra"></asp:ListItem>
                    <asp:ListItem Text="Angola" Value="Angola"></asp:ListItem>
                    <asp:ListItem Text="Anguilla" Value="Anguilla"></asp:ListItem>
                    <asp:ListItem Text="Antigua and Barbuda" Value="Antigua and Barbuda"></asp:ListItem>
                    <asp:ListItem Text="Argentina" Value="Argentina"></asp:ListItem>
                    <asp:ListItem Text="Armenia" Value="Armenia"></asp:ListItem>
                    <asp:ListItem Text="Aruba" Value="Aruba"></asp:ListItem>
                    <asp:ListItem Text="Australia" Value="Australia"></asp:ListItem>
                    <asp:ListItem Text="Austria" Value="Austria"></asp:ListItem>
                    <asp:ListItem Text="Azerbaijan" Value="Azerbaijan"></asp:ListItem>
                    <asp:ListItem Text="Bahamas" Value="Bahamas"></asp:ListItem>
                    <asp:ListItem Text="Bahrain" Value="Bahrain"></asp:ListItem>
                    <asp:ListItem Text="Bangladesh" Value="Bangladesh"></asp:ListItem>
                    <asp:ListItem Text="Barbados" Value="Barbados"></asp:ListItem>
                    <asp:ListItem Text="Belarus" Value="Belarus"></asp:ListItem>
                    <asp:ListItem Text="Belgium" Value="Belgium"></asp:ListItem>
                    <asp:ListItem Text="Belize" Value="Belize"></asp:ListItem>
                    <asp:ListItem Text="Benin" Value="Benin"></asp:ListItem>
                    <asp:ListItem Text="Bermuda" Value="Bermuda"></asp:ListItem>
                    <asp:ListItem Text="Bhutan" Value="Bhutan"></asp:ListItem>
                    <asp:ListItem Text="Bolivia" Value="Bolivia"></asp:ListItem>
                    <asp:ListItem Text="Bosnia-Herzegovina" Value="Bosnia-Herzegovina"></asp:ListItem>
                    <asp:ListItem Text="Botswana" Value="Botswana"></asp:ListItem>
                    <asp:ListItem Text="Bouvet Island" Value="Bouvet Island"></asp:ListItem>
                    <asp:ListItem Text="Brazil" Value="Brazil"></asp:ListItem>
                    <asp:ListItem Text="Brunei" Value="Brunei"></asp:ListItem>
                    <asp:ListItem Text="Bulgaria" Value="Bulgaria"></asp:ListItem>
                    <asp:ListItem Text="Burkina Faso" Value="Burkina Faso"></asp:ListItem>
                    <asp:ListItem Text="Burundi" Value="Burundi"></asp:ListItem>
                    <asp:ListItem Text="Cambodia" Value="Cambodia"></asp:ListItem>
                    <asp:ListItem Text="Cameroon" Value="Cameroon"></asp:ListItem>
                    <asp:ListItem Text="Canada" Value="Canada"></asp:ListItem>
                    <asp:ListItem Text="Cape Verde" Value="Cape Verde"></asp:ListItem>
                    <asp:ListItem Text="Cayman Islands" Value="Cayman Islands"></asp:ListItem>
                    <asp:ListItem Text="Central African Republic" Value="Central African Republic"></asp:ListItem>
                    <asp:ListItem Text="Chad" Value="Chad"></asp:ListItem>
                    <asp:ListItem Text="Chile" Value="Chile"></asp:ListItem>
                    <asp:ListItem Text="China" Value="China"></asp:ListItem>
                    <asp:ListItem Text="Christmas Island" Value="Christmas Island"></asp:ListItem>
                    <asp:ListItem Text="Cocos (Keeling) Islands" Value="Cocos (Keeling) Islands"></asp:ListItem>
                    <asp:ListItem Text="Colombia" Value="Colombia"></asp:ListItem>
                    <asp:ListItem Text="Comoros" Value="Comoros"></asp:ListItem>
                    <asp:ListItem Text="Congo, Democratic Republic of the (Zaire)" Value="Congo, Democratic Republic of the (Zaire)"></asp:ListItem>
                    <asp:ListItem Text="Congo, Republic of" Value="Congo, Republic of"></asp:ListItem>
                    <asp:ListItem Text="Cook Islands" Value="Cook Islands"></asp:ListItem>
                    <asp:ListItem Text="Costa Rica" Value="Costa Rica"></asp:ListItem>
                    <asp:ListItem Text="Croatia" Value="Croatia"></asp:ListItem>
                    <asp:ListItem Text="Cuba" Value="Cuba"></asp:ListItem>
                    <asp:ListItem Text="Cyprus" Value="Cyprus"></asp:ListItem>
                    <asp:ListItem Text="Czech Republic" Value="Czech Republic"></asp:ListItem>
                    <asp:ListItem Text="Denmark" Value="Denmark"></asp:ListItem>
                    <asp:ListItem Text="Djibouti" Value="Djibouti"></asp:ListItem>
                    <asp:ListItem Text="Dominica" Value="Dominica"></asp:ListItem>
                    <asp:ListItem Text="Dominican Republic" Value="Dominican Republic"></asp:ListItem>
                    <asp:ListItem Text="Ecuador" Value="Ecuador"></asp:ListItem>
                    <asp:ListItem Text="Egypt" Value="Egypt"></asp:ListItem>
                    <asp:ListItem Text="El Salvador" Value="El Salvador"></asp:ListItem>
                    <asp:ListItem Text="Equatorial Guinea" Value="Equatorial Guinea"></asp:ListItem>
                    <asp:ListItem Text="Eritrea" Value="Eritrea"></asp:ListItem>
                    <asp:ListItem Text="Estonia" Value="Estonia"></asp:ListItem>
                    <asp:ListItem Text="Ethiopia" Value="Ethiopia"></asp:ListItem>
                    <asp:ListItem Text="Falkland Islands" Value="Falkland Islands"></asp:ListItem>
                    <asp:ListItem Text="Faroe Islands" Value="Faroe Islands"></asp:ListItem>
                    <asp:ListItem Text="Fiji" Value="Fiji"></asp:ListItem>
                    <asp:ListItem Text="Finland" Value="Finland"></asp:ListItem>
                    <asp:ListItem Text="France" Value="France"></asp:ListItem>
                    <asp:ListItem Text="French Guiana" Value="French Guiana"></asp:ListItem>
                    <asp:ListItem Text="Gabon" Value="Gabon"></asp:ListItem>
                    <asp:ListItem Text="Gambia" Value="Gambia"></asp:ListItem>
                    <asp:ListItem Text="Georgia" Value="Georgia"></asp:ListItem>
                    <asp:ListItem Text="Germany" Value="Germany"></asp:ListItem>
                    <asp:ListItem Text="Ghana" Value="Ghana"></asp:ListItem>
                    <asp:ListItem Text="Gibraltar" Value="Gibraltar"></asp:ListItem>
                    <asp:ListItem Text="Greece" Value="Greece"></asp:ListItem>
                    <asp:ListItem Text="Greenland" Value="Greenland"></asp:ListItem>
                    <asp:ListItem Text="Grenada" Value="Grenada"></asp:ListItem>
                    <asp:ListItem Text="Guadeloupe (French)" Value="Guadeloupe (French)"></asp:ListItem>
                    <asp:ListItem Text="Guam (USA)" Value="Guam (USA)"></asp:ListItem>
                    <asp:ListItem Text="Guatemala" Value="Guatemala"></asp:ListItem>
                    <asp:ListItem Text="Guinea" Value="Guinea"></asp:ListItem>
                    <asp:ListItem Text="Guinea Bissau" Value="Guinea Bissau"></asp:ListItem>
                    <asp:ListItem Text="Guyana" Value="Guyana"></asp:ListItem>
                    <asp:ListItem Text="Haiti" Value="Haiti"></asp:ListItem>
                    <asp:ListItem Text="Holy See" Value="Holy See"></asp:ListItem>
                    <asp:ListItem Text="Honduras" Value="Honduras"></asp:ListItem>
                    <asp:ListItem Text="Hong Kong" Value="Hong Kong"></asp:ListItem>
                    <asp:ListItem Text="Hungary" Value="Hungary"></asp:ListItem>
                    <asp:ListItem Text="Iceland" Value="Iceland"></asp:ListItem>
                    <asp:ListItem Text="India" Value="India"></asp:ListItem>
                    <asp:ListItem Text="Indonesia" Value="Indonesia"></asp:ListItem>
                    <asp:ListItem Text="Iran" Value="Iran"></asp:ListItem>
                    <asp:ListItem Text="Iraq" Value="Iraq"></asp:ListItem>
                    <asp:ListItem Text="Ireland" Value="Ireland"></asp:ListItem>
                    <asp:ListItem Text="Israel" Value="Israel"></asp:ListItem>
                    <asp:ListItem Text="Italy" Value="Italy"></asp:ListItem>
                    <asp:ListItem Text="Ivory Coast (Cote D`Ivoire)" Value="Ivory Coast (Cote D`Ivoire)"></asp:ListItem>
                    <asp:ListItem Text="Jamaica" Value="Jamaica"></asp:ListItem>
                    <asp:ListItem Text="Japan" Value="Japan"></asp:ListItem>
                    <asp:ListItem Text="Jordan" Value="Jordan"></asp:ListItem>
                    <asp:ListItem Text="Kazakhstan" Value="Kazakhstan"></asp:ListItem>
                    <asp:ListItem Text="Kenya" Value="Kenya"></asp:ListItem>
                    <asp:ListItem Text="Kiribati" Value="Kiribati"></asp:ListItem>
                    <asp:ListItem Text="Kuwait" Value="Kuwait"></asp:ListItem>
                    <asp:ListItem Text="Kyrgyzstan" Value="Kyrgyzstan"></asp:ListItem>
                    <asp:ListItem Text="Laos" Value="Laos"></asp:ListItem>
                    <asp:ListItem Text="Latvia" Value="Latvia"></asp:ListItem>
                    <asp:ListItem Text="Lebanon" Value="Lebanon"></asp:ListItem>
                    <asp:ListItem Text="Lesotho" Value="Lesotho"></asp:ListItem>
                    <asp:ListItem Text="Liberia" Value="Liberia"></asp:ListItem>
                    <asp:ListItem Text="Libya" Value="Libya"></asp:ListItem>
                    <asp:ListItem Text="Liechtenstein" Value="Liechtenstein"></asp:ListItem>
                    <asp:ListItem Text="Lithuania" Value="Lithuania"></asp:ListItem>
                    <asp:ListItem Text="Luxembourg" Value="Luxembourg"></asp:ListItem>
                    <asp:ListItem Text="Macau" Value="Macau"></asp:ListItem>
                    <asp:ListItem Text="Macedonia" Value="Macedonia"></asp:ListItem>
                    <asp:ListItem Text="Madagascar" Value="Madagascar"></asp:ListItem>
                    <asp:ListItem Text="Malawi" Value="Malawi"></asp:ListItem>
                    <asp:ListItem Text="Malaysia" Value="Malaysia"></asp:ListItem>
                    <asp:ListItem Text="Maldives" Value="Maldives"></asp:ListItem>
                    <asp:ListItem Text="Mali" Value="Mali"></asp:ListItem>
                    <asp:ListItem Text="Malta" Value="Malta"></asp:ListItem>
                    <asp:ListItem Text="Marshall Islands" Value="Marshall Islands"></asp:ListItem>
                    <asp:ListItem Text="Martinique (French)" Value="Martinique (French)"></asp:ListItem>
                    <asp:ListItem Text="Mauritania" Value="Mauritania"></asp:ListItem>
                    <asp:ListItem Text="Mauritius" Value="Mauritius"></asp:ListItem>
                    <asp:ListItem Text="Mayotte" Value="Mayotte"></asp:ListItem>
                    <asp:ListItem Text="Mexico" Value="Mexico"></asp:ListItem>
                    <asp:ListItem Text="Micronesia" Value="Micronesia"></asp:ListItem>
                    <asp:ListItem Text="Moldova" Value="Moldova"></asp:ListItem>
                    <asp:ListItem Text="Monaco" Value="Monaco"></asp:ListItem>
                    <asp:ListItem Text="Mongolia" Value="Mongolia"></asp:ListItem>
                    <asp:ListItem Text="Montenegro" Value="Montenegro"></asp:ListItem>
                    <asp:ListItem Text="Montserrat" Value="Montserrat"></asp:ListItem>
                    <asp:ListItem Text="Morocco" Value="Morocco"></asp:ListItem>
                    <asp:ListItem Text="Mozambique" Value="Mozambique"></asp:ListItem>
                    <asp:ListItem Text="Myanmar" Value="Myanmar"></asp:ListItem>
                    <asp:ListItem Text="Namibia" Value="Namibia"></asp:ListItem>
                    <asp:ListItem Text="Nauru" Value="Nauru"></asp:ListItem>
                    <asp:ListItem Text="Nepal" Value="Nepal"></asp:ListItem>
                    <asp:ListItem Text="Netherlands" Value="Netherlands"></asp:ListItem>
                    <asp:ListItem Text="Netherlands Antilles" Value="Netherlands Antilles"></asp:ListItem>
                    <asp:ListItem Text="New Caledonia (French)" Value="New Caledonia (French)"></asp:ListItem>
                    <asp:ListItem Text="New Zealand" Value="New Zealand"></asp:ListItem>
                    <asp:ListItem Text="Nicaragua" Value="Nicaragua"></asp:ListItem>
                    <asp:ListItem Text="Niger" Value="Niger"></asp:ListItem>
                    <asp:ListItem Text="Nigeria" Value="Nigeria"></asp:ListItem>
                    <asp:ListItem Text="Niue" Value="Niue"></asp:ListItem>
                    <asp:ListItem Text="Norfolk Island" Value="Norfolk Island"></asp:ListItem>
                    <asp:ListItem Text="North Korea" Value="North Korea"></asp:ListItem>
                    <asp:ListItem Text="Northern Mariana Islands" Value="Northern Mariana Islands"></asp:ListItem>
                    <asp:ListItem Text="Norway" Value="Norway"></asp:ListItem>
                    <asp:ListItem Text="Oman" Value="Oman"></asp:ListItem>
                    <asp:ListItem Text="Pakistan" Value="Pakistan"></asp:ListItem>
                    <asp:ListItem Text="Palau" Value="Palau"></asp:ListItem>
                    <asp:ListItem Text="Panama" Value="Panama"></asp:ListItem>
                    <asp:ListItem Text="Papua New Guinea" Value="Papua New Guinea"></asp:ListItem>
                    <asp:ListItem Text="Paraguay" Value="Paraguay"></asp:ListItem>
                    <asp:ListItem Text="Peru" Value="Peru"></asp:ListItem>
                    <asp:ListItem Text="Philippines" Value="Philippines"></asp:ListItem>
                    <asp:ListItem Text="Pitcairn Island" Value="Pitcairn Island"></asp:ListItem>
                    <asp:ListItem Text="Poland" Value="Poland"></asp:ListItem>
                    <asp:ListItem Text="Polynesia (French)" Value="Polynesia (French)"></asp:ListItem>
                    <asp:ListItem Text="Portugal" Value="Portugal"></asp:ListItem>
                    <asp:ListItem Text="Puerto Rico" Value="Puerto Rico"></asp:ListItem>
                    <asp:ListItem Text="Qatar" Value="Qatar"></asp:ListItem>
                    <asp:ListItem Text="Reunion" Value="Reunion"></asp:ListItem>
                    <asp:ListItem Text="Romania" Value="Romania"></asp:ListItem>
                    <asp:ListItem Text="Russia" Value="Russia"></asp:ListItem>
                    <asp:ListItem Text="Rwanda" Value="Rwanda"></asp:ListItem>
                    <asp:ListItem Text="Saint Helena" Value="Saint Helena"></asp:ListItem>
                    <asp:ListItem Text="Saint Kitts and Nevis" Value="Saint Kitts and Nevis"></asp:ListItem>
                    <asp:ListItem Text="Saint Lucia" Value="Saint Lucia"></asp:ListItem>
                    <asp:ListItem Text="Saint Pierre and Miquelon" Value="Saint Pierre and Miquelon"></asp:ListItem>
                    <asp:ListItem Text="Saint Vincent and Grenadines" Value="Saint Vincent and Grenadines"></asp:ListItem>
                    <asp:ListItem Text="Samoa" Value="Samoa"></asp:ListItem>
                    <asp:ListItem Text="San Marino" Value="San Marino"></asp:ListItem>
                    <asp:ListItem Text="Sao Tome and Principe" Value="Sao Tome and Principe"></asp:ListItem>
                    <asp:ListItem Text="Saudi Arabia" Value="Saudi Arabia"></asp:ListItem>
                    <asp:ListItem Text="Senegal" Value="Senegal"></asp:ListItem>
                    <asp:ListItem Text="Serbia" Value="Serbia"></asp:ListItem>
                    <asp:ListItem Text="Seychelles" Value="Seychelles"></asp:ListItem>
                    <asp:ListItem Text="Sierra Leone" Value="Sierra Leone"></asp:ListItem>
                    <asp:ListItem Text="Singapore" Value="Singapore"></asp:ListItem>
                    <asp:ListItem Text="Slovakia" Value="Slovakia"></asp:ListItem>
                    <asp:ListItem Text="Slovenia" Value="Slovenia"></asp:ListItem>
                    <asp:ListItem Text="Solomon Islands" Value="Solomon Islands"></asp:ListItem>
                    <asp:ListItem Text="Somalia" Value="Somalia"></asp:ListItem>
                    <asp:ListItem Text="South Africa" Value="South Africa"></asp:ListItem>
                    <asp:ListItem Text="South Georgia and South Sandwich Islands" Value="South Georgia and South Sandwich Islands"></asp:ListItem>
                    <asp:ListItem Text="South Korea" Value="South Korea"></asp:ListItem>
                    <asp:ListItem Text="South Sudan" Value="South Sudan"></asp:ListItem>
                    <asp:ListItem Text="Spain" Value="Spain"></asp:ListItem>
                    <asp:ListItem Text="Sri Lanka" Value="Sri Lanka"></asp:ListItem>
                    <asp:ListItem Text="Sudan" Value="Sudan"></asp:ListItem>
                    <asp:ListItem Text="Suriname" Value="Suriname"></asp:ListItem>
                    <asp:ListItem Text="Svalbard and Jan Mayen Islands" Value="Svalbard and Jan Mayen Islands"></asp:ListItem>
                    <asp:ListItem Text="Swaziland" Value="Swaziland"></asp:ListItem>
                    <asp:ListItem Text="Sweden" Value="Sweden"></asp:ListItem>
                    <asp:ListItem Text="Switzerland" Value="Switzerland"></asp:ListItem>
                    <asp:ListItem Text="Syria" Value="Syria"></asp:ListItem>
                    <asp:ListItem Text="Taiwan" Value="Taiwan"></asp:ListItem>
                    <asp:ListItem Text="Tajikistan" Value="Tajikistan"></asp:ListItem>
                    <asp:ListItem Text="Tanzania" Value="Tanzania"></asp:ListItem>
                    <asp:ListItem Text="Thailand" Value="Thailand"></asp:ListItem>
                    <asp:ListItem Text="Timor-Leste (East Timor)" Value="Timor-Leste (East Timor)"></asp:ListItem>
                    <asp:ListItem Text="Togo" Value="Togo"></asp:ListItem>
                    <asp:ListItem Text="Tokelau" Value="Tokelau"></asp:ListItem>
                    <asp:ListItem Text="Tonga" Value="Tonga"></asp:ListItem>
                    <asp:ListItem Text="Trinidad and Tobago" Value="Trinidad and Tobago"></asp:ListItem>
                    <asp:ListItem Text="Tunisia" Value="Tunisia"></asp:ListItem>
                    <asp:ListItem Text="Turkey" Value="Turkey"></asp:ListItem>
                    <asp:ListItem Text="Turkmenistan" Value="Turkmenistan"></asp:ListItem>
                    <asp:ListItem Text="Turks and Caicos Islands" Value="Turks and Caicos Islands"></asp:ListItem>
                    <asp:ListItem Text="Tuvalu" Value="Tuvalu"></asp:ListItem>
                    <asp:ListItem Text="Uganda" Value="Uganda"></asp:ListItem>
                    <asp:ListItem Text="Ukraine" Value="Ukraine"></asp:ListItem>
                    <asp:ListItem Text="United Arab Emirates" Value="United Arab Emirates"></asp:ListItem>
                    <asp:ListItem Text="Uruguay" Value="Uruguay"></asp:ListItem>
                    <asp:ListItem Text="Uzbekistan" Value="Uzbekistan"></asp:ListItem>
                    <asp:ListItem Text="Vanuatu" Value="Vanuatu"></asp:ListItem>
                    <asp:ListItem Text="Venezuela" Value="Venezuela"></asp:ListItem>
                    <asp:ListItem Text="Vietnam" Value="Vietnam"></asp:ListItem>
                    <asp:ListItem Text="Virgin Islands" Value="Virgin Islands"></asp:ListItem>
                    <asp:ListItem Text="Wallis and Futuna Islands" Value="Wallis and Futuna Islands"></asp:ListItem>
                    <asp:ListItem Text="Yemen" Value="Yemen"></asp:ListItem>
                    <asp:ListItem Text="Zambia" Value="Zambia"></asp:ListItem>
                    <asp:ListItem Text="Zimbabwe" Value="Zimbabwe"></asp:ListItem>
                </asp:DropDownList>
            </div>
            <br class="clear" />
        </div>
        <br class="clear" />
    </div>

    <div style="padding-bottom:40px;">
    </div>
</div>

</asp:Content>
