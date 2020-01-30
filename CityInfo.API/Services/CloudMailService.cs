using System;
using System.Diagnostics;

namespace CityInfo.API.Services
{
    public class CloudMailService: IMailService
    {
        string _mailTo = Startup.Configuration["mailSettings:mailTo"];
        string _mailFrom = Startup.Configuration["mailSettings:mailFrom"];

        public void Send(string subject, string message)
        {
            Debug.WriteLine($"Mail from {_mailFrom} to {_mailTo}, with CloudMailService");
            Debug.WriteLine($"Subject is {subject}");
            Debug.WriteLine($"Message is {message}");
        }
    }
}
