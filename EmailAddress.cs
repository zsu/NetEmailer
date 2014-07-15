#region License
// Copyright 2006 Zhicheng Su
//
//Licensed under the Apache License, Version 2.0 (the "License"); 
//you may not use this file except in compliance with the License. 
//You may obtain a copy of the License at 

//http://www.apache.org/licenses/LICENSE-2.0 

//Unless required by applicable law or agreed to in writing, software 
//distributed under the License is distributed on an "AS IS" BASIS, 
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
//See the License for the specific language governing permissions and 
//limitations under the License. 
#endregion
using System;
using System.Text;
namespace NetEmailer
{

	/// <summary>
	/// Email address class
	/// </summary>
	public class EmailAddress
	{
		StringBuilder _emailAddresses=new StringBuilder();
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="emailAddresses">Email addresses seperated by comma or semicolon</param>
		public EmailAddress(string emailAddresses)
		{
            Add(emailAddresses);
		}

		/// <summary>
		/// Add email addresses to the end of the list.
		/// </summary>
        /// <param name="emailAddresses">Email addresses seperated by comma or semicolon</param>
		/// <returns>Email address list</returns>
		public string Add(string emailAddresses)
		{
            if (emailAddresses != null && emailAddresses.Trim() != string.Empty)
            {
                string[] addresses = emailAddresses.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string address in addresses)
                    _emailAddresses.AppendFormat("{0};", address);
            }
			string emails= _emailAddresses.ToString();
            if (emails != null && emails.EndsWith(";"))
                emails = emails.Remove(emails.Length - 1, 1);
            return emails;
		}

		/// <summary>
		/// Clear the Email address list
		/// </summary>
		public void Clear()
		{
			_emailAddresses=new StringBuilder();
		}

		/// <summary>
        /// Email addresses seperated by comma or semicolon
		/// </summary>
		public string Address
		{
			get{
                string emails = _emailAddresses.ToString();
                if (emails != null && emails.EndsWith(";"))
                    emails = emails.Remove(emails.Length - 1, 1);
                return emails;
            }
		}

	}// END CLASS DEFINITION EmailAddress
}
