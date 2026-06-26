using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

//step1 - taking url then creating the request
Console.WriteLine("Enter Url: ");
Uri uri = new Uri(Console.ReadLine());
TcpClient client = new TcpClient(uri.Host, uri.Port);
var stream = client.GetStream();
string request = 
    $"GET {uri.PathAndQuery} HTTP/1.1\r\n" +
    $"Host: {uri.Host} \r\n" +
    "Connection: close\r\n" +
    "User-Agent: Mozilla/5.0\r\n" +
    "\r\n";

//step2 - getting the stream
static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
{
    return true;
}

Stream activeStream = stream;

if (uri.Scheme == "https") 
{
    SslStream sslStream = new SslStream(
        stream, 
        false, 
        new RemoteCertificateValidationCallback(CertificateValidationCallback));

    sslStream.AuthenticateAsClient(uri.Host);

    activeStream = sslStream;
}


//step3 - encode
byte[] encoded = Encoding.ASCII.GetBytes(request);

activeStream.Write(encoded, 0, encoded.Length);

//reading
byte[] buffer = new byte[4096];

int bytesRead;
bytesRead = activeStream.Read(buffer, 0, buffer.Length);

StringBuilder sb = new StringBuilder();

while (bytesRead > 0){
    string putTogether = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    sb.Append(putTogether);
    bytesRead = activeStream.Read(buffer, 0, buffer.Length);
}

//header removal
string part1 = sb.ToString();
int split = part1.IndexOf("\r\n\r\n");
string headers = part1.Substring(0, split);
string removedHeaders = part1.Substring(split + 4);
//use when needed
//Console.WriteLine(headers);
//<> stripping vars

string body;
int location1;
int location2;
int removesigns;
//<style> removing
while (removedHeaders.IndexOf("<style>") != -1)
{
    location1 = removedHeaders.IndexOf("<style>");
    location2 = removedHeaders.IndexOf("</style>", location1);
    removesigns = location2 - location1 + "</style>".Length;
    removedHeaders = removedHeaders.Remove(location1, removesigns);
}
//<script> removing
while (removedHeaders.IndexOf("<script>") != -1)
{
    location1 = removedHeaders.IndexOf("<script>");
    location2 = removedHeaders.IndexOf("</script>", location1);
    removesigns = location2 - location1 + "</script>".Length;
    removedHeaders = removedHeaders.Remove(location1, removesigns);
}
//<title> removing
while (removedHeaders.IndexOf("<title>") != -1)
{
    location1 = removedHeaders.IndexOf("<title>");
    location2 = removedHeaders.IndexOf("</title>", location1);
    removesigns = location2 - location1 + "</title>".Length;
    removedHeaders = removedHeaders.Remove(location1, removesigns);
}
//<> stripping
while (removedHeaders.IndexOf("<") != -1)
{
    location1 = removedHeaders.IndexOf("<");
    location2 = removedHeaders.IndexOf(">", location1);
    removesigns = location2 - location1 + 1;
    removedHeaders = removedHeaders.Remove(location1, removesigns);
}

string[] lines = removedHeaders.Split("\r\n");
List<string> kept = new List<string>();

foreach (var line in lines)
{
    if (!isHex(line))
    {
        kept.Add(line);
    }
}
string join = String.Join("\r\n", kept);

body = join;
Console.WriteLine(body);

//Source - https://stackoverflow.com/a/223854
//creates a true or false bool named isHex, and for each character(c) in characters in the string
//itll check if its a hex by going thru several checks, and if its !(not a hex) itll go false, otherwise true

bool isHex(string chars)
{
    bool isaHex;
    foreach (var c in chars)
    {
        isaHex = ((c >= '0' && c <= '9') ||
            (c >= 'a' && c <= 'f') ||
            (c >= 'A' && c <= 'F'));

        if (!isaHex)
            return false;
    }
    return true;
}