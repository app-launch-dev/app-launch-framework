using Amazon;
using Amazon.Runtime;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AppLaunch.Services;

public class AwsSesEmailService(ISettingsService settingsService) : IEmailSender
{
    
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        try
        {
            var settings = await settingsService.GetSettings();
            var accessKey = settings.Data.AwsSesAccessKey;
            var secretKey = settings.Data.AwsSesSecretKey;
            
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey)) 
                throw new Exception("AWS SES access key and secret key are required to be configured in global settings");
            
            var awsCredentials = new BasicAWSCredentials(accessKey, secretKey);
            var _client = new AmazonSimpleEmailServiceClient(awsCredentials, RegionEndpoint.USEast1);
            var sendRequest = new SendEmailRequest
            {
                Source = settings.Data.DefaultEmailFrom,
                Destination = new Destination { ToAddresses = new List<string> { email } },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body { Html = new Content(htmlMessage) }
                }
            };

            SendEmailResponse response = await _client.SendEmailAsync(sendRequest);

            // Check if the response contains a valid message ID
            if (!string.IsNullOrEmpty(response.MessageId))
            {
                Console.WriteLine($"Email sent successfully! MessageId: {response.MessageId}");
            }
            else
            {
                Console.WriteLine("Failed to send email: No MessageId returned.");
            }
        }
        catch (AmazonSimpleEmailServiceException ex)
        {
            Console.WriteLine($"SES Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Error: {ex.Message}");
        }
    }
}