using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;

// https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?view=net-5.0
namespace tcp_server
{
    class Server
    {
        static void Main(string[] args)
        {
            Server foo = new Server();
            foo.StartServer();
        }

        public void StartServer()
        {
            TcpListener server = null;

            try
            {
                // Config
                int port = 10001;
                IPAddress ipAddress = IPAddress.Parse("127.0.0.1");

                // Start Listener / Start Server
                server = new TcpListener(ipAddress, port);
                server.Start();

                // Data to be sent.
                Byte[] bytes = new Byte[256];

                // The actual data that will be converted into bytes.
                String data = null;

                // The server will keep listening for any TCP messages with the specified port.
                while (true)
                {
                    // Send message to the console.
                    Console.Write("Waiting for a new message");
                    
                    // Blocking the call to accept the messages.
                    TcpClient client = server.AcceptTcpClient();

                    // Send message to the console.
                    Console.WriteLine("Message Received. Proceeding...");

                    // Clear previous data / reset.
                    data = null;

                    // Get the stream data.
                    NetworkStream stream = client.GetStream();

                    // Get the data.
                    int numberOfBytes;

                    // Keep reading the data bytes
                    while ((numberOfBytes = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        // Convert each individual byte into a string.
                        data = System.Text.Encoding.ASCII.GetString(bytes, 0, numberOfBytes);

                        // Convert JSON string to Message object.
                        Message newMessage = JsonSerializer.Deserialize<Message>(data);

                        // The message to be put out to the console.
                        string output = "Message: " + newMessage.Content + ", Time: " + newMessage.Date;

                        // Send message to the console.
                        Console.WriteLine("Received: {0}", output);

                        // Convert the modified data to bytes, preparing to send it back.
                        byte[] response = System.Text.Encoding.ASCII.GetBytes(output);

                        // Send the response back.
                        stream.Write(response, 0, response.Length);

                        // Send message to the console.
                        Console.WriteLine("Sent: {0}", output);
                    }

                    client.Close();
                }
            }
            // Wait for any SocketException and write out appropriate error message.
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
            // Always close down the server.
            finally
            {
                server.Stop();
            }

            Console.Read();
        }
    }
}
