using System.Net.Sockets;

public class ClientInfo
{
    public Socket ClientSocket { get; set; }
    public Guid Id { get; set; }

    public ClientInfo(Socket clientSocket)
    {
        ClientSocket = clientSocket;
        ClientSocket.ReceiveBufferSize = 1024*10;
        ClientSocket.SendBufferSize = 1024*10;
        ClientSocket.ReceiveTimeout = 2000;
        ClientSocket.SendTimeout = 2000;
        Id = Guid.NewGuid();
    }


}
