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
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;

namespace NetEmailer
{
	/// <summary>
	/// Email Information.
	/// </summary>
	public sealed class EmailMessage
	{
        private const int DefaultTimeOut=100000;   //default timeout 100,000 milliseconds(100 seconds)
		private string _body,_server,_subject;
		private int _maxNumberOfRecipientsPerEmail,_timeout;
		private EmailAddress _to,_from,_cc,_bcc;
		private MailPriority _priority;
		private EmailAttachments _attachments;
        private bool _isBodyHtml, _enalbeSSL;

        public event SendCompletedEventHandler SendCompleted; 

        /// <overloads>
        /// + 7 Overloads.
        /// </overloads>
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageText">Message body</param>
        /// <param name="server">SMTP server</param>
        /// <param name="to">To</param>
        /// <param name="from">From</param>
        /// <param name="subject">Subject</param>
        /// <param name="cc">CC</param>
        /// <param name="bcc">BCC</param>
        /// <param name="priority">Priority</param>
        /// <param name="maxNumberOfRecipientsPerEmail">Max number of recipients per email</param>
        /// <param name="attachments">Attachments list</param>
        /// <param name="isBodyHtml">html body flag</param>
        /// <param name="enableSSL">Eanble SSL flag</param>
        /// <param name="iTimeout">Timeout in milliseconds</param>
        public EmailMessage(string messageText, string server, string to, 
            string from, string subject, string cc, string bcc, 
            MailPriority priority, int maxNumberOfRecipientsPerEmail, 
            string attachments,bool isBodyHtml,bool enableSSL, int timeout)
        {

            _body = messageText;
            _server = server;
            _subject = subject;
            _to = new EmailAddress(to);
            _from = new EmailAddress(from);
            _cc = new EmailAddress(cc);
            _bcc = new EmailAddress(bcc);
            _priority = priority;
            _maxNumberOfRecipientsPerEmail = maxNumberOfRecipientsPerEmail;
            _attachments = new EmailAttachments(attachments);
            _isBodyHtml = isBodyHtml;
            _enalbeSSL = enableSSL;
            _timeout = timeout;

        }

        /// <overloads>
        /// + 7 Overloads.
        /// </overloads>
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageText">Message body</param>
        /// <param name="server">SMTP server</param>
        /// <param name="to">To</param>
        /// <param name="from">From</param>
        /// <param name="subject">Subject</param>
        /// <param name="cc">CC</param>
        /// <param name="bcc">BCC</param>
        /// <param name="priority">Priority</param>
        /// <param name="maxNumberOfRecipientsPerEmail">Max number of recipients per email</param>
        /// <param name="attachments">Attachments list</param>
        /// <param name="isBodyHtml">html email flag</param>
        /// <param name="enableSSL">Enable SSL flag</param>
        public EmailMessage(string messageText, string server, string to, 
            string from, string subject, string cc, string bcc, 
            MailPriority priority, int maxNumberOfRecipientsPerEmail, 
            string attachments,bool isBodyHtml, bool enableSSL):
            this(messageText, server, to, from, subject, cc, bcc, priority, maxNumberOfRecipientsPerEmail, attachments, isBodyHtml,enableSSL,DefaultTimeOut)
        {}

        /// <overloads>
        /// + 7 Overloads.
        /// </overloads>
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageText">Message body</param>
        /// <param name="server">SMTP server</param>
        /// <param name="to">To</param>
        /// <param name="from">From</param>
        /// <param name="subject">Subject</param>
        /// <param name="cc">CC</param>
        /// <param name="bcc">BCC</param>
        /// <param name="priority">Priority</param>
        /// <param name="maxNumberOfRecipientsPerEmail">Max number of recipients per email</param>
        /// <param name="attachments">Attachments list</param>
        /// <param name="isBodyHtml">html email flag</param>
        public EmailMessage(string messageText, string server, string to,
            string from, string subject, string cc, string bcc,
            MailPriority priority, int maxNumberOfRecipientsPerEmail,
            string attachments, bool isBodyHtml)
            :this(messageText, server, to, from, subject, cc, bcc, priority, maxNumberOfRecipientsPerEmail, attachments, isBodyHtml, false, DefaultTimeOut)
        { }

        /// <overloads>
        /// + 7 Overloads.
        /// </overloads>
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="messageText">Message body</param>
        /// <param name="server">SMTP server</param>
        /// <param name="to">To</param>
        /// <param name="from">From</param>
        /// <param name="subject">Subject</param>
        /// <param name="cc">CC</param>
        /// <param name="bcc">BCC</param>
        /// <param name="priority">Priority</param>
        /// <param name="maxNumberOfRecipientsPerEmail">Max number of recipients per Email</param>
        /// <param name="attachments">Attachments list</param>
        public EmailMessage(string messageText, string server, string to,
            string from, string subject, string cc, string bcc,
            MailPriority priority, int maxNumberOfRecipientsPerEmail,
            string attachments)
            : this(messageText, server, to, from, subject, cc, bcc, priority, maxNumberOfRecipientsPerEmail, attachments, false, false, DefaultTimeOut)
        { }

		/// <overloads>
		/// + 7 Overloads.
		/// </overloads>
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="messageText">Message body</param>
		/// <param name="server">SMTP server</param>
		/// <param name="to">To</param>
		/// <param name="from">From</param>
		/// <param name="subject">Subject</param>
		/// <param name="cc">CC</param>
		/// <param name="bcc">BCC</param>
		/// <param name="priority">Priority</param>
		public EmailMessage(string messageText,string server, string to, string from, string subject, string cc, string bcc, MailPriority priority):
			this(messageText,server,to,from,subject,cc,bcc,priority,int.MaxValue,String.Empty,false,false,DefaultTimeOut)
		{

		}

		/// <overloads>
		/// + 7 Overloads.
		/// </overloads>
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="messageText">Message body</param>
		/// <param name="server">SMTP server</param>
		/// <param name="to">To</param>
		/// <param name="from">From</param>
		/// <param name="subject">Subject</param>
		public EmailMessage(string messageText,string server, string to, string from, string subject):
            this(messageText, server, to, from, subject, String.Empty, String.Empty, MailPriority.Normal, int.MaxValue, String.Empty, false, false, DefaultTimeOut)
		{
		}

		/// <overloads>
		/// + 7 Overloads.
		/// </overloads>
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="messageText">Message Body</param>
		/// <param name="server">SMTP Server</param>
		/// <param name="to">To</param>
		/// <param name="from">From</param>
		/// <param name="subject">Subject</param>
		/// <param name="priority">Priority</param>
		public EmailMessage(string messageText,string server, string to, string from, string subject,MailPriority priority):
            this(messageText, server, to, from, subject, String.Empty, String.Empty, priority, int.MaxValue, String.Empty, false, false, DefaultTimeOut)
		{
		}
        /// <summary>
        /// Smtp server
        /// </summary>
		public string Server
		{
			get
			{
				return _server;
			}
			set
			{
				_server=value;
			}
		}
        /// <summary>
        /// Maximum number of recipients per email. 
        /// Recipients email addresses will be automatically splitted into small groups if the total number exceed the limit.
        /// </summary>
		public int MaxNumberOfRecipientsPerEmail
		{
			get
			{
				return _maxNumberOfRecipientsPerEmail;
			}
			set
			{
				_maxNumberOfRecipientsPerEmail=value;
			}
		}
        // Summary:
        //     Gets or sets a value that specifies the amount of time after which a synchronous
        //     Overload:System.Net.Mail.SmtpClient.Send call times out.
        //
        // Returns:
        //     An System.Int32 that specifies the time-out value in milliseconds. The default
        //     value is 100,000 (100 seconds).
        //
        // Exceptions:
        //   System.ArgumentOutOfRangeException:
        //     The value specified for a set operation was less than zero.
        //
        //   System.InvalidOperationException:
        //     You cannot change the value of this property when an email is being sent.
        public int TimeOut
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }
        // Summary:
        //     Gets or sets a value indicating whether the mail message body is in Html.
        //
        // Returns:
        //     true if the message body is in Html; else false. The default is false.
        public bool IsBodyHtml
        {
            get
            {
                return _isBodyHtml;
            }
            set
            {
                _isBodyHtml = value;
            }
        }
        //
        // Summary:
        //     Specify whether Smtp server uses Secure Sockets Layer
        //     (SSL) to encrypt the connection.
        //
        // Returns:
        //     true if the Smtp server uses SSL; otherwise, false. The default
        //     is false.
        public bool EnableSsl
        {
            get
            {
                return _enalbeSSL;
            }
            set
            {
                _enalbeSSL = value;
            }
        }
        // Summary:
        //     Gets the address collection that contains the recipients of this e-mail message.
        public EmailAddress To
		{
			get
			{
				return _to;
			}
		}
        // Summary:
        //     Gets the address collection that contains the carbon copy (CC) recipients
        //     for this e-mail message.
		public EmailAddress CC
		{
			get
			{
				return _cc;
			}
		}
        // Summary:
        //     Gets the address collection that contains the blind carbon copy (BCC) recipients
        //     for this e-mail message.
		public EmailAddress Bcc
		{
			get
			{
				return _bcc;
			}
		}
        // Summary:
        //     Gets the from address for this e-mail message.
		public EmailAddress From
		{
			get
			{
				return _from;
			}
		}
        // Summary:
        //     Gets or sets the subject line for this e-mail message.
        //
        // Returns:
        //     A System.String that contains the subject content.
		public string Subject
		{
			get
			{
				return _subject;

			}
			set
			{
				_subject=value;
			}
		}
        // Summary:
        //     Gets or sets the priority of this e-mail message.
        //
        // Returns:
        //     A System.Net.Mail.MailPriority that contains the priority of this message.
		public MailPriority Priority
		{
			get
			{
				return _priority;
			}
			set
			{
				_priority=value;
			}
		}
        // Summary:
        //     Gets or sets the message body.
        //
        // Returns:
        //     A System.String value that contains the body text.
		public string Body
		{
			get
			{
				return _body;
			}
			set
			{
				_body=value;
			}
		}
        // Summary:
        //     Gets the attachment collection used to store data attached to this e-mail
        //     message.
        //
        // Returns:
        //     A writable EmailAttachments.
		public EmailAttachments Attachments
		{
			get
			{
				return _attachments;
			}
			set
			{
				_attachments=value;
			}
		}

		/// <summary>
		/// Send out the email.
		/// </summary>
		public void Send()
		{
            SendEmail(false, null);
		}

        /// <summary>
        /// Send out the email asynchronously.
        /// </summary>
        public void SendAsync(object token)
        {
            SendEmail(true, token);
        }
        private void SendEmail(bool async,object token)
        {
            string delimiters = ";,";
            char[] delimiter = delimiters.ToCharArray();

            ArrayList arrayList;
            int count = 0;
            string[] toTemp = { }, ccTemp = { }, bccTemp = { }, attachmentsTemp = { };
            ArrayList toList = new ArrayList(), ccList = new ArrayList(), bccList = new ArrayList();
            //			if(_oTo.Address==null || _oTo.Address.Trim()==String.Empty)
            //				throw new System.ArgumentNullException("Email receiver's address can not be empty.");
            if (_from.Address == null || _from.Address.Trim() == String.Empty)
                throw new System.ArgumentNullException("Email sender's address can not be empty.");
            if (_subject == null || _subject.Trim() == String.Empty)
                throw new System.ArgumentNullException("Email subject can not be empty.");
            //			if(_sServer==null || _sServer.Trim()==String.Empty)
            //				throw new System.ArgumentNullException("Email server's address can not be empty.");
            using (MailMessage email = new MailMessage())
            {
                email.From = new MailAddress(_from.Address);
                email.Priority = _priority;
                email.Subject = _subject;
                email.Body = _body;
                email.IsBodyHtml = _isBodyHtml;
                #region Add Attachments
                if (_attachments.Attachments.Trim() != String.Empty)
                    attachmentsTemp = _attachments.Attachments.Split(delimiter);
                foreach (string sFileName in attachmentsTemp)
                {
                    email.Attachments.Add(new Attachment(sFileName));
                }
                #endregion
                SmtpClient smtpClient = new SmtpClient();
                if (!string.IsNullOrEmpty(_server))
                    smtpClient.Host = _server;
                smtpClient.EnableSsl = _enalbeSSL;
                smtpClient.Timeout = _timeout;

                #region Split the recipients' list
                if (_to.Address.Trim() != String.Empty)
                    toTemp = _to.Address.Split(delimiter);
                if (_cc.Address.Trim() != String.Empty)
                    ccTemp = _cc.Address.Split(delimiter);
                if (_bcc.Address.Trim() != String.Empty)
                    bccTemp = _bcc.Address.Split(delimiter);

                #region To
                arrayList = new ArrayList();
                count = 0;
                for (int i = 0; i < toTemp.Length; i++)
                {
                    if (count < this._maxNumberOfRecipientsPerEmail)
                    {
                        arrayList.Add(toTemp[i]);
                        count++;
                    }
                    else
                    {
                        toList.Add(arrayList);
                        count = 0;
                        arrayList = new ArrayList();
                        arrayList.Add(toTemp[i]);
                        count++;
                    }
                }
                if (arrayList.Count != 0)
                    toList.Add(arrayList);

                #endregion
                #region CC
                arrayList = new ArrayList();
                count = 0;
                for (int i = 0; i < ccTemp.Length; i++)
                {
                    if (count < this._maxNumberOfRecipientsPerEmail)
                    {
                        arrayList.Add(ccTemp[i]);
                        count++;
                    }
                    else
                    {
                        ccList.Add(arrayList);
                        count = 0;
                        arrayList = new ArrayList();
                        arrayList.Add(ccTemp[i]);
                        count++;
                    }
                }
                if (arrayList.Count != 0)
                    ccList.Add(arrayList);

                #endregion
                #region BCC
                arrayList = new ArrayList();
                count = 0;
                for (int i = 0; i < bccTemp.Length; i++)
                {
                    if (count < this._maxNumberOfRecipientsPerEmail)
                    {
                        arrayList.Add(bccTemp[i]);
                        count++;
                    }
                    else
                    {
                        bccList.Add(arrayList);
                        count = 0;
                        arrayList = new ArrayList();
                        arrayList.Add(bccTemp[i]);
                        count++;
                    }
                }
                if (arrayList.Count != 0)
                    bccList.Add(arrayList);

                #endregion
                #endregion Split the recipients' list
                if (async)
                    smtpClient.SendCompleted += new SendCompletedEventHandler(OnSendCompleted);

                if (toList.Count <= 1 && ccList.Count <= 1 && bccList.Count <= 1)
                {
                    for (int i = 0; i < toList.Count; i++)
                    {
                        arrayList = toList[i] as ArrayList;
                        for (int j = 0; j < arrayList.Count; j++)
                            email.To.Add(arrayList[j] as string);
                    }
                    for (int i = 0; i < bccList.Count; i++)
                    {
                        arrayList = bccList[i] as ArrayList;
                        for (int j = 0; j < arrayList.Count; j++)
                            email.Bcc.Add(arrayList[j] as string);
                    }
                    for (int i = 0; i < ccList.Count; i++)
                    {
                        arrayList = ccList[i] as ArrayList;
                        for (int j = 0; j < arrayList.Count; j++)
                            email.CC.Add(arrayList[j] as string);
                    }
                    if (async)
                        smtpClient.SendAsync(email, token);
                    else
                        smtpClient.Send(email);
                }
                else
                {
                    for (int i = 0; i < toList.Count; i++)
                    {
                        arrayList = toList[i] as ArrayList;
                        email.To.Clear();
                        for (int j = 0; j < arrayList.Count; j++)
                            email.To.Add(arrayList[j] as string);
                        if (async)
                            smtpClient.SendAsync(email, token);
                        else
                            smtpClient.Send(email);
                    }
                    for (int i = 0; i < ccList.Count; i++)
                    {
                        arrayList = ccList[i] as ArrayList;
                        email.To.Clear();
                        email.CC.Clear();
                        email.Headers.Clear();
                        for (int j = 0; j < arrayList.Count; j++)
                            email.CC.Add(arrayList[j] as string);
                        if (async)
                            smtpClient.SendAsync(email, token);
                        else
                            smtpClient.Send(email);
                    }
                    for (int i = 0; i < bccList.Count; i++)
                    {
                        arrayList = bccList[i] as ArrayList;
                        email.To.Clear();
                        email.CC.Clear();
                        email.Bcc.Clear();
                        email.Headers.Clear();
                        for (int j = 0; j < arrayList.Count; j++)
                            email.Bcc.Add(arrayList[j] as string);
                        if (async)
                            smtpClient.SendAsync(email, token);
                        else
                            smtpClient.Send(email);
                    }
                }
            }

        }
        private void OnSendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if(SendCompleted!=null)
                SendCompleted(sender, e);
        }
	}// END CLASS DEFINITION EmailMessage

	/// <summary>
	/// Email attachments
	/// </summary>
	public sealed class EmailAttachments
	{
		StringBuilder _emailAttachments=new StringBuilder();
		/// <summary>
		/// Constructor
		/// </summary>
        /// <param name="emailAttachments">Email attachments seperated by comma or semicolon</param>
		public EmailAttachments(string emailAttachments)
		{
            Add(emailAttachments);
		}

		/// <summary>
        /// Add email attachments seperated by comma or semicolon to the end of the list.
		/// </summary>
        /// <param name="sEmailAttachmentsString">Email attachment seperated by comma or semicolon</param>
        /// <returns>Email attachments seperated by semicolon</returns>
		public string Add(string emailAttachments)
		{
            if (emailAttachments != null && emailAttachments.Trim() != String.Empty)
            {
                string[] attachments = emailAttachments.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string attachment in attachments)
                    _emailAttachments.AppendFormat("{0};", attachment);
            }
            string attachmentlist = _emailAttachments.ToString();
            if (attachmentlist != null && attachmentlist.EndsWith(";"))
                attachmentlist = attachmentlist.Remove(attachmentlist.Length - 1, 1);
            return attachmentlist;
		}

		/// <summary>
		/// Clear the Email attachment list
		/// </summary>
		public void Clear()
		{
			_emailAttachments=new StringBuilder();
		}

		/// <summary>
        /// Emaill attachments seperated by semicolon
		/// </summary>
		public string Attachments
		{
            get
            {
                string attachmentlist = _emailAttachments.ToString();
                if (attachmentlist != null && attachmentlist.EndsWith(";"))
                    attachmentlist = attachmentlist.Remove(attachmentlist.Length - 1, 1);
                return attachmentlist;
            }
		}

	}// END CLASS DEFINITION EmailAttachments

}
