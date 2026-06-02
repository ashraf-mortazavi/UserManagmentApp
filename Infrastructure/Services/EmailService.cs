using ManageUsers.Application.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace ManageUsers.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;

    public EmailService(
        IEmailSender emailSender,
        IConfiguration configuration)
    {
        _emailSender = emailSender;
        _configuration = configuration;
    }

    public async Task<bool> SendPasswordResetEmailAsync(string email, string userName, string resetLink)
    {
        try
        {
            var appName = _configuration["AppSettings:AppName"] ?? "اپلیکیشن";
            var subject = $"بازیابی رمز عبور - {appName}";

            var htmlMessage = GeneratePasswordResetHtml(email, userName, resetLink, appName);

            await _emailSender.SendEmailAsync(email, subject, htmlMessage);

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private string GeneratePasswordResetHtml(string email, string userName, string resetLink, string appName)
    {
        return $@"
        <!DOCTYPE html>
        <html dir='rtl' lang='fa'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <style>
                body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 0; }}
                .container {{ max-width: 600px; margin: 0 auto; padding: 0; background-color: #ffffff; }}
                .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: -40px 20px; text-align: center; }}
                .content {{ padding: 40px 30px; }}
                .button {{ display: inline-block; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 14px 32px; text-decoration: none; border-radius: 8px; font-weight: bold; font-size: 16px; margin: 20px 0; box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4); }}
                .footer {{ padding: 20px; background-color: #f8f9fa; text-align: center; font-size: 12px; color: #6c757d; border-top: 1px solid #e9ecef; }}
                .warning {{ background-color: #fff3cd; border: 1px solid #ffeaa7; padding: 15px; border-radius: 8px; margin: 25px 0; color: #856404; }}
                .link-box {{ background-color: #f8f9fa; padding: 15px; border-radius: 8px; margin: 20px 0; word-break: break-all; font-size: 14px; }}
            </style>
        </head>
        <body>
            <div class='container'>
                <div class='header'>
                    <h1 style='margin: 0; padding: 30px 0;'>بازیابی رمز عبور</h1>
                </div>
                <div class='content'>
                    <p>سلام <strong>{userName}</strong> عزیز،</p>
                    <p>درخواست بازیابی رمز عبور برای حساب کاربری شما در <strong>{appName}</strong> ثبت شده است.</p>
                    
                    <div style='text-align: center;'>
                        <a href='{resetLink}' class='button'>بازیابی رمز عبور</a>
                    </div>
                    
                    <div class='warning'>
                        <p><strong>⚠️ توجه مهم:</strong></p>
                        <p>• این لینک فقط به مدت <strong>24 ساعت</strong> معتبر است</p>
                        <p>• اگر شما این درخواست را ندادید، این ایمیل را نادیده بگیرید</p>
                        <p>• برای امنیت بیشتر، پس از ورود به حساب، رمز عبور خود را تغییر دهید</p>
                    </div>
                    
                    <p>اگر دکمه بالا کار نمی‌کند، لینک زیر را در مرورگر خود کپی کنید:</p>
                    <div class='link-box'>
                        {resetLink}
                    </div>
                    
                    <p>با تشکر،<br>تیم پشتیبانی {appName}</p>
                </div>
                <div class='footer'>
                    <p>این ایمیل به صورت خودکار ارسال شده است. لطفاً به آن پاسخ ندهید.</p>
                    <p>© {DateTime.Now.Year} {appName}. تمامی حقوق محفوظ است.</p>
                </div>
            </div>
        </body>
        </html>";
    }

    public async Task<bool> SendEmailAsync(string email, string subject, string htmlContent)
    {
        try
        {
            await _emailSender.SendEmailAsync(email, subject, htmlContent);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}

   
