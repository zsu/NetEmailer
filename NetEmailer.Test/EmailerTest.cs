using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using NetEmailer;
using NetEmailer.TokenMapping;
using System.Configuration;
using System.Reflection;
using NetEmailer.Configuration;

namespace NetEmailer.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class EmailerTest:IParameterProvider
    {
        private const string SectionKeyEmailHealthTest="Email Health Test";
        private const string SectionKeyPasswordChangeConfirm = "Password Changed Confirm";
        private string[] _parameters;
        private string _emailerFilePath;
        private IEmailerConfig _emailerConfig;
        public EmailerTest()
        {
            _emailerFilePath = Util.EmailerConfigFilePath;
            _emailerConfig = EmailerConfigManager.GetConfig(_emailerFilePath);
            _parameters= new string[] { "ApplicationAcronym",
            "UserEmail",
            "SupportOrganization",
            "SupportHours",
            "SupportEmail",
            "SupportPhone"
            };
        }

        #region Additional test attributes
        
        // You can use the following additional attributes as you write your tests:
        
        // Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext) 
        //{
        //}
        
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        
        #endregion

        [TestMethod]
        public void BuildMessage_To()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            if (_emailerConfig[SectionKeyEmailHealthTest].To != null && message.To.Address != null)
            {
                string source = _emailerConfig[SectionKeyEmailHealthTest].To;
                EmailAddress emailAddress = new EmailAddress(source);
                Assert.AreEqual(emailAddress.Address, message.To.Address);
            }
            else
                Assert.IsTrue((_emailerConfig[SectionKeyEmailHealthTest].To == null || _emailerConfig[SectionKeyEmailHealthTest].To.Trim() == string.Empty) &&
                    (message.To.Address == null || message.To.Address.Trim() == string.Empty));
        }

        [TestMethod]
        public void BuildMessage_SMTPServer()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            Assert.AreEqual(_emailerConfig[SectionKeyEmailHealthTest].SmtpServer, message.Server);
        }

        [TestMethod]
        public void BuildMessage_From()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            if (_emailerConfig[SectionKeyEmailHealthTest].From != null && message.From.Address != null)
            {
                string source = _emailerConfig[SectionKeyEmailHealthTest].From;
                EmailAddress emailAddress = new EmailAddress(source);
                Assert.AreEqual(emailAddress.Address, message.From.Address);
            }
            else
                Assert.IsTrue((_emailerConfig[SectionKeyEmailHealthTest].From == null || _emailerConfig[SectionKeyEmailHealthTest].From.Trim() == string.Empty) &&
                    (message.From.Address == null || message.From.Address.Trim() == string.Empty));
        }

        [TestMethod]
        public void BuildMessage_Subject()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            string subject = _emailerConfig[SectionKeyEmailHealthTest].Subject;
            subject = SubstitueParameters(subject);
            Assert.AreEqual(subject, message.Subject);
        }

        [TestMethod]
        public void BuildMessage_CC()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            if (_emailerConfig[SectionKeyEmailHealthTest].CC != null && message.CC.Address != null)
            {
                string source = _emailerConfig[SectionKeyEmailHealthTest].CC;
                EmailAddress emailAddress = new EmailAddress(source);
                Assert.AreEqual(emailAddress.Address, message.CC.Address);
            }
            else
                Assert.IsTrue((_emailerConfig[SectionKeyEmailHealthTest].CC == null || _emailerConfig[SectionKeyEmailHealthTest].CC.Trim() == string.Empty) &&
                    (message.CC.Address == null || message.CC.Address.Trim() == string.Empty));
        }

        [TestMethod]
        public void BuildMessage_Bcc()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            if (_emailerConfig[SectionKeyEmailHealthTest].Bcc != null && message.Bcc.Address != null)
            {
                string source = _emailerConfig[SectionKeyEmailHealthTest].Bcc;
                EmailAddress emailAddress = new EmailAddress(source);
                Assert.AreEqual(emailAddress.Address, message.Bcc.Address);
            }
            else
                Assert.IsTrue((_emailerConfig[SectionKeyEmailHealthTest].Bcc == null || _emailerConfig[SectionKeyEmailHealthTest].Bcc.Trim() == string.Empty) &&
                    (message.Bcc.Address == null || message.Bcc.Address.Trim() == string.Empty));
        }

        //[TestMethod]
        //public void BuildMessage_Body()
        //{
        //    EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
        //    Assert.IsTrue(message.Body!=null);
        //}

        [TestMethod]
        public void BuildMessage_Attachment()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            if (_emailerConfig[SectionKeyEmailHealthTest].Attachments != null && message.Attachments.Attachments != null)
            {
                string source = _emailerConfig[SectionKeyEmailHealthTest].Attachments;
                EmailAttachments emailAttachments = new EmailAttachments(source);
                Assert.AreEqual(emailAttachments.Attachments, message.Attachments.Attachments);
            }
            else
                Assert.IsTrue((_emailerConfig[SectionKeyEmailHealthTest].Attachments == null || _emailerConfig[SectionKeyEmailHealthTest].Attachments.Trim() == string.Empty) &&
                    (message.Attachments.Attachments == null || message.Attachments.Attachments.Trim() == string.Empty));
        }

        [TestMethod]
        public void BuildMessage_Priority()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            Assert.AreEqual(_emailerConfig[SectionKeyEmailHealthTest].Priority, message.Priority.ToString());
        }

        [TestMethod]
        public void BuildMessage_IsBodyHtml()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyEmailHealthTest, this);
            Assert.AreEqual(Convert.ToBoolean(_emailerConfig[SectionKeyEmailHealthTest].IsBodyHtml), message.IsBodyHtml);
        }
        [TestMethod]
        public void Send()
        {
            EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyPasswordChangeConfirm, this);
            message.Send();
        }
        public string GetProperty(string key)
        {
            if (key == null || key.Trim() == string.Empty)
                return string.Empty;
            switch (key.Trim())
            {
                case "ApplicationAcronym":
                    return "NetEmailer";
                case "UserEmail":
                    return "user1@yourcompany.com";
                case "SupportOrganization":
                    return "NetEmailer";
                case "SupportHours":
                    return "Mon. - Fri., 0800 - 1700 ET";
                case "SupportEmail":
                    return "support@yourcompany.com";
                case "SupportPhone":
                    return "111-1111-111";
                case "DateTimeFormat":
                    return "MMMM d, yyyy";
            }
            return string.Empty;
        }

        private string SubstitueParameters(string source)
        {
            foreach (string parameter in _parameters)
            {
                source=source.Replace("{="+parameter.Trim()+"}", GetProperty(parameter));
            }
            return source;
        }
    }
    public class Util
    {
        private const string EmailerConfigKey = "EmailerConfig";
        public static string EmailerConfigFilePath
        {
            get { return GetFullPath(ConfigurationManager.AppSettings[EmailerConfigKey]);}
        }
        /// <summary>
        /// Resolve path into absolute physical path.
        /// </summary>
        /// <param name="sPath">Relative path to the webroot or absolute physical path of file</param>
        /// <returns>Absolute Physical full path of the file</returns>
        public static string GetFullPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            string pathRoot = Path.GetPathRoot(path);
            if (pathRoot == string.Empty || pathRoot == "\\")
                if (System.Web.HttpContext.Current != null)
                    return Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~"), path);
                else
                    if (Assembly.GetEntryAssembly() != null)
                        return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), path);
                    else
                        return Path.GetFullPath(path);
            else
                return path;
        }
    }
}
