using System.IO;
using saving_loading;
using UnityEngine;

namespace saving_loading
{
    public static class SaveSystem
    {
        private const int MaxSlots = 5;

        public static void Save(SaveData data)
        {
            string path = Application.persistentDataPath + "/save_" + GetNextSlot() + ".json";
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
        }

        public static SaveData Load(int slot)
        {
            string path = Application.persistentDataPath + "/save_" + slot + ".json";

            if (!File.Exists(path))
                return null;

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }

        public static int GetNextSlot()
        {
            for (int i = 0; i < MaxSlots; i++)
            {
                string path = Application.persistentDataPath + "/save_" + i + ".json";
                if (!File.Exists(path))
                    return i;
            }

            return -1; // no slots left
        }

        public static int CountSlots()
        {
            int count = 0;
            for (int i = 0; i < MaxSlots; i++)
            {
                string path = Application.persistentDataPath + "/save_" + i + ".json";
                if (File.Exists(path))
                    count++;
            }

            return count;
        }
    }
}