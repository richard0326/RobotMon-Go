using System.Net;
using System.Net.Mail;

namespace ApiServer.Services
{
    public class EmailManager
    {
        private static string _sendEmail;
        private static string _sendEmailPW;
        
        public static void Init(string sendEmail, string sendEmailPW)
        {
            _sendEmail = sendEmail;
            _sendEmailPW = sendEmailPW;
        }

        public static void SendEmail(string recvEmailAddress, string title, string bodyText, bool isBodyHtml = false)
        {
            // MailMessage 객체 생성
            // 파라미터 : (보내는사람, 받는사람, 주제, 본문) 
            MailMessage msg = new MailMessage(_sendEmail, recvEmailAddress, title, bodyText);

            msg.IsBodyHtml = isBodyHtml;

            // SmtpClient 셋업 (SMTP 서버, 포트)
            SmtpClient smtp = new SmtpClient("smtp.office365.com", 587);
            smtp.EnableSsl = true; // SSL 사용
            
            // 아웃룩, Live 또는 Hotmail의 계정과 암호를 지정
            smtp.Credentials = new NetworkCredential(_sendEmail, _sendEmailPW);
            
            // 메일 발송
            smtp.Send(msg);
        }
    }
}