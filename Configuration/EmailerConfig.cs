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
using System.IO;
using System.Xml;
using System.Web;
using System.Text;
using System.Web.Caching;
using System.Collections;
using NetEmailer.Common;
using System.Configuration;
using System.Threading;
using System.Collections.Generic;


namespace NetEmailer.Configuration
{
	/// <summary>
	/// Summary description for DataConfig.
	/// </summary>
    public sealed class EmailerConfig : IEmailerConfig, IConfigurable, IDisposable
	{
        private bool disposed = false;
        private ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        private Dictionary<string, EmailerSection> _emailerSections;
        private const string EmailerSection = "emailer";
		private const string Body="body";
		private const string SmtpServer="smtpServer";
		private const string MaxNumberOfRecipientsPerEmail="maxNumberOfRecipientsPerEmail";
		private const string From="from";
		private const string To="to";
		private const string CC="cc";
		private const string Bcc="bcc";
		private const string Priority="priority";
		private const string Subject="subject";
		private const string Attachments="attachments";
        private const string IsBodyHtml = "isBodyHtml";
        private const string EnableSsl = "enableSsl";
        private const string TimeOut = "timeout";
        private const string TagEmail = "email";
        private const string TagAdd = "add";
        private const string AttributeName = "name";
        private const string AttributeValue = "value";

        /// <summary>
        /// Return all emailer sectioins
        /// </summary>
        public Dictionary<string, EmailerSection> EmailerSections
        {
            get { return _emailerSections; }
        }

		/// <summary>
		/// Return the specific section config
		/// </summary>
		public EmailerSection this[string name]
		{
			get
			{
                return _emailerSections[name] as EmailerSection;
			}
		}
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sConfigFilePath">Configuration file full path</param>
		public EmailerConfig(string configFilePath)
		{
            Configure(configFilePath);
		}

        #region IConfigurable Members
        public void Configure(string configFilePath)
        {
            Check.IsNotEmpty(configFilePath, "configFilePath");
            configFilePath = Util.GetFullPath(configFilePath);
            if (!File.Exists(configFilePath))
            {
                throw new FileNotFoundException(String.Format(NetEmailer.File_Not_Found, configFilePath));
            }
            ConfigXmlDocument document1 = new ConfigXmlDocument();
            try
            {
                document1.Load(configFilePath);
            }
            catch (XmlException exception1)
            {
                throw new ConfigurationErrorsException(exception1.Message, exception1, configFilePath, exception1.LineNumber);
            }
            XmlNode root = document1.DocumentElement;
            _locker.EnterWriteLock();
            try
            {
                HandlerBase.CheckForUnrecognizedAttributes(root);
                XmlNodeList configNodeList = document1.GetElementsByTagName(EmailerSection);
                if (configNodeList.Count == 0)
                {
                    throw new System.Exception(string.Format("{0}: XML configuration does not contain a {1} element. Configuration aborted.", configFilePath, EmailerSection));
                }
                else if (configNodeList.Count > 1)
                {
                    throw new System.Exception(string.Format("{0}: XML configuration contains {1} {2} elements. Only one is allowed. Configuration aborted.", configFilePath, configNodeList.Count, EmailerSection));
                }
                else
                {
                    XmlDocument newDoc = new XmlDocument();
                    XmlElement element = configNodeList[0] as XmlElement;
                    if (element == null)
                        throw new System.Exception(string.Format("{0}: {1} must be an element. Configuration aborted", configFilePath, EmailerSection));
                    else
                    {
                        ParseXml(element);
                    }
                }
            }
            finally
            {
                _locker.ExitWriteLock();
            }
            FileWatchHandler fileWatchHandler = new FileWatchHandler(this, new FileInfo(configFilePath));
            fileWatchHandler.StartWatching();
        }
        #endregion
        private void ParseXml(XmlElement element)
        {
            if (element == null)
                return;

            EmailerSection section, defaultSection;
            _emailerSections = new Dictionary<string,EmailerSection>();

			#region get default settings
            defaultSection = new EmailerSection(
                element.Attributes[SmtpServer] == null ? String.Empty : element.Attributes[SmtpServer].Value,
                element.Attributes[MaxNumberOfRecipientsPerEmail] == null ? String.Empty : element.Attributes[MaxNumberOfRecipientsPerEmail].Value,
                element.Attributes[Priority] == null ? String.Empty : element.Attributes[Priority].Value,
                element.Attributes[From] == null ? String.Empty : element.Attributes[From].Value,
                element.Attributes[To] == null ? String.Empty : element.Attributes[To].Value,
                element.Attributes[CC] == null ? String.Empty : element.Attributes[CC].Value,
                element.Attributes[Bcc] == null ? String.Empty : element.Attributes[Bcc].Value,
                element.Attributes[Subject] == null ? String.Empty : element.Attributes[Subject].Value,
                element.Attributes[Body] == null ? String.Empty : element.Attributes[Body].Value,
                element.Attributes[Attachments] == null ? String.Empty : element.Attributes[Attachments].Value,
                element.Attributes[IsBodyHtml] == null ? String.Empty : element.Attributes[IsBodyHtml].Value,
                element.Attributes[EnableSsl] == null ? String.Empty : element.Attributes[EnableSsl].Value,
                element.Attributes[TimeOut] == null ? String.Empty : element.Attributes[TimeOut].Value);
			#endregion
			#region Load Sections' settings
            foreach (XmlNode currentNode in element.ChildNodes)
            {
                if (currentNode.NodeType == XmlNodeType.Element)
                {
                    XmlElement currentElement = (XmlElement)currentNode;

                    if (currentElement.LocalName == TagEmail && currentElement.Attributes[AttributeName]!=null && 
                        currentElement.Attributes[AttributeName].Value.Trim()!=String.Empty)
                    {
                        section = new EmailerSection(defaultSection.SmtpServer,
                            defaultSection.MaxNumberOfRecipientsPerEmail,
                            defaultSection.Priority,
                            defaultSection.From,
                            defaultSection.To,
                            defaultSection.CC,
                            defaultSection.Bcc,
                            defaultSection.Subject,
                            defaultSection.Body,
                            defaultSection.Attachments,
                            defaultSection.IsBodyHtml,
                            defaultSection.EnableSSL,
                            defaultSection.Timeout);
                        //Process content elements
                        foreach (XmlNode contentNode in currentNode.ChildNodes)
                        {
                            if (contentNode.NodeType == XmlNodeType.Element && contentNode.LocalName == TagAdd)
                            {
                                XmlElement oContentElement=(XmlElement)contentNode;
                                if (oContentElement.Attributes[AttributeValue] != null && oContentElement.Attributes[AttributeValue].Value.Trim() != string.Empty)
                                {
                                    switch (oContentElement.Attributes[AttributeName].Value)
                                    {
                                        case SmtpServer:
                                            section.SmtpServer = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case MaxNumberOfRecipientsPerEmail:
                                            section.MaxNumberOfRecipientsPerEmail = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case Priority:
                                            section.Priority = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case From:
                                            section.From = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case To:
                                            section.To = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case CC:
                                            section.CC = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case Bcc:
                                            section.Bcc = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case Subject:
                                            section.Subject = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case Body:
                                            section.Body = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case Attachments:
                                            section.Attachments = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case IsBodyHtml:
                                            section.IsBodyHtml= oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case EnableSsl:
                                            section.EnableSSL = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        case TimeOut:
                                            section.Timeout = oContentElement.Attributes[AttributeValue].Value;
                                            break;
                                        default: break; //ignore all the non-match attributes

                                    }
                                }
                                else
                                {
                                    //In order to allow html format text, Body template can be specified in the innertext of the element
                                    if (oContentElement.Attributes[AttributeName].Value == Body && !string.IsNullOrEmpty(oContentElement.InnerText))
                                    {
                                        section.Body = oContentElement.InnerText;
                                    }
                                }
                            }
                        }
                        _emailerSections.Add(currentElement.Attributes[AttributeName].Value, section);

                    }
                }
            }

			#endregion

		}

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    if (_locker != null)
                        _locker.Dispose();
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.


