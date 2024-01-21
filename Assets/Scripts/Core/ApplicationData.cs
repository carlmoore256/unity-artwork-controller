using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

public class ApplicationData
{
    public static void SaveJSON(string json, string filename)
    {
        var path = Path.Combine(Application.persistentDataPath, filename);
        File.WriteAllText(path, json);
    }

    public static void SaveJSON(string json, string folder, string filename)
    {
        if (!Directory.Exists(Path.Combine(Application.persistentDataPath, folder)))
        {
            Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, folder));
        }
        var path = Path.Combine(Application.persistentDataPath, folder, filename);
        Debug.Log($"[ApplicationData] Saving to {path}");
        File.WriteAllText(path, json);
    }

    public static T LoadJSON<T>(string filename)
    {
        var path = Path.Combine(Application.persistentDataPath, filename);
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static T LoadJSON<T>(string folder, string filename)
    {
        var path = Path.Combine(Application.persistentDataPath, folder, filename);
        var json = File.ReadAllText(path);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static string[] GetFiles(string folder, bool removeExtension = false)
    {
        var path = Path.Combine(Application.persistentDataPath, folder);
        if (!Directory.Exists(path))
        {
            return null;
        }
        var files = Directory.GetFiles(path);
        if (removeExtension)
            files = files.Select(f => Path.GetFileNameWithoutExtension(f)).ToArray();
        return files;
    }
}
