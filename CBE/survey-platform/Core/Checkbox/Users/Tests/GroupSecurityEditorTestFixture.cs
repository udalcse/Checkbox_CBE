#if UNIT_TESTS
using System;
using System.Data;
using NUnit.Framework;
using Prezza.Framework.Common.Tests;
using System.Security.Principal;

using Prezza.Framework.Security;
using Checkbox.Security;
using Checkbox.Users;
using Checkbox.Users.Security;
using Prezza.Framework.Security.Principal;

namespace Checkbox.Users.Tests
{
	/// <summary>
	/// Summary description for GroupSecurityEditorTestFixture.
	/// </summary>
	[TestFixture]
	public class GroupSecurityEditorTestFixture : TestFixture
	{
		private SecurityEditor editor;

		public GroupSecurityEditorTestFixture() : base("GroupSecurityEditorTestFixture")
		{
		}

		[TestFixtureSetUp]
		public void CreateSecurityEditor()
		{
			System.Threading.Thread.CurrentPrincipal = UserManager.GetUser("peter");
			Group group = Group.GetGroup(1005);
			Assert.IsNotNull(group, "Error loading Group");
			editor = new GroupSecurityEditor(group);
			Assert.IsNotNull(editor, "Error creating editor");
		}

		[Test]
		public void GetEntriesInAcl()
		{
			Assert.IsNotNull(editor, "Editor was not loaded");
			DataTable list = editor.List();
			Assert.IsNotNull(list, "List() return nothing");
			foreach(DataRow row in list.Rows)
			{
				Log("Found Entry");
				for(int i = 0; i < row.ItemArray.Length; i++)
				{
					Log(list.Columns[i].ColumnName + ": " + row.ItemArray[i].ToString());
				}
			}
		}

		[Test]
		public void GetAvailable()
		{
			DataTable available = editor.GetAccessPermissible();
			Assert.IsNotNull(available, "Error in GetAccessPermissible()");

			Log("Count of users: " + available.Rows.Count);
			foreach(DataRow row in available.Rows)
			{
				Log("Found Available");
				for(int i = 0; i < row.ItemArray.Length; i++)
				{
					Log(available.Columns[i].ColumnName + ": " + row.ItemArray[i].ToString());
				}
			}
		}

		[Test]
		public void GrantAccess()
		{
			DataTable available = editor.GetAccessPermissible();
			Assert.IsNotNull(available, "Error in GetAccessPermissible()");

			EntryData[] entries = new EntryData[available.Rows.Count];
			for(int i = 0; i < available.Rows.Count; i++)
			{
				entries[i] = new EntryData((string)available.Rows[i]["EntryType"], (string)available.Rows[i]["EntryIdentifier"]);
			}
			string message;
			Assert.IsTrue(editor.GrantAccess(entries, out message), "Error Granting Access");
			Log("GrantAccess() out message is: " + message);
			
		}
	}
}

#endif