                // Note disposing has been done.
                disposed = true;
            }
        }
        #endregion
    }

	public class EmailerSection
	{
		#region private members
        //all values are string type in order to apply tokenmapper
		private string _body, _smtpServer, _from, _to, _cc, _bcc, _subject,
            _attachments, _priority, _maxNumberOfRecipientsPerEmail,
            _isBodyHtml,_enableSSL,_timeout;
		#endregion

		#region Properties
		public string Body
		{
			get{return _body;}
			set{_body=value;}
		}
		public string SmtpServer
		{
			get{return _smtpServer;}
			set{_smtpServer=value;}
		}
		public string From
		{
			get{return _from;}
			set{_from=value;}
		}
		public string To
		{
			get{return _to;}
			set{_to=value;}
		}
		public string CC
		{
			get{return _cc;}
			set{_cc=value;}
		}
		public string Bcc
		{
			get{return _bcc;}
			set{_bcc=value;}
		}
		public string Subject
		{
			get{return _subject;}
			set{_subject=value;}
		}
		public string Priority
		{
			get{return _priority;}
			set{_priority=value;}
		}
		public string MaxNumberOfRecipientsPerEmail
		{
			get{return _maxNumberOfRecipientsPerEmail;}
			set{_maxNumberOfRecipientsPerEmail=value;}
		}
        public string IsBodyHtml
        {
            get { return _isBodyHtml; }
            set { _isBodyHtml = value; }
        }
        public string EnableSSL
        {
            get { return _enableSSL; }
            set { _enableSSL = value; }
        }
        public string Timeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }
        public string Attachments
		{
			get{return _attachments;}
			set{_attachments=value;}
		}
		#endregion
		public EmailerSection(string smtpServer, string maxNumberOfRecipientsPerEmail, 
            string priority, string from, string to, string cc, string bcc, 
            string subject, string body, string attachments, string isBodyHtml,string enableSSL,string timeout)
		{
			_smtpServer=smtpServer;
			_maxNumberOfRecipientsPerEmail=maxNumberOfRecipientsPerEmail;
			_priority=priority;
			_from=from;
			_to=to;
			_cc=cc;
			_bcc=bcc;
			_subject=subject;
			_body=body;
			_attachments=attachments;
            _isBodyHtml = isBodyHtml;
            _enableSSL = enableSSL;
            _timeout = timeout;
		}
	}
}
