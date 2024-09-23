class Program
{
    static void Main(string[] args)
    {
        var adminConsumer = new AdminConsumer(
            hostName: "localhost",
            deadLetterExchange: "deadLetterExchange",
            deadLetterQueue: "deadLetterQueue"
        );

        adminConsumer.SendTestMessage("Test message from Admin");
        adminConsumer.Consume();
    }
}
