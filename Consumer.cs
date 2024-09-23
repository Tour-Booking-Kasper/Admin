using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

public class AdminConsumer
{
    //Opretter properties, for de nødvenige parametre for RabbitMQ
    public string HostName { get; set; }
    public string DeadLetterExchange { get; set; }
    public string DeadLetterQueue { get; set; }

    // Constructor til at initialisere properties   
    public AdminConsumer(string hostName, string deadLetterExchange, string deadLetterQueue)
    {
        HostName = hostName;
        DeadLetterExchange = deadLetterExchange;
        DeadLetterQueue = deadLetterQueue;
    }

    //Consume metode til at modtage beskeder fra RabbitMQ, deadLetterExchange og deadLetterQueue
    public void Consume()
    {
        var factory = new ConnectionFactory() { HostName = this.HostName };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.ExchangeDeclare(exchange: DeadLetterExchange, type: "direct");
            channel.QueueDeclare(queue: DeadLetterQueue, durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" Admin app is waiting for messages in the dead letter queue...");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                
                Console.WriteLine($" [x] Admin Received invalid message: {message}");
            };

            channel.BasicConsume(queue: DeadLetterQueue, autoAck: true, consumer: consumer);

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }


    //Funktion til at sende en test besked til DeadLetterExchange
    public void SendTestMessage(string message)
    {
        var factory = new ConnectionFactory() { HostName = this.HostName };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: DeadLetterExchange, routingKey: "", body: body);
            Console.WriteLine(" [x] Sent test message to dead letter queue.");
        }
    }

}
