#NetEmailer

 Template based email creation with dynamic contents

#What is NetEmailer
NetEmailer is a .net email library to provide xml template based email creation with dynamic contents. 
Every aspect of an email is configurable in the template files and dynamic contents can be injected during runtime. 
Change to the template file can be made anytime without restarting the running application.

#Background

Sending emails like pass word change confirmation is a common task for applications. 
Quite often we need to send out emails that have almost the same contents with just a few things differed, for example, user name or the dynamically generated password. 
The purpose of this library is to provide a flexible way to fullfill this job across different projects.

#Using the code

Let's dive into the code. In the class that you want to send an email, implements the IParameterProvider interface. There is only one method in this interface: GetProperty, which is used to provide dynamic information to substitue the codeblock(the syntax is {=key}) in the template file(we will come to the detail of this later).
Now you only need two lines of codes to create the email and send it out. Emailer.BuildMessage is a static function that take the template file full path, the email section key( Different emails are distinguished by key in the template file) and the class that implements the IParameterProvider as inputs and return an EmailMessage object which contains all the information of the specific email and has the codebloacks substituted with the dynamic information. Then you just call the Send() or SendAsyn() function on the EmailMessage object.
~~~xml
        public class EmailerTest:IParameterProvider
        {

               ...

               public string GetProperty(string key)                 {

                       if (key == null || key.Trim() == string.Empty)

                               return string.Empty;

                       switch (key.Trim())

                               {

                                      case "ApplicationAcronym":

                                              return "NetEmailer";

                                      case "UserEmail":

                                              return "user1@yourcompany.com";

                                      case "SupportOrganization": return "NetEmailer";

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

               public void Send()

               {

                       EmailMessage message = Emailer.BuildMessage(_emailerFilePath, SectionKeyPasswordChangeConfirm, this);

                       message.Send();

               }

        }
~~~

#Configuration file

All email templates are defined in a single xml configuration file. The following is the Emailer.config included in the attached unit test project.
~~~xml
<?xml version="1.0" encoding="utf-8" ?>

<configuration>

  <!--CodeBlock can be used in the value string. The syntax is as follows:

{=VariableName}

There are some predefined CodeBlocks:

{=Newline}: NewLine character;

{=Tab}: Tab;

{=WhiteSpace}: White Space

{=DateTime}: Current DateTime; The format can be provided by the IParameterProvider through the property 'DateTimeFormat'

{=DoubleQuote}: Double Quote

{=SingleQuote}: Single Quote

The value string format will be kept so you can choose whether use the predefined formatting CodeBlock in the string or just format the string in the configuration file.-->

  <emailer smtpServer="smtp.yourcompany.com" maxNumberOfRecipientsPerEmail="20" from="no-reply-netemailer@yourcompany.com" isBodyHtml="false" enableSSL="false" timeout="100000">

    <!-- Default setting will be overrite by specific setting belows if it's not empty string in the specific section -->

    <!--Maximum number of recipients in each recipient list(to, cc, bcc), Emailer will split them into several emails if the list length exceed this number -->

    <!--Timeout value is in milliseconds-->

    <!--priority value can be "High","Low" or "Normal"-->

    <!--{=VariableName} can be used in any value string except for dateTimeFormat-->

    <!--if the smtpServer authentication required SSL,set enableSSL="true"-->

    <email name="Email Health Test">

      <add name="to" value="user1@yourcompany.com;user2@yourcompany.com" />

      <add name="cc" value="cc@yourcompany.com" />

      <add name="bcc" value="bcc@yourcompany.com" />

      <add name="priority" value="Normal" />

      <add name="subject" value="{=ApplicationAcronym} Email Health Test" />

      <add name="isBodyHtml" value="false" />

      <add name="attachments" value="file1.txt;file2.txt" />

      <!--To send Html email, set 'isBodyHtml' to true; use <![CDATA[]]> to wrap the html-->

      <add name="body" value="This test message was sent {=DateTime} from {=ApplicationAcronym} system.">

      </add>

    </email>

    <email name="Password Changed Confirm">

      <add name="to" value="user1@yourcompany.com" />

      <add name="from" value="" />

      <add name="cc" value="" />

      <add name="bcc" value="" />

      <add name="priority" value="Normal" />

      <add name="subject" value="{=ApplicationAcronym} Password Changed" />

      <add name="body" value="Your password for {=ApplicationAcronym} was changed {=DateTime}.

 

If you have any questions regarding the {=ApplicationAcronym} password change process, please do not hesitate to contact {=SupportOrganization}.

 

If you did not attempt to change the password for {=ApplicationAcronym} web site recently, please contact {=SupportOrganization}.

 

Please do not reply to this auto-generated message.

 

Respectfully,

{=SupportOrganization}

Customer Support Hotline

Hours: {=SupportHours}

Email: {=SupportEmail}

Phone: {=SupportPhone}" />

    </email>

  </emailer>

</configuration>
~~~
 Each \<email\> must have a unique name, and this is also the second parameter for the Emailer.BuildMessage function. Inside each email element, the followings can be configured by providing the name and value: smtpServer; maxNumberOfRecipientsPerEmail; isBodyHtml; enableSSL; timeout; to; from; cc; bcc; priority; subject;body; attachment. You can also provide a default value for any of these as the attribute of <Emailer> element and only provide neccessary overrides in the specific <email> element.

To send Html email, set "isBodyHtml" to true; use \<![CDATA[]]\> to wrap the html content.

"maxNumberOfRecipientsPerEmail" is the threshold that used to split the recipients list into small group, this might helpful when the smtp server cannot take a long list of recipeints.

CodeBlocks are placeholder for dynamic information. The syntax of CodeBlock is {=key}. When calling the Emailer.BuildMessage function, dynamic information will be provided by the IParameterProvider object and the CodeBlocks will be substitued with the values. There are some pre-defined CodeBlocks:
{=Newline}: NewLine character;
{=Tab}: Tab;
{=WhiteSpace}: White Space
{=DateTime}: Current DateTime; The format can be provided by the IParameterProvider through the property 'DateTimeFormat'
{=DoubleQuote}: Double Quote
{=SingleQuote}: Single Quote

#Summary

With one configuration file and two lines of codes, you have all the power and flexibility to create all the emails. And don't forget the configuration file( Email templates) can be change on the fly. Enjoy coding!
