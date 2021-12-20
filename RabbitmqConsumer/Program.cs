using Entity;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Net.Mail;
using System.Text;

namespace RabbitmqConsumer
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest",
            };

            using (IConnection connection = factory.CreateConnection())
            using (IModel channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "EmailQueue",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: true,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, mq) =>
                {
                    var body = mq.Body.ToArray();
                    var jsonString = Encoding.UTF8.GetString(body);
                    EmailModel mailContent = JsonConvert.DeserializeObject<EmailModel>(jsonString);

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp-mail.outlook.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.Credentials = new System.Net.NetworkCredential("rabbitmqdeneme@outlook.com", "sifre");

                    MailMessage eposta = new MailMessage();
                    eposta.From = new MailAddress("rabbitmqdeneme@outlook.com", "rabbit");
                    eposta.To.Add(mailContent.Email);
                    eposta.Subject = mailContent.Header;
                    eposta.Body = mailContent.Body;

                    try
                    {
                        smtp.Send(eposta);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Mail gönderilirken bir hata ile karşılaşıldı: " + ex.Message);
                    }
                    Console.WriteLine("Mail gönderildi!");
                };

                channel.BasicConsume(queue: "EmailQueue",
                                     autoAck: true,
                                     consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}
