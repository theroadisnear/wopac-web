using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using System.Linq;
using System.Data.SqlClient;
using WebJob2.AzureWebService;
namespace WebJob2
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var webService = new AzureWebService.SampleWebServiceSoapClient();
            string message = webService.NotifyBookDeadline();
            Console.WriteLine(message);
            //WebJob2.SMTP.SendNotification();
        }
    }
}
