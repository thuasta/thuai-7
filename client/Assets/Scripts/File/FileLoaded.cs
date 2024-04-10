using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
/// <summary>
/// Transfer the file between Scene "Menu" and "Record"
/// </summary>
public class FileLoaded : MonoBehaviour
{
    public enum FileType
    {
        None,
        Level,
        Record
    };

    public string File { get; set; }
    public FileType Type = FileLoaded.FileType.Level;

    public bool HaveServer = false;
    public string Token;
    public string ServerHost;
    public int ServerPort;
    public Process ServerProcess;
    /// <summary>
    /// The obj would not be destroyed to transfer the data
    /// </summary>
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
}
