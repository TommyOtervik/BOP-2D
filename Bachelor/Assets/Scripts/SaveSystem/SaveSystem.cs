using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
/*
 * Lagrins system
 *  Brukes til å lagre data i binær format. Serializable.
 *  
 *   @AOP - 225280
 */
public static class SaveSystem
{
    public static void SavePlayer(Player player)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/Player.fun";

        FileStream stream = new FileStream(path, FileMode.Create);
        PlayerData data = new PlayerData(player);

        formatter.Serialize(stream, data);
        stream.Close();
    }


    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/Player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static void SaveAbomination(AbominationMiniBoss abom)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/abom.dat";

        FileStream stream = new FileStream(path, FileMode.Create);
        AbominationData data = new AbominationData(abom);


        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static AbominationData LoadAbomination()
    {
        string path = Application.persistentDataPath + "/abom.dat";

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            AbominationData data = formatter.Deserialize(stream) as AbominationData;
            
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }

    }

}
