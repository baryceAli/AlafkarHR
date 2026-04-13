using Microsoft.AspNetCore.Builder.Extensions;

namespace Auth.Helpers;

public static class GenerateOTP
{
    

    public static string Generate(int otpLength)
    {
        //Generate OTP 
        int otpRandStart = 1;
        int otpRandEnd = 10;
        for (int i = 0; i < otpLength - 1; i++)
        {
            otpRandStart = otpRandStart * 10;
            otpRandEnd = otpRandEnd * 10;
        }
        var otp = new Random().Next(otpRandStart, otpRandEnd).ToString();
        return otp;
    }
}
