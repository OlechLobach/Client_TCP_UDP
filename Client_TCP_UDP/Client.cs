using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Client
{
    static async Task Main(string[] args)
    {
        string ip = "192.168.0.103"; // Ваша IP-адреса
        int port = 11000;

        try
        {
            TcpClient client = new TcpClient();
            await client.ConnectAsync(ip, port);
            NetworkStream stream = client.GetStream();

            Console.Write("Enter username: ");
            string username = Console.ReadLine();
            Console.Write("Enter password: ");
            string password = Console.ReadLine();
            string loginDetails = $"{username}:{password}";
            await SendMessageAsync(stream, loginDetails);

            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine(response);
            if (response.Contains("Invalid"))
            {
                return;
            }

            while (true)
            {
                Console.WriteLine("Enter currency pair (e.g., USD EUR) or 'exit' to quit:");
                string message = Console.ReadLine();
                if (message.ToLower() == "exit") break;

                await SendMessageAsync(stream, message);
                bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Server response: {response}");
            }

            client.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Exception: {e.Message}");
        }
    }

    private static async Task SendMessageAsync(NetworkStream stream, string message)
    {
        byte[] msg = Encoding.UTF8.GetBytes(message);
        await stream.WriteAsync(msg, 0, msg.Length);
    }
}