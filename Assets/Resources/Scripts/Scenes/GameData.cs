using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class GameData {
    static readonly string GAME_DATA_DIR = Application.persistentDataPath+"/GameData";
    static GameData() { if (!Directory.Exists(GAME_DATA_DIR)) Directory.CreateDirectory(GAME_DATA_DIR); }
    public static object readFile(string fileName) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Open(GAME_DATA_DIR + fileName, FileMode.OpenOrCreate);
        if (saveFile.Length == 0) {
            saveFile.Close();
            return null;
        }
        object data = formatter.Deserialize(saveFile);
        saveFile.Close();
        return data;
    }
    public static void writeFile(string fileName, object data) {
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream saveFile = File.Open(GAME_DATA_DIR + fileName, FileMode.OpenOrCreate);
        formatter.Serialize(saveFile, data);
        saveFile.Close();
    }
    public static void deleteFile(string fileName) {
        File.Delete(GAME_DATA_DIR + fileName);
    }
}
