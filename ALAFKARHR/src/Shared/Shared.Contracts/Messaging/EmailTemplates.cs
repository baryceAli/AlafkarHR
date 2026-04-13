using System.Text.Encodings.Web;

namespace Shared.Contracts.Messaging;

public static class EmailTemplates
{
    public static string GetConfirmEmailTemplate(string OTP)
    {
        //var encodedUrl = HtmlEncoder.Default.Encode(confirmationUrl);
        return $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f6f9fc; padding: 30px; color: #333; }}
                        .container {{ background: #fff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1); max-width: 600px; margin: auto; padding: 30px; }}
                        .header {{ text-align: center; color: #28a745; }}
                        .button {{ display: inline-block; padding: 12px 24px; margin-top: 20px; background-color: #28a745; color: #fff; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                        .footer {{ font-size: 12px; color: #777; text-align: center; margin-top: 30px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2 class='header'>Confirm email OTP</h2>
                        <p class='text-center'><strong>{OTP}</strong></p>
                    </div>
                </body>
                </html>";
    }

    public static string GetResetPasswordTemplate(string OTP)
    {
        return $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <style>
                        body {{ font-family: Arial, sans-serif; background-color: #f6f9fc; padding: 30px; color: #333; }}
                        .container {{ background: #fff; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1); max-width: 600px; margin: auto; padding: 30px; }}
                        .header {{ text-align: center; color: #007bff; }}
                        .button {{ display: inline-block; padding: 12px 24px; margin-top: 20px; background-color: #007bff; color: #fff; text-decoration: none; border-radius: 5px; font-weight: bold; }}
                        .footer {{ font-size: 12px; color: #777; text-align: center; margin-top: 30px; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2 class='header'>Password Reset OTP</h2>
                        <p class='text-center'> <strong>{OTP}</strong>,</p>
                        
                    </div>
                </body>
                </html>";
    }

    public static string EmailConfirmationFailed => $@"
            <html>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Email Confirmation</title>
                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
            </head>
            <body style='font-family:Arial; background-color:#f8f9fa;'>
                <div class='container d-flex justify-content-center align-items-center vh-100'>
                    <div class='card shadow text-center' style='max-width: 500px; width:100%;'>
                        <div class='card-header bg-danger text-white'>
                            <h3>❌ Email Confirmation Failed</h3>
                        </div>
                        <div class='card-body'>
                            <p>Please make sure the link is correct or try again.</p>
                        </div>
                    </div>
                </div>
            </body>
            </html>";

    public static string GetEmailConfirmedHtml(string userName, string resetPasswordURL) => $@"
            <html>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>Email Confirmation</title>
                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css' rel='stylesheet'>
            </head>
            <body style='font-family:Arial;text-align:center; background-color:#f8f9fa; padding:50px'>
                <div class='container d-flex justify-content-center align-items-center vh-100'>
                    <div class='card shadow text-center' style='max-width: 500px; width:100%;'>
                        <div class='card-header'>
                            <h3 class='text-center text-success'>✅ Email Confirmed Successfully!</h3>
                        </div>
                        <div class='card-body'>
                            <p class='text-center'>Thank you for confirming your email, <strong>{userName}</strong>.</p>
                            
                        </div>
                        <div class='card-footer'>
                            <a href='{resetPasswordURL}' class='btn btn-success'>Create your password</a>
                        </div>
                    </div>
                </div>
            </body>
            </html>";
}
