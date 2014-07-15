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
using System.Net.Mail;
using NetEmailer.TokenMapping;
using NetEmailer.Configuration;



namespace NetEmailer
{
	/// <summary>
	/// Class provides the functionalities to parse the configuration file and build the EmailMessage object
	/// </summary>
	public sealed class Emailer
	{

		/// <overloads>
		/// + 1 Overloads.
		/// </overloads>
		/// <summary>
		/// Parse the configuration file and build the EmailMessage object
		/// </summary>
		/// <param name="configFilePath">Config file fullpath</param>
		/// <param name="key">Emailconfig section key</param>
		/// <param name="parameterProvider">Parameter value provider</param>
		/// <returns>EmailMessage that contains the email information</returns>
		public static EmailMessage BuildMessage(
			string configFilePath,
			string key,
			IParameterProvider parameterProvider)
		{

			return BuildMessage(configFilePath,key,string.Empty,parameterProvider);
		}


		/// <overloads>
		/// + 1 Overloads.
		/// </overloads>
		/// <summary>
		/// Parse the configuration file and build the EmailMessage object
		/// </summary>
		/// <param name="configFilePath">Configuration file fullpath</param>
		/// <param name="key">Emailconfig section key</param>
		/// <param name="message">Message text</param>
		/// <param name="parameterProvider">Parameter value provider</param>
		/// <returns>EmailMessage that contains the email information</returns>
		public static EmailMessage BuildMessage(
			string configFilePath,
			string key,
			string message,
			IParameterProvider parameterProvider)
		{
			IEmailerConfig emailerConfig=EmailerConfigManager.GetConfig(configFilePath);
			EmailerSection emailerSection=emailerConfig[key];
			string server,from,to,subject,cc,bcc,priority,attachments,
                isBodyHtml,enableSsl,timeout;
            bool bIsBodyHtml = false,bEnableSSL=false;
			int maxNumberOfRecipientsPerEmail=int.MaxValue,iTimeout=100000,iOut;
			string sMaxNumberOfRecipientsPerEmail=TokenMapper.ParseParameterizedString(emailerSection.MaxNumberOfRecipientsPerEmail,parameterProvider);
            if (!string.IsNullOrEmpty(sMaxNumberOfRecipientsPerEmail))
            {
                if (Int32.TryParse(sMaxNumberOfRecipientsPerEmail, out iOut))
                    if(iOut>0)
                        maxNumberOfRecipientsPerEmail = iOut;
            }
            timeout = TokenMapper.ParseParameterizedString(emailerSection.Timeout, parameterProvider);
            if (!string.IsNullOrEmpty(timeout))
            {
                if (Int32.TryParse(timeout, out iOut))
                    if (iOut > 0)
                        iTimeout = iOut;
            }
            isBodyHtml = TokenMapper.ParseParameterizedString(emailerSection.IsBodyHtml, parameterProvider);
            if (!string.IsNullOrEmpty(isBodyHtml) && isBodyHtml.Trim().ToLower()=="true")
            {
                bIsBodyHtml = true;
            }
            enableSsl = TokenMapper.ParseParameterizedString(emailerSection.EnableSSL, parameterProvider);
            if (!string.IsNullOrEmpty(enableSsl) && enableSsl.Trim().ToLower() == "true")
            {
                bEnableSSL = true;
            } 
            server = emailerSection.SmtpServer;
			from=emailerSection.From;
			to=emailerSection.To;
			cc=emailerSection.CC;
			bcc=emailerSection.Bcc;
			subject=emailerSection.Subject;
			priority=emailerSection.Priority;
			attachments=emailerSection.Attachments;
            if(string.IsNullOrEmpty(message))
                message=emailerSection.Body;

			return new EmailMessage(TokenMapper.ParseParameterizedString(message,parameterProvider),
				TokenMapper.ParseParameterizedString(server,parameterProvider),
				TokenMapper.ParseParameterizedString(to,parameterProvider),
				TokenMapper.ParseParameterizedString(from,parameterProvider),
				TokenMapper.ParseParameterizedString(subject,parameterProvider),
				TokenMapper.ParseParameterizedString(cc,parameterProvider),
				TokenMapper.ParseParameterizedString(bcc,parameterProvider),
				(MailPriority) Enum.Parse(typeof(MailPriority),TokenMapper.ParseParameterizedString(priority,parameterProvider), true),
				maxNumberOfRecipientsPerEmail,
				attachments,
                bIsBodyHtml,
                bEnableSSL,
                iTimeout);
		}
	}// END CLASS DEFINITION Emailer
}
