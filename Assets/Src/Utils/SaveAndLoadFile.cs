using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Newtonsoft.Json;

public static class SaveAndLoadFile
{
    public static void SaveData(string fileName, object data)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".dat";
        FileStream fileStream = new FileStream(path, FileMode.Create);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(fileStream, data);
        fileStream.Close();
        Debug.Log($"SaveData: {path}");
    }

    public static T LoadData<T>(string fileName, T defaultValue)
    {
        string path = Application.persistentDataPath + "/" + fileName + ".dat";
        if (!File.Exists(path))
        {
            Debug.Log($"LoadData: {path} not found");
            return defaultValue;
        }
        FileStream fileStream = new FileStream(path, FileMode.Open);
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        object data = binaryFormatter.Deserialize(fileStream);
        fileStream.Close();
        Debug.Log($"LoadData: {path} data: {JsonConvert.SerializeObject(data)}");
        return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(data));
    }
}
