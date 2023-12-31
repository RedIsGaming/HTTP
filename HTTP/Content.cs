using System.Net;
namespace HTTP;

public readonly struct Content<T>
where T : IConvertible
{
  private static int counter = 0;
  private const string OK = "HTTP/2.0 200 OK\r\nContent-Type: text/html\r\nContent-Length: 1000\r\n\r\n";
  private const string NOTFOUND = "HTTP/2.0 404 Not Found\r\nContent-Type: text/html\r\nContent-Length: 200\r\n\r\n";

  public T Page(string method, string url, TextWriter textWriter) {
    return method == "GET" && url == "/contact" ? Contact(textWriter) : 
      method == "GET" && url == "/counter" ? Counter(textWriter) : 
      method == "GET" && url == "/" ? Home(textWriter) : 
      NotFound(textWriter);
  }

  private T CreateHTML(string body) {
    var content = OK;
    content += $"<html>";
    content += $"  <head><title>HTTP requests</title></head>";
    content += $"  <body>{body}</body>";
    content += $"</html>";

    return (T) Convert.ChangeType(content, typeof(T));
  }

  private T Home(TextWriter textWriter)
  {
    var content = "";
    content += $"<p>Welcome at the website of Amusement park ";
    content += $"<a href='https://nl.wikipedia.org/wiki/Den_Haag' target='blank'>The Hague!</a></p>";
    content += $"<a href='/contact'>To contact page</a>";
    content += $"<p>{HttpRequestHeader.UserAgent}</p>";
    
    var html = CreateHTML(content);
    textWriter.Write(html);
    return html;
  }

  private T Contact(TextWriter textWriter)
  {
    var content = $"<p>This is the contact page of Willem the Zwijger!</p>";

    var html = CreateHTML(content);
    textWriter.Write(html);
    return html;
  }

  private T Counter(TextWriter textWriter) {
    counter += 1;
    var content = $"<p>{counter}</p>";

    var html = CreateHTML(content);
    textWriter.Write(html);
    return html;
  }

  private T NotFound(TextWriter textWriter) {
    var content = NOTFOUND;
    content += $"<html>";
    content += $"  <head><title>HTTP requests</title></head>";
    content += $"  <body><h1>Page not found, because more then 1 penis exist!</h1></body>";
    content += $"</html>";
    
    textWriter.Write(content);
    return (T) Convert.ChangeType(content, typeof(T));
  }
}
