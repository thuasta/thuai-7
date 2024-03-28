using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ErrorLogger : MonoBehaviour
{
    private StreamWriter _fileWriter;
    // Start is called before the first frame update
    void Start()
    {
        string targetPath = $"{Directory.GetCurrentDirectory()}/ErrorLog";
        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
        }

        string fileName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".log";
        string filePath = Path.Combine(targetPath, fileName);
        _fileWriter = File.CreateText(filePath);
        _fileWriter.WriteLine(DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ": Client runs!\n");

        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception)
        {
            _fileWriter.WriteLine(logString);
            _fileWriter.WriteLine(stackTrace + "\n");
        }
    }
    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }
    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
        _fileWriter.Close();
    }
}
