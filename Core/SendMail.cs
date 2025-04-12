using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;
using System.Configuration;
using System.IO;
using static System.Net.WebRequestMethods;

namespace Core
{
    public class SendMail
    {
        public string MailHost { get; set; }                 //指定发送邮件的服务器地址或IP，如smtp.163.com
        public int Port { get; set; }        //指定发送邮件端口 
        public string MailAddress { get; set; }           //发件人邮箱地址
        public string MailDisplayName { get; set; }    //发件人邮箱用户名
        public string MailPassWord { get; set; }        //发件人邮箱密码
        public bool IsEnableSsl { get; set; } = false;     //启用加密方式发送邮件


        public SendMail(SendMailModel sendMailModel)
        {
            this.MailHost = sendMailModel.MailHost;
            this.Port = sendMailModel.Port;
            this.MailAddress = sendMailModel.MailAddress;
            this.MailDisplayName = sendMailModel.MailDisplayName;
            this.MailPassWord = sendMailModel.MailPassWord;
            this.IsEnableSsl = sendMailModel.IsEnableSsl;
        }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="file">附件集合</param>
        public string SendmailFile(string mailTo, string[] files, string Subject, string Body, MailPriority mailPriority = MailPriority.High)
        {
            string res = "";
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            smtpClient.Host = MailHost;
            smtpClient.EnableSsl = IsEnableSsl;
            smtpClient.Port = Port;//指定发送邮件端口 
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(MailAddress, MailPassWord);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;//是否为html格式 
            mailMessage.Priority = mailPriority;//发送邮件的优先等级 
            mailMessage.From = new MailAddress(MailAddress, MailDisplayName);//发件人和显示发件人名称

            string[] MailToList = mailTo.Split(';');
            for (int i = 0; i < MailToList.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(MailToList[i], @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    mailMessage.To.Add(new MailAddress(MailToList[i], MailToList[i].Substring(0, MailToList[i].IndexOf("@"))));  //收件人和收件人显示姓名
                }
            }
            //mailMessage.To.Add(MailTo);//收件人
            mailMessage.Subject = Subject;//邮件主题 
            mailMessage.Attachments.Clear();
            //添加邮件附件，可发送多个文件
            foreach (var filename in files)
            {
                var attachment = new Attachment(filename, MediaTypeNames.Application.Octet);
                attachment.ContentId = Path.GetFileNameWithoutExtension(filename);
                mailMessage.Attachments.Add(attachment);
            }
            mailMessage.Body = Body;//邮件内容

            try
            {
                smtpClient.Send(mailMessage);
                res = "成功";
            }
            catch (Exception ex)
            {
                res = "邮箱异常！" + ex.Message;
                //throw new Exception("邮箱异常！" + ex.Message);
            }
            return res;
        }

        /// <summary>
        /// 发送HTML邮件加附件
        /// </summary>
        /// <param name="mailTo">收件人</param>
        /// <param name="files">附件集合</param>
        public string SendmailFile(string mailTo, List<Attachment> files, string Subject, string Body, MailPriority mailPriority = MailPriority.High)
        {
            string res = "";
            SmtpClient smtpClient = new SmtpClient();
            MailMessage mailMessage = new MailMessage();
            smtpClient.Host = MailHost;
            smtpClient.EnableSsl = IsEnableSsl;
            smtpClient.Port = Port;//指定发送邮件端口 
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new System.Net.NetworkCredential(MailAddress, MailPassWord);
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailMessage.BodyEncoding = Encoding.UTF8;
            mailMessage.IsBodyHtml = true;//是否为html格式 
            mailMessage.Priority = mailPriority;//发送邮件的优先等级 
            mailMessage.From = new MailAddress(MailAddress, MailDisplayName);//发件人和显示发件人名称

            string[] MailToList = mailTo.Split(';');
            for (int i = 0; i < MailToList.Length; i++)
            {
                if (System.Text.RegularExpressions.Regex.IsMatch(MailToList[i], @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
                {
                    mailMessage.To.Add(new MailAddress(MailToList[i], MailToList[i].Substring(0, MailToList[i].IndexOf("@"))));  //收件人和收件人显示姓名
                }
            }
            //mailMessage.To.Add(MailTo);//收件人
            mailMessage.Subject = Subject;//邮件主题 
            mailMessage.Attachments.Clear();
            //添加邮件附件，可发送多个文件
            for (int i = 0; i < files.Count; i++)
            {
                mailMessage.Attachments.Add(files[i]);
            }

            // 将html正文作为AlternateView添加到邮件中
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(Body, null, "text/html");
            mailMessage.AlternateViews.Add(htmlView);

            //mailMessage.Body = Body;//邮件内容

            try
            {
                smtpClient.Send(mailMessage);
                res = "成功";
            }
            catch (Exception ex)
            {
                res = "邮箱异常！" + ex.Message;
                //throw new Exception("邮箱异常！" + ex.Message);
            }
            return res;
        }
    }
}
public class SendMailModel
{
    public string MailHost { get; set; }                 //指定发送邮件的服务器地址或IP，如smtp.163.com
    public int Port { get; set; }        //指定发送邮件端口 
    public string MailAddress { get; set; }           //发件人邮箱地址
    public string MailDisplayName { get; set; }    //发件人邮箱用户名
    public string MailPassWord { get; set; }        //发件人邮箱密码
    public bool IsEnableSsl { get; set; } = false;  //启用加密方式发送邮件
}
