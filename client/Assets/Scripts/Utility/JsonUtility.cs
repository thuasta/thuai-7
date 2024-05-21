using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json.Linq;

public class JsonUtility
{
    /// <summary>
    /// Unzip the level_data.dat
    /// </summary>
    /// <param name="levelDataZipArchive"></param>
    /// <exception cref="Exception"></exception>
    public static JObject UnzipLevel(string path)
    {
        Stream levelDataStream = null;

        if (Directory.Exists(path))
        {
            levelDataStream = ZipFile.OpenRead($"{path}/level.dat").GetEntry("level_data.json").Open() ??
             throw new Exception("Level data not found in zip archive.");
        }
        else if (File.Exists(path))
        {
            ZipArchive mcLevelDataZipFile = ZipFile.OpenRead(path);
            //Stream mcLevelDataEntryStream = mcLevelDataZipFile.GetEntry("level.dat.old").Open() ??
            //    (mcLevelDataZipFile.GetEntry("level.dat").Open() ??
            //    throw new Exception("mcLevel data not found in zip archive."));
            Stream mcLevelDataEntryStream = (mcLevelDataZipFile.GetEntry("level.dat").Open() ??
            throw new Exception("mcLevel data not found in zip archive."));


            ZipArchive levelDataZipFile = new ZipArchive(mcLevelDataEntryStream);
            levelDataStream = levelDataZipFile.GetEntry("level_data.json").Open() ??
             throw new Exception("Level data not found in zip archive.");
        }


        // Read the level data to a JSON string.
        StreamReader levelDataStreamReader = new StreamReader(levelDataStream);

        //Debug.Log(levelDataEntryStreamReader.ReadToEnd());

        return (JObject)JToken.ReadFrom(new JsonTextReader(levelDataStreamReader));
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static JObject UnzipRecord(string path)
    {
        // Load all the record entry
        List<JObject> allRecordJsonObject = new();

        if (Directory.Exists(path))
        {
            Debug.Log("Record is a Directory.");
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            //Loop through each file
            foreach (string file in files)
            {
                try
                {
                    if (!file.EndsWith("/") && !file.Contains(".meta"))
                    {
                        using (Stream stream = File.OpenRead(file))
                        {
                            // Unzip the record
                            ZipArchive recordZipArchive = new(stream);
                            StreamReader recordStreamReader = new(recordZipArchive.Entries[0].Open());
                            allRecordJsonObject.Add((JObject)JToken.ReadFrom(new JsonTextReader(recordStreamReader)));
                            Debug.Log(recordStreamReader.ReadToEnd().ToString());
                        }
                    }
                }
                catch
                {

                }
            }
        }
        else if (File.Exists(path))
        {
            Debug.Log("Record is a Zipped File.");
            ZipArchive ncLevelDataZipFile = ZipFile.OpenRead($"{path}");
            foreach (ZipArchiveEntry recordEntry in ncLevelDataZipFile.Entries)
            {
                try
                {
                    Debug.Log($"Unzipped record file name: {recordEntry.FullName}");
                    // If the recordEntry is not folder and not level
                    if (!recordEntry.FullName.Contains("level") && !recordEntry.FullName.EndsWith("/"))
                    {
                        StreamReader recordStreamReader = new(recordEntry.Open());
                        allRecordJsonObject.Add((JObject)JToken.ReadFrom(new JsonTextReader(recordStreamReader)));
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }

        if (allRecordJsonObject.Count == 0)
            throw new Exception("Record data not found in zip archive.");

        // Compute the first tick
        // pair<int index, int tick>
        (int, int)[] indexAndTicks = new (int, int)[allRecordJsonObject.Count];
        int nowRecordIndex = 0;


        foreach (JObject jsonObject in allRecordJsonObject)
        {
            indexAndTicks[nowRecordIndex].Item1 = nowRecordIndex;
            // If the record file is wrong, then the record will not be added. So let initial tick equal to -1
            indexAndTicks[nowRecordIndex].Item2 = -1;
            //ZipFile.OpenRead(recordDataEntry.FullName);

            // Find the first tick;
            JArray records = (JArray)jsonObject["records"];
            if (records != null && records.Count > 0)
            {
                foreach (JToken recordInfo in records)
                {
                    JValue tick = (JValue)recordInfo["currentTicks"];
                    if (tick != null)
                    {
                        // The first tick
                        indexAndTicks[nowRecordIndex].Item2 = (int)tick;
                        break;
                    }
                }
                // string messageType = records[0]["messageType"].ToString();
                // if (messageType != null && messageType == "MAP" && records.Count > 1)
                // {
                //     JValue tick = (JValue)records[1]["currentTicks"];
                //     if (tick != null)
                //     {
                //         // The first tick
                //         indexAndTicks[nowRecordIndex].Item2 = (int)tick;
                //     }
                //     else
                //     {
                //         indexAndTicks[nowRecordIndex].Item2 = -1;
                //     }
                // }
                // else
                // {
                //     foreach (JToken recordInfo in records)
                //     {
                //         JValue tick = (JValue)recordInfo["currentTicks"];
                //         if (tick != null)
                //         {
                //             // The first tick
                //             indexAndTicks[nowRecordIndex].Item2 = (int)tick;
                //             break;
                //         }
                //         else
                //         {
                //             indexAndTicks[nowRecordIndex].Item2 = -2;
                //         }
                //     }
                // }
            }
            nowRecordIndex++;
        }
        // Rearrange the order of record file according to their first ticks
        List<(int, int)> indexAndTicksList = indexAndTicks.ToList<(int, int)>();
        indexAndTicksList.Sort((x, y) => x.Item2.CompareTo(y.Item2));

        foreach ((int, int) it in indexAndTicksList) {
            Debug.Log($"RecordInfo: {it.Item1},{it.Item2}");
        }

        // Write the json obj according to the order
        JObject recordJsonObject = new()
        {
            {"type","record" },
            { "records", new JArray() }
        };

        foreach ((int, int) indexAndTick in indexAndTicksList)
        {
            if (indexAndTick.Item2 != -2)
            {
                // Serial number in allRecordJsonObject: indexAndTick.Item1
                JObject jsonObject = allRecordJsonObject[indexAndTick.Item1];
                JArray records = (JArray)jsonObject["records"];

                // Append
                ((JArray)recordJsonObject["records"]).Merge(records);
            }
        }
        // Sort the final array according to tick
        JArray allRecordsArray = (JArray)recordJsonObject["records"];

        //allRecordJsonObject.OrderBy(record => (int)record["tick"]);

        return recordJsonObject;
    }
    /// <summary>
    /// Parse the json file
    /// </summary>
    /// <param name="fileInfo">The file from class Upload.OpenFileName</param>
    /// <returns></returns>
    public static JsonTextReader ReadJsonFile(string filePath)
    {
        System.IO.StreamReader file = System.IO.File.OpenText(filePath);
        return new JsonTextReader(file);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonPath">which is inner the resource folder</param>
    /// <returns></returns>
    public static Dictionary<string, int> ParseBlockDictJson(string jsonPath)
    {
        // "Json/Dict"
        TextAsset text = Resources.Load(jsonPath) as TextAsset;
        string json = text.text;
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }
        else
        {
            Dictionary<string, int> dict = JsonConvert.DeserializeObject<Dictionary<string, int>>(json);
            // Delete the prefix "minecraft:" in keys
            string prefix = "minecraft:";

            string ReplaceKey(string key, int prefixIndex)
            {
                key = key.Substring(prefixIndex + prefix.Length);
                // Capitalize the name 
                key = key[..1].ToUpper() + key[1..];
                return key;
            };

            dict = dict.ToDictionary(dictItem => dictItem.Key.IndexOf(prefix) == -1 ?
                dictItem.Key : ReplaceKey(dictItem.Key, dictItem.Key.IndexOf(prefix)),
                dictItem => dictItem.Value);

            return dict;
        }
    }
}
