using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Text;

namespace Pframe.Tools
{
	public class EmailHelper
	{
		public string mailFrom { get; set; }
        
		public string[] mailToArray { get; set; }
        
		public string[] mailCcArray { get; set; }
        
		public string mailSubject { get; set; }
        
		public string mailBody { get; set; }
        
		public string mailPwd { get; set; }
        
		public string host { get; set; }
        
		public bool isbodyHtml { get; set; }
        
		public string[] attachmentsPath { get; set; }
        
		public bool Send()
		{
			MailAddress from = new MailAddress(this.mailFrom);
			MailMessage mailMessage = new MailMessage();
			if (this.mailToArray != null)
			{
				for (int i = 0; i < this.mailToArray.Length; i++)
				{
					mailMessage.To.Add(this.mailToArray[i].ToString());
				}
			}
			if (this.mailCcArray != null)
			{
				for (int j = 0; j < this.mailCcArray.Length; j++)
				{
					mailMessage.CC.Add(this.mailCcArray[j].ToString());
				}
			}
			mailMessage.From = from;
			mailMessage.Subject = this.mailSubject;
			mailMessage.SubjectEncoding = Encoding.UTF8;
			mailMessage.Body = this.mailBody;
			mailMessage.BodyEncoding = Encoding.Default;
			mailMessage.Priority = MailPriority.High;
			mailMessage.IsBodyHtml = this.isbodyHtml;
			try
			{
				if (this.attachmentsPath != null && this.attachmentsPath.Length != 0)
				{
					foreach (string fileName in this.attachmentsPath)
					{
						Attachment item = new Attachment(fileName);
						mailMessage.Attachments.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				string str = "在添加附件时有错误:";
				Exception ex2 = ex;
				throw new Exception(str + ((ex2 != null) ? ex2.ToString() : null));
			}
			SmtpClient smtpClient = new SmtpClient();
			smtpClient.EnableSsl = true;
			smtpClient.Credentials = new NetworkCredential(this.mailFrom, this.mailPwd);
			smtpClient.Host = this.host;
			bool result;
			try
			{
				smtpClient.Send(mailMessage);
				result = true;
			}
			catch
			{
				result = false;
			}
			return result;
		}
        
		public bool Send(out string errs)
		{
			MailAddress from = new MailAddress(this.mailFrom);
			MailMessage mailMessage = new MailMessage();
			if (this.mailToArray != null)
			{
				for (int i = 0; i < this.mailToArray.Length; i++)
				{
					mailMessage.To.Add(this.mailToArray[i].ToString());
				}
			}
			if (this.mailCcArray != null)
			{
				for (int j = 0; j < this.mailCcArray.Length; j++)
				{
					mailMessage.CC.Add(this.mailCcArray[j].ToString());
				}
			}
			mailMessage.From = from;
			mailMessage.Subject = this.mailSubject;
			mailMessage.SubjectEncoding = Encoding.UTF8;
			mailMessage.Body = this.mailBody;
			mailMessage.BodyEncoding = Encoding.Default;
			mailMessage.Priority = MailPriority.High;
			mailMessage.IsBodyHtml = this.isbodyHtml;
			try
			{
				if (this.attachmentsPath != null && this.attachmentsPath.Length != 0)
				{
					foreach (string fileName in this.attachmentsPath)
					{
						Attachment item = new Attachment(fileName);
						mailMessage.Attachments.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				string str = "在添加附件时有错误:";
				Exception ex2 = ex;
				throw new Exception(str + ((ex2 != null) ? ex2.ToString() : null));
			}
			SmtpClient smtpClient = new SmtpClient();
			smtpClient.EnableSsl = true;
			smtpClient.Credentials = new NetworkCredential(this.mailFrom, this.mailPwd);
			smtpClient.Host = this.host;
			bool result;
			try
			{
				smtpClient.Send(mailMessage);
				errs = "";
				result = true;
			}
			catch (SmtpException ex3)
			{
				errs = ex3.Message;
				result = false;
			}
			return result;
		}       
	}
}
