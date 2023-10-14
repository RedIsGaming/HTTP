using System.Net;
using System.Net.Sockets;
namespace HTTP;

public class Program
{
  private static void MultipleRequest(Socket connection)
  {
    var request = new NetworkStream(connection);
    var requestReader = new StreamReader(request);
    var streamWriter = new StreamWriter(request);
    
    try
    {
      var firstLine = requestReader.ReadLine()?.Split(" ");

      if (firstLine == null)
      {
        return;
      }
      
      var line = requestReader.ReadLine();
      const int contentLength = 0;
      SearchRequest(line, requestReader, contentLength);
      Content(streamWriter);
    }

    finally
    {
      connection.Close();
      request.Close();
      requestReader.Close();
      streamWriter.Close();
    }
  }

  private static void SearchRequest(string? line, StreamReader requestReader, int contentLength)
  {
    if (contentLength > 0)
    {
      var bytes = new char[contentLength];
      requestReader.Read(bytes, 0, contentLength);
    }
    
    while (!string.IsNullOrEmpty(line) && !requestReader.EndOfStream)
    {
      var pieces = line.Split(":");
      (string header, var value) = (pieces[0], pieces[1]);

      if (header.ToLower() == "content-length")
      {
        contentLength = int.Parse(value);
      }

      line = requestReader.ReadLine();
    }
  }

  private static void Content(TextWriter streamWriter)
  {
    var content = "HTTP/2.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: 1000\r\n\r\n";
    content += "<html>";
    content += "  <head>";
    content += "    <title>HTTP requests</title>";
    content += "  </head>";
    content += "  <body>";
    content += "    <p>Welcome at the website of Amusement park";
    content += "    <a href='nl.wikipedia.org/wiki/Den_Haag' target='blank'>The Hague!</a></p>";
    content += "  </body>";
    content += "</html>";
    
    streamWriter.Write(content);
    streamWriter.Flush();
  }
  
  private static void Main()
  {
    var server = new TcpListener(new IPAddress(new byte[] {127, 0, 0, 1}), 5000);
    server.Start();

    while (true)
    {
      var connection = server.AcceptSocket();
      Task.Run(() => MultipleRequest(connection));
    }
  }
}
