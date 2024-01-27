using Microsoft.Extensions.Options;
using ServicesApp.API.Helpers;
using System.Diagnostics;
using Twilio;
using Twilio.Clients;
using Twilio.Rest.Api.V2010.Account;

namespace ServicesApp.Infrastructure.Persistence
{
    public class TwilioSmsService : ISmsService
    {
        private readonly TwilioSettings _twilioSettings;
        public TwilioSmsService(IOptions<TwilioSettings> twilioSettings)
        {
            _twilioSettings = twilioSettings.Value;
            TwilioClient.Init(_twilioSettings.AccountSID, _twilioSettings.AuthToken);
        }


        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var twilio = new TwilioRestClient(
                    _twilioSettings.AccountSID,
                    _twilioSettings.AuthToken
                );

                //var verificationCheck = VerificationCheckResource.Create(
                //       to: phoneNumber,
                //       code: message,
                //       pathServiceSid: "VA164aa874e2c50c805ef1cb36c296602d"
                //   );
                //if(verificationCheck.Status.Equals(""))
                var result = MessageResource.Create(
                       body: message,
                       from: new Twilio.Types.PhoneNumber(_twilioSettings.PhoneNumber),
                       to: new Twilio.Types.PhoneNumber(phoneNumber)
                   );
                Debug.WriteLine(result.Status.ToString());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // Log or handle the exception as needed
                return false;
            }
        }
    }
}
