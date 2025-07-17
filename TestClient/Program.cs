using System.IO;
using System.Text;

System.Console.WriteLine("Generating Corefile and updating resolv.conf...");

string corefileContent = $@"
.:53 {{
    hosts {{
{GetHosts()}
    }}
    errors
    # log
}}";

System.Console.WriteLine($"Corefile:\n{corefileContent}");
Directory.CreateDirectory("/etc/coredns");
File.WriteAllText("/etc/coredns/Corefile", corefileContent);
Console.WriteLine("Corefile generated at /etc/coredns/Corefile");

// Read original resolv.conf and prepend CoreDNS nameserver
string resolvConfOriginal = File.ReadAllText("/etc/resolv.conf");
string newContent = "nameserver 127.0.0.1" + Environment.NewLine + resolvConfOriginal;
File.WriteAllText("/etc/resolv.conf", newContent);
System.Console.WriteLine("Updated /etc/resolv.conf to use CoreDNS.");

static string GetHosts()
{
    var hostVars = Environment.GetEnvironmentVariables().Cast<System.Collections.DictionaryEntry>().Where(e => e.Key.ToString().StartsWith("HOST_"));
    StringBuilder sb = new StringBuilder();
    foreach (System.Collections.DictionaryEntry hostVar in hostVars)
    {
        string host = hostVar.Key.ToString().Replace("_", "-").ToLower();
        foreach (string ipStr in hostVar.Value.ToString().Split())
        {
            sb.AppendLine($"        {ipStr} {host}");
        }
        sb.AppendLine();
    }
    return sb.ToString();
}