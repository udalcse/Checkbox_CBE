////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
// This file and its contents are protected by United States and 
// International copyright laws. Unauthorized reproduction and/or 
// distribution of all or any portion of the code contained herein 
// is strictly prohibited and will result in severe civil and criminal 
// penalties. Any violations of this copyright will be prosecuted 
// to the fullest extent possible under law. 
// 
// THE SOURCE CODE CONTAINED HEREIN AND IN RELATED FILES IS PROVIDED 
// TO THE REGISTERED DEVELOPER FOR THE PURPOSES OF EDUCATION AND 
// TROUBLESHOOTING. UNDER NO CIRCUMSTANCES MAY ANY PORTION OF THE SOURCE
// CODE BE DISTRIBUTED, DISCLOSED OR OTHERWISE MADE AVAILABLE TO ANY 
// THIRD PARTY WITHOUT THE EXPRESS WRITTEN CONSENT OF XHEO. 
// 
// UNDER NO CIRCUMSTANCES MAY THE SOURCE CODE BE USED IN WHOLE OR IN 
// PART, AS THE BASIS FOR CREATING A PRODUCT THAT PROVIDES THE SAME, OR 
// SUBSTANTIALLY THE SAME, FUNCTIONALITY AS ANY XHEO PRODUCT. 
// 
// THE REGISTERED DEVELOPER ACKNOWLEDGES THAT THIS SOURCE CODE 
// CONTAINS VALUABLE AND PROPRIETARY TRADE SECRETS OF XHEO, THE 
// REGISTERED DEVELOPER AGREES TO EXPEND EVERY EFFORT TO INSURE ITS 
// CONFIDENTIALITY. 
// 
// THE END USER LICENSE AGREEMENT (EULA) ACCOMPANYING THE PRODUCT 
// PERMITS THE REGISTERED DEVELOPER TO REDISTRIBUTE THE PRODUCT IN 
// EXECUTABLE FORM ONLY IN SUPPORT OF APPLICATIONS WRITTEN USING 
// THE PRODUCT. IT DOES NOT PROVIDE ANY RIGHTS REGARDING THE 
// SOURCE CODE CONTAINED HEREIN. 
// 
// THIS COPYRIGHT NOTICE MAY NOT BE REMOVED FROM THIS FILE. 
#endregion
////////////////////////////////////////////////////////////////////////////////
//
// Class:		AddressCustomFormControl
// Author:		Paul Alexander
// Created:		Saturday, February 22, 2003 1:33:00 PM
//
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Xheo.Licensing
{
	/// <summary>
	/// Implements a US address form for the <see cref="RegistrationForm"/>.
	/// <seealso cref="ICustomFormControl"/>.
	/// </summary>
	/// <remarks>
	/// The AddressCustomFormControl adds the following fields to the registration
	/// form
	/// <div class="tablediv"><table cellspacing="0" class="dtTABLE">
	/// 	<tr valign="top">
	/// 	<th width="50%">Name</th>
	/// 	<th width="50%">Use</th>
	/// 	</tr>
	/// 	<tr>
	/// 		<td>country</td>
	/// 		<td>The users's country. The value is one of the US Postal service's two letter country indentification codes.</td>
	/// 	</tr>
	/// 	<tr>
	/// 		<td>streetAddress</td>
	/// 		<td>The users's street address.</td>
	/// 	</tr>
	/// 	<tr>
	/// 		<td>city</td>
	/// 		<td>The users's city.</td>
	/// 	</tr>
	/// 	<tr>
	/// 		<td>state</td>
	/// 		<td>The users's city or province. When country is US, then contains one of the US Postal service's two letter state abbreviations.</td>
	/// 	</tr>
	/// 	<tr>
	/// 		<td>zipcode</td>
	/// 		<td>The users's zip or postal code.</td>
	/// 	</tr>
	/// 	<tr>
	/// 		<td>phone</td>
	/// 		<td>The users's phone number.</td>
	/// 	</tr>
	/// </table>
	/// </div>
	/// </remarks>
#if LICENSING
	public 
#else
	internal
#endif
		class AddressCustomFormControl : System.Windows.Forms.UserControl, ICustomFormControl
	{
		////////////////////////////////////////////////////////////////////////////////
		#region Fields
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _street;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox _zipCode;
		private System.Windows.Forms.TextBox _city;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox _phone;
		private System.Windows.Forms.TextBox _state;
		private System.Windows.Forms.ComboBox _usState;
		private System.Windows.Forms.ComboBox _country;
		private System.Windows.Forms.Label label6;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Constructors/Destructors

		/// Initializes a new instance of the AddressCustomFormControl class.
		public AddressCustomFormControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this._street = new System.Windows.Forms.TextBox();
			this._city = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this._zipCode = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this._phone = new System.Windows.Forms.TextBox();
			this._state = new System.Windows.Forms.TextBox();
			this._usState = new System.Windows.Forms.ComboBox();
			this._country = new System.Windows.Forms.ComboBox();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 48);
			this.label1.TabIndex = 2;
			this.label1.Text = "#UIAD_StreetAddress";
			// 
			// _street
			// 
			this._street.AcceptsReturn = true;
			this._street.Location = new System.Drawing.Point(104, 32);
			this._street.MaxLength = 256;
			this._street.Multiline = true;
			this._street.Name = "_street";
			this._street.Size = new System.Drawing.Size(336, 44);
			this._street.TabIndex = 3;
			this._street.Text = "";
			// 
			// _city
			// 
			this._city.Location = new System.Drawing.Point(104, 88);
			this._city.MaxLength = 128;
			this._city.Name = "_city";
			this._city.Size = new System.Drawing.Size(336, 20);
			this._city.TabIndex = 5;
			this._city.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(0, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 20);
			this.label4.TabIndex = 4;
			this.label4.Text = "#UIAD_City";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 20);
			this.label2.TabIndex = 6;
			this.label2.Text = "#UIAD_State";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(0, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 20);
			this.label3.TabIndex = 9;
			this.label3.Text = "#UIAD_Zip";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _zipCode
			// 
			this._zipCode.Location = new System.Drawing.Point(104, 152);
			this._zipCode.MaxLength = 20;
			this._zipCode.Name = "_zipCode";
			this._zipCode.Size = new System.Drawing.Size(176, 20);
			this._zipCode.TabIndex = 10;
			this._zipCode.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(0, 184);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 20);
			this.label5.TabIndex = 11;
			this.label5.Text = "#UIAD_Phone";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _phone
			// 
			this._phone.Location = new System.Drawing.Point(104, 184);
			this._phone.MaxLength = 30;
			this._phone.Name = "_phone";
			this._phone.Size = new System.Drawing.Size(176, 20);
			this._phone.TabIndex = 12;
			this._phone.Text = "";
			// 
			// _state
			// 
			this._state.Location = new System.Drawing.Point(104, 120);
			this._state.MaxLength = 128;
			this._state.Name = "_state";
			this._state.Size = new System.Drawing.Size(336, 20);
			this._state.TabIndex = 8;
			this._state.Text = "";
			// 
			// _usState
			// 
			this._usState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._usState.Items.AddRange(new object[] {
														  "AL",
														  "AK",
														  "AS",
														  "AZ",
														  "AR",
														  "CA",
														  "CO",
														  "CT",
														  "DE",
														  "DC",
														  "FM",
														  "FL",
														  "GA",
														  "GU",
														  "HI",
														  "ID",
														  "IL",
														  "IN",
														  "IA",
														  "KS",
														  "KY",
														  "LA",
														  "ME",
														  "MH",
														  "MD",
														  "MA",
														  "MI",
														  "MN",
														  "MS",
														  "MO",
														  "MT",
														  "NE",
														  "NV",
														  "NH",
														  "NJ",
														  "NM",
														  "NY",
														  "NC",
														  "ND",
														  "MP",
														  "OH",
														  "OK",
														  "OR",
														  "PW",
														  "PA",
														  "PR",
														  "RI",
														  "SC",
														  "SD",
														  "TN",
														  "TX",
														  "UT",
														  "VT",
														  "VI",
														  "VA",
														  "WA",
														  "WV",
														  "WI",
														  "WY"});
			this._usState.Location = new System.Drawing.Point(104, 120);
			this._usState.Name = "_usState";
			this._usState.Size = new System.Drawing.Size(64, 21);
			this._usState.TabIndex = 7;
			this._usState.Visible = false;
			// 
			// _country
			// 
			this._country.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._country.ItemHeight = 13;
			this._country.Location = new System.Drawing.Point(104, 0);
			this._country.Name = "_country";
			this._country.Size = new System.Drawing.Size(176, 21);
			this._country.TabIndex = 1;
			this._country.SelectedIndexChanged += new System.EventHandler(this._country_SelectedIndexChanged);
			this._country.Items.AddRange(new object[] {
														  "US - United States",
														  "CA - Canada",
														  "AF - Afghanistan, Islamic State of",
														  "AL - Albania",
														  "DZ - Algeria",
														  "AS - American Samoa",
														  "AO - Angola",
														  "AI - Anguilla",
														  "AQ - Antarctica",
														  "AG - Antigua and Barbuda",
														  "AR - Argentina",
														  "AM - Armenia",
														  "AW - Aruba",
														  "AU - Australia",
														  "AT - Austria",
														  "AZ - Azerbaidjan",
														  "BS - Bahamas",
														  "BH - Bahrain",
														  "BD - Bangladesh",
														  "BB - Barbados",
														  "BY - Belarus",
														  "BE - Belgium",
														  "BZ - Belize",
														  "BJ - Benin",
														  "BM - Bermuda",
														  "BT - Bhutan",
														  "BO - Bolivia",
														  "BA - Bosnia-Herzegovina",
														  "BW - Botswana",
														  "BV - Bouvet Island",
														  "BR - Brazil",
														  "IO - British Indian Ocean Territory",
														  "BN - Brunei Darussalam",
														  "BG - Bulgaria",
														  "BF - Burkina Faso",
														  "BI - Burundi",
														  "KH - Cambodia, Kingdom of",
														  "CM - Cameroon",
														  "CV - Cape Verde",
														  "KY - Cayman Islands",
														  "CF - Central African Republic",
														  "TD - Chad",
														  "CL - Chile",
														  "CN - China",
														  "CX - Christmas Island",
														  "CC - Cocos (Keeling) Islands",
														  "CO - Colombia",
														  "KM - Comoros",
														  "CG - Congo",
														  "CD - Congo, The Democratic Republic of the",
														  "CK - Cook Islands",
														  "CR - Costa Rica",
														  "HR - Croatia",
														  "CU - Cuba",
														  "CY - Cyprus",
														  "CZ - Czech Republic",
														  "DK - Denmark",
														  "DJ - Djibouti",
														  "DM - Dominica",
														  "DO - Dominican Republic",
														  "TP - East Timor",
														  "EC - Ecuador",
														  "EG - Egypt",
														  "SV - El Salvador",
														  "GQ - Equatorial Guinea",
														  "ER - Eritrea",
														  "EE - Estonia",
														  "ET - Ethiopia",
														  "FK - Falkland Islands",
														  "FO - Faroe Islands",
														  "FJ - Fiji",
														  "FI - Finland",
														  "CS - Former Czechoslovakia",
														  "SU - Former USSR",
														  "FR - France",
														  "FX - France (European Territory)",
														  "GF - French Guyana",
														  "TF - French Southern Territories",
														  "GA - Gabon",
														  "GM - Gambia",
														  "GE - Georgia",
														  "DE - Germany",
														  "GH - Ghana",
														  "GI - Gibraltar",
														  "GB - Great Britain",
														  "GR - Greece",
														  "GL - Greenland",
														  "GD - Grenada",
														  "GP - Guadeloupe (French)",
														  "GU - Guam (USA)",
														  "GT - Guatemala",
														  "GN - Guinea",
														  "GW - Guinea Bissau",
														  "GY - Guyana",
														  "HT - Haiti",
														  "HM - Heard and McDonald Islands",
														  "VA - Holy See (Vatican City State)",
														  "HN - Honduras",
														  "HK - Hong Kong",
														  "HU - Hungary",
														  "IS - Iceland",
														  "IN - India",
														  "ID - Indonesia",
														  "IR - Iran",
														  "IQ - Iraq",
														  "IE - Ireland",
														  "IL - Israel",
														  "IT - Italy",
														  "CI - Ivory Coast (Cote D\'Ivoire)",
														  "JM - Jamaica",
														  "JP - Japan",
														  "JO - Jordan",
														  "KZ - Kazakhstan",
														  "KE - Kenya",
														  "KI - Kiribati",
														  "KW - Kuwait",
														  "KG - Kyrgyz Republic (Kyrgyzstan)",
														  "LA - Laos",
														  "LV - Latvia",
														  "LB - Lebanon",
														  "LS - Lesotho",
														  "LR - Liberia",
														  "LY - Libya",
														  "LI - Liechtenstein",
														  "LT - Lithuania",
														  "LU - Luxembourg",
														  "MO - Macau",
														  "MK - Macedonia",
														  "MG - Madagascar",
														  "MW - Malawi",
														  "MY - Malaysia",
														  "MV - Maldives",
														  "ML - Mali",
														  "MT - Malta",
														  "MH - Marshall Islands",
														  "MQ - Martinique (French)",
														  "MR - Mauritania",
														  "MU - Mauritius",
														  "YT - Mayotte",
														  "MX - Mexico",
														  "FM - Micronesia",
														  "MD - Moldavia",
														  "MC - Monaco",
														  "MN - Mongolia",
														  "MS - Montserrat",
														  "MA - Morocco",
														  "MZ - Mozambique",
														  "MM - Myanmar",
														  "NA - Namibia",
														  "NR - Nauru",
														  "NP - Nepal",
														  "NL - Netherlands",
														  "AN - Netherlands Antilles",
														  "NT - Neutral Zone",
														  "NC - New Caledonia (French)",
														  "NZ - New Zealand",
														  "NI - Nicaragua",
														  "NE - Niger",
														  "NG - Nigeria",
														  "NU - Niue",
														  "NF - Norfolk Island",
														  "KP - North Korea",
														  "MP - Northern Mariana Islands",
														  "NO - Norway",
														  "OM - Oman",
														  "PK - Pakistan",
														  "PW - Palau",
														  "PA - Panama",
														  "PG - Papua New Guinea",
														  "PY - Paraguay",
														  "PE - Peru",
														  "PH - Philippines",
														  "PN - Pitcairn Island",
														  "PL - Poland",
														  "PF - Polynesia (French)",
														  "PT - Portugal",
														  "PR - Puerto Rico",
														  "QA - Qatar",
														  "RE - Reunion (French)",
														  "RO - Romania",
														  "RU - Russian Federation",
														  "RW - Rwanda",
														  "GS - S. Georgia & S. Sandwich Isls.",
														  "SH - Saint Helena",
														  "KN - Saint Kitts & Nevis Anguilla",
														  "LC - Saint Lucia",
														  "PM - Saint Pierre and Miquelon",
														  "ST - Saint Tome (Sao Tome) and Principe",
														  "VC - Saint Vincent & Grenadines",
														  "WS - Samoa",
														  "SM - San Marino",
														  "SA - Saudi Arabia",
														  "SN - Senegal",
														  "SC - Seychelles",
														  "SL - Sierra Leone",
														  "SG - Singapore",
														  "SK - Slovak Republic",
														  "SI - Slovenia",
														  "SB - Solomon Islands",
														  "SO - Somalia",
														  "ZA - South Africa",
														  "KR - South Korea",
														  "ES - Spain",
														  "LK - Sri Lanka",
														  "SD - Sudan",
														  "SR - Suriname",
														  "SJ - Svalbard and Jan Mayen Islands",
														  "SZ - Swaziland",
														  "SE - Sweden",
														  "CH - Switzerland",
														  "SY - Syria",
														  "TJ - Tadjikistan",
														  "TW - Taiwan",
														  "TZ - Tanzania",
														  "TH - Thailand",
														  "TG - Togo",
														  "TK - Tokelau",
														  "TO - Tonga",
														  "TT - Trinidad and Tobago",
														  "TN - Tunisia",
														  "TR - Turkey",
														  "TM - Turkmenistan",
														  "TC - Turks and Caicos Islands",
														  "TV - Tuvalu",
														  "UG - Uganda",
														  "UA - Ukraine",
														  "AE - United Arab Emirates",
														  "UK - United Kingdom",
														  "UY - Uruguay",
														  "UM - USA Minor Outlying Islands",
														  "UZ - Uzbekistan",
														  "VU - Vanuatu",
														  "VE - Venezuela",
														  "VN - Vietnam",
														  "VG - Virgin Islands (British)",
														  "VI - Virgin Islands (USA)",
														  "WF - Wallis and Futuna Islands",
														  "EH - Western Sahara",
														  "YE - Yemen",
														  "YU - Yugoslavia",
														  "ZR - Zaire",
														  "ZM - Zambia",
														  "ZW - Zimbabwe",
														  "XX - Other"});
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 0);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 20);
			this.label6.TabIndex = 0;
			this.label6.Text = "#UIAD_Country";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// AddressCustomFormControl
			// 
			this.Controls.Add(this._usState);
			this.Controls.Add(this._country);
			this.Controls.Add(this._street);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._city);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this._zipCode);
			this.Controls.Add(this.label5);
			this.Controls.Add(this._phone);
			this.Controls.Add(this._state);
			this.Controls.Add(this.label6);
			this.Name = "AddressCustomFormControl";
			this.Size = new System.Drawing.Size(440, 205);
			this.ResumeLayout(false);

		}
		#endregion

		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Properties


		#endregion
		////////////////////////////////////////////////////////////////////////////////

		////////////////////////////////////////////////////////////////////////////////
		#region Operations

		/// <summary>
		/// Initializes the fields on the control from the collection of values.
		/// </summary>
		/// <param name="prefix">
		///		The prefix to use on each key when searching for a value for 
		///		this control.
		/// </param>
		/// <param name="values">
		///		Collection of values to use when initializing.
		/// </param>
		public void InitializeFields( string prefix, NameValueCollection values )
		{
			if( values[ prefix + "streetaddress" ] != null )
				_street.Text = values[ prefix + "streetaddress" ];
			if( values[ prefix + "city" ] != null )
				_city.Text = values[ prefix + "city" ];

			bool us = false;
			if( values[ prefix + "country" ] != null )
			{
				string country = values[ prefix + "country" ];
				foreach( string item in _country.Items )
					if( item.StartsWith( country ) )
					{
						_country.SelectedItem = item;
						if( item.StartsWith( "US" ) )
							us = true;
						break;
					}
			}

			if( us )
			{
				if( values[ prefix + "state" ] != null )
				{
					string state = values[ prefix + "state" ];
					foreach( string item in _usState.Items )
					{
						if( item.StartsWith( state ) )
						{
							_usState.SelectedItem = item;
							break;
						}
					}
				}
			}
			else
			{
				if( values[ prefix + "state" ] != null )
					_state.Text = values[ prefix + "state" ];
			}

			if( values[ prefix + "zipcode" ] != null )
				_zipCode.Text = values[ prefix + "zipcode" ];

			if( values[ prefix + "phone" ] != null )
				_phone.Text = values[ prefix + "phone" ];
		}

		/// <summary>
		/// Performs validation on the field in the control.
		/// </summary>
		/// <param name="field">
		///		The field to validate.
		/// </param>
		/// <returns>
		///		Returns true if the field value is valid, otherwise false.
		/// </returns>
		public bool ValidateField( string field )
		{
			string message = null;
						
			switch( field )
			{
				case "streetaddress":
					if( _street.Text == null || _street.Text.Length == 0 )
					{
						message = Internal.StaticResourceProvider.CurrentProvider.GetString( "MAD_EnterStreet" );
						_street.Focus();
					}
					break;
				case "city":
					if( _city.Text == null || _city.Text.Length == 0 )
					{
						message = Internal.StaticResourceProvider.CurrentProvider.GetString( "MAD_EnterCity" );
						_city.Focus();
					}
					break;
				case "state":
					if( _usState.Visible )
					{
						if( _usState.SelectedIndex == - 1)
						{
							message = Internal.StaticResourceProvider.CurrentProvider.GetString( "MAD_EnterState" );
							_usState.Focus();
						}
					}
					else
					{
						if( _state.Text == null || _state.Text.Length == 0 )
						{
							message = Internal.StaticResourceProvider.CurrentProvider.GetString( "MAD_EnterState" );
							_state.Focus();
						}
					}
					break;
				case "province":
					goto case "state";
				case "zipcode":
					if( _zipCode.Text == null || _zipCode.Text.Length == 0 )
					{
						message = Internal.StaticResourceProvider.CurrentProvider.GetString( "MAD_EnterZip" );
						_zipCode.Focus();
					}
					break;
				case "postalcode":
					goto case "zipcode";
				case "phone":
					if( _phone.Text == null || _phone.Text.Length == 0 )
					{
						message = Internal.StaticResourceProvider.CurrentProvider.GetString( "MAD_EnterPhone" );
						_phone.Focus();
					}
					break;
				case "country":
					if( _country.SelectedIndex == -1 )
					{
						message = Internal.StaticResourceProvider.CurrentProvider.GetString( "MAD_EnterCountry" );
						_country.Focus();
					}
					break;
			}

			if( message != null )
			{
				MessageBox.Show(
					this.FindForm(),
					message,
					"Error",
					MessageBoxButtons.OK,
					MessageBoxIcon.Error );
				return false;
			}

			return true;
		}

		/// <summary>
		/// Adds the values of the fields to the value collection. Multiple values can
		/// be assigned to the same key.
		/// </summary>
		/// <param name="values">
		///		A collection of name/value pairs to add the field values to.
		/// </param>
		public void AddValues( LicenseValuesCollection values )
		{
			values.Add( "StreetAddress", _street.Text );
			values.Add( "City", _city.Text );
			if( _usState.Visible )
			{
				if( _usState.Text != null && _usState.Text.Length > 0 )
					values.Add( "State", _usState.Text.Substring( 0, 2 ) );
			}
			else
			{
				values.Add( "State", _state.Text );
			}
			values.Add( "Zip", _zipCode.Text );
			if( _country.Text != null && _country.Text.Length > 0 )
				values.Add( "Country",  _country.Text.Substring( 0, 2 ) );
			values.Add( "Phone", _phone.Text );
		}

		private void _country_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if( _country.SelectedItem != null && _country.Text == "US - United States" )
			{
				_usState.Visible = true;
				_state.Visible = false;
			}
			else
			{
				_state.Visible = true;
				_usState.Visible = false;
			}
		}

		#endregion
		////////////////////////////////////////////////////////////////////////////////
	} // End class AddressCustomFormControl
} // End namespace Xheo.Licensing

////////////////////////////////////////////////////////////////////////////////
#region Copyright © 2002-2005 XHEO, INC. All Rights Reserved.
//
#endregion
////////////////////////////////////////////////////////////////////////////////