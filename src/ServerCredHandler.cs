using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using VintageAuth;
using Vintagestory.Client;

public static class ServerCredHandler {
    public static string connectDataToId(ServerConnectData connectData) {
        Console.WriteLine($"received data: {connectData}");
        if (connectData == null || connectData.HostRaw == null) {
            return null;
        }
        return $"{connectData.HostRaw}{connectData.Port}";
    }
    public static void SaveCredentials(string identifier, string password)
    {
        if (identifier == null) {
            return;
        }
        Dictionary<string, string> credentials = LoadCredentials();

        if (!credentials.ContainsKey(identifier))
        {
            credentials.Add(identifier, password);
        }
        else
        {
            credentials[identifier] = password;
        }

        SaveCredentialsToFile(credentials);
    }

    public static string GetCredentials(string identifier)
    {
        if (identifier == null) {
            return null;
        }
        Dictionary<string, string> credentials = LoadCredentials();

        if (credentials.ContainsKey(identifier))
        {
            return credentials[identifier];
        }

        return null;
    }

    public static void RemoveCredentials(string identifier)
    {
        if (identifier == null) {
            return;
        }
        Dictionary<string, string> credentials = LoadCredentials();

        if (credentials.ContainsKey(identifier))
        {
            credentials.Remove(identifier);
            SaveCredentialsToFile(credentials);
        }
    }

    private static Dictionary<string, string> LoadCredentials()
{
    Dictionary<string, string> credentials;

    if (File.Exists(VAConstants.SERVERCREDSPATH))
    {
        string json = File.ReadAllText(VAConstants.SERVERCREDSPATH);
        credentials = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
    }
    else
    {
        // Create directory structure if it doesn't exist
        Directory.CreateDirectory(Path.GetDirectoryName(VAConstants.SERVERCREDSPATH));
        credentials = new Dictionary<string, string>();
    }

    return credentials;
}

    private static void SaveCredentialsToFile(Dictionary<string, string> credentials)
    {
        string json = JsonConvert.SerializeObject(credentials, Formatting.Indented);
        File.WriteAllText(VAConstants.SERVERCREDSPATH, json);
    }
}