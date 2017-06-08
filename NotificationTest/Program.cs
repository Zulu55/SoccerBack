using Microsoft.Azure.NotificationHubs;
using System;
using System.Collections.Generic;

namespace NotificationTest
{
    class Program
    {
        private static NotificationHubClient hub;

        public static void Main(string[] args)
        {
            hub = NotificationHubClient.CreateClientFromConnectionString("Endpoint=sb://zuluhub2.servicebus.windows.net/;SharedAccessKeyName=DefaultFullSharedAccessSignature;SharedAccessKey=J9t78uCQkWHaOeg7q+FhOYBOeGXd3rdx6xHdTQ012sg=", "Zulu2");

            do
            {
                Console.WriteLine("Type a new message:");
                var message = Console.ReadLine();
                SendNotificationAsync(message);
                Console.WriteLine("The message was sent...");
            } while (true);
        }

        private static async void SendNotificationAsync(string message)
        {
            var tags = new List<string>();
            tags.Add("userId:1");
            tags.Add("userId:2");
            tags.Add("userId:3");
            await hub.SendGcmNativeNotificationAsync("{ \"data\" : {\"Message\":\"" + message + "\"}}", tags);
        }

    }
}
