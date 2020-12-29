using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

// https://www.cs.dartmouth.edu/~campbell/cs60/socketprogramming.html#:~:text=There%20are%20some%20fundamental%20differences,%2C%20reliable%20and%20stream%20based).&text=recvfrom%20returns%20the%20IP%20address,a%20response%20to%20the%20client.
namespace tcp_client
{
    class Client
    {
        static void Main(string[] args)
        {
            Client client = new Client();
            int port = 10001;

            // Wait for new messages to be sent.
            while (true)
            {
                Console.WriteLine("Please enter something.");

                // Create new Message object and instantiate its properties.
                Message newMessage = new Message();
                newMessage.Content = Console.ReadLine();
                newMessage.Date = DateTime.Now.ToString("h:mm:ss tt");

                // Connect to the server.
                client.Connect("127.0.0.21", port, newMessage);
            }
        }

        public void Connect (String ipAddress, int port, Message message)
        {
            try
            {
                // Connect to the server.
                TcpClient client = new TcpClient(ipAddress, port);

                // Convert Message object to json string.
                string messageToJson = JsonSerializer.Serialize(message);

                // Convert string message into bytes.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(messageToJson);

                // Network stream.
                NetworkStream stream = client.GetStream();

                // Write the bytes data onto the Network Stream, prepare it to be sent.
                stream.Write(data, 0, data.Length);

                // Send message to the console.
                Console.WriteLine("Sent: {0}", messageToJson);

                // Resetting data
                data = new Byte[256];

                // A place holder to store the response data
                String responseData = String.Empty;

                // Read the response data 
                int bytes = stream.Read(data, 0, data.Length);

                // Convert responded bytes into string, or reading the bytes.
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Response: {0}", responseData);

                // Close / Clear up everything
                stream.Close();
                client.Close();

            }
            // Wait for any SocketException and write out appropriate error message.
            catch (SocketException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
