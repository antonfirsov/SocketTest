using System.IO;
using System.Text;

System.Console.WriteLine("Generating Corefile and updating resolv.conf...");

string corefileContent = $@"
.:53 {{
    hosts {{
        {GetHosts("HOST_A", "HOST_B")}
    }}
    errors
    log
}}";

Directory.CreateDirectory("/etc/coredns");
File.WriteAllText("/etc/coredns/Corefile", corefileContent);
Console.WriteLine("Corefile generated at /etc/coredns/Corefile");

// Read original resolv.conf and prepend CoreDNS nameserver
string resolvConfOriginal = File.ReadAllText("/etc/resolv.conf");
string newContent = "nameserver 127.0.0.1" + Environment.NewLine + resolvConfOriginal;
File.WriteAllText("/etc/resolv.conf", newContent);
System.Console.WriteLine("Updated /etc/resolv.conf to use CoreDNS.");

static string GetHosts(params string[] hostVars)
{
    StringBuilder sb = new StringBuilder();
    foreach (string hostVar in hostVars)
    {
        string host = hostVar.Replace("_", "-").ToLower();
        foreach (string ipStr in Environment.GetEnvironmentVariable(hostVar).Split())
        {
            sb.AppendLine($"{ipStr} {host}");
        }
    }
    return sb.ToString();
}
