using System.Net;
using System.Net.Sockets;
namespace HTTP;

public class Program
{
  private static async Task MultipleRequest(Socket socket)
  {
    using var networkStream = new NetworkStream(socket);
    using var streamReader = new StreamReader(networkStream);
    using var streamWriter = new StreamWriter(networkStream);
    
    try
    {
      var firstLine = (await streamReader.ReadLineAsync())?.Split(" ");

      if (firstLine == null)
      {
        return;
      }

      var (method, url) = (firstLine[0], firstLine[1]);
      var line = await streamReader.ReadLineAsync();
      var content = new Content<string>();
      content.Page(method, url, streamWriter);
      
      await SearchRequest(line, streamReader);
      streamWriter.Flush();
    }

    finally
    {
      socket.Close();
      networkStream.Close();
      streamReader.Close();
      streamWriter.Close();
    }
  }

  private static async Task SearchRequest(string? line, StreamReader streamReader)
  {
    var contentLength = 0;
    
    while (!string.IsNullOrEmpty(line) && !streamReader.EndOfStream)
    {
      var pieces = line.Split(":");
      var (header, value) = (pieces[0], pieces[1]);

      if (header.ToLower() == "content-length")
      {
        contentLength = int.Parse(value);
      }

      line = await streamReader.ReadLineAsync();
    }

    if (contentLength > 0)
    {
      var bytes = new char[contentLength];
      await streamReader.ReadAsync(bytes, 0, contentLength);
    }
  }
  
  private static async Task Main()
  {
    var tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
    tcpListener.Start();

    while (true)
    {
      var socket = await tcpListener.AcceptSocketAsync();
      await Task.Run(() => MultipleRequest(socket));
    }
  }
}
