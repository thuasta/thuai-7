using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;
using System.Text.RegularExpressions;

public class MenuController : MonoBehaviour
{
    /// <summary>
    /// The path of the total project
    /// </summary>
    private string _projectPath;

    /// <summary>
    /// Button sound controller
    /// </summary>
    private AudioSource _buttonSound;


    private Button _startGameButton;
    private GameObject _title;
    /// <summary>
    /// Load file objects
    /// </summary>
    // private Upload _upload = new();
    private FileLoaded _fileLoaded;
    /// <summary>
    /// Switch button
    /// </summary>
    private Button _recordRefreshButton;
    private readonly Button _playSwitchButton;

    /// <summary>
    /// The content in the scroll view
    /// </summary>
    private GameObject _scrollViewContent;
    private GameObject _scrollView;
    private Button _closeButton;

    /// <summary>
    /// The address of all nclevels
    /// </summary>
    private string[] _levels;
    /// <summary>
    /// The button that is displayed in the 'record' column
    /// </summary>
    [SerializeField]
    private GameObject _recordButtonPrefab;
    private readonly List<Button> _recordButtons = new();

    /// <summary>
    /// Help
    /// </summary>
    private Button _helpButton;
    private GameObject _helpDocumentWindow;
    private Button _closeHelpButton;

    /// <summary>
    /// Quit game
    /// </summary>
    private Button _quitButton;

    // Start is called before the first frame update
    void Start()
    {
        _projectPath = Directory.GetCurrentDirectory();
        _fileLoaded = GameObject.Find("RecordReader").GetComponent<FileLoaded>();


        _helpButton = GameObject.Find("Canvas/HelpButton").GetComponent<Button>();
        _helpButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            _helpDocumentWindow.SetActive(true);
            // Hide the title and start game button self.
            _title.SetActive(false);
            _startGameButton.gameObject.SetActive(false);
            _quitButton.gameObject.SetActive(false);
            _helpButton.gameObject.SetActive(false);
        });
        _helpDocumentWindow = GameObject.Find("Canvas/HelpDocument");
        _closeHelpButton = GameObject.Find("Canvas/HelpDocument/CloseButton").GetComponent<Button>();
        _closeHelpButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            _helpDocumentWindow.SetActive(false);
            // Unroll the title and start game button self.
            _title.SetActive(true);
            _startGameButton.gameObject.SetActive(true);
            _quitButton.gameObject.SetActive(true);
            _helpButton.gameObject.SetActive(true);
        });
        _helpDocumentWindow.SetActive(false);

        _quitButton = GameObject.Find("Canvas/Quit").GetComponent<Button>();
        _quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
        
        // Sound
        _buttonSound = GameObject.Find("ButtonSound").GetComponent<AudioSource>();
        _buttonSound.clip ??= Resources.Load<AudioClip>("Music/Sound/Button/ButtonClick");


        _scrollView = GameObject.Find("Canvas/RecordViewer");
        _scrollViewContent = GameObject.Find("Canvas/RecordViewer/Viewport/Content");
        _recordButtonPrefab = Resources.Load<GameObject>("GUI/Button/LevelButton");
        Debug.Log(_recordButtonPrefab);
        // Define the event of start button
        _title = GameObject.Find("Canvas/Title");
        _startGameButton = GameObject.Find("Canvas/StartGameButton").GetComponent<Button>();
        _startGameButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            // Hide the title and start game button self.
            _title.SetActive(false);
            _startGameButton.gameObject.SetActive(false);
            _quitButton.gameObject.SetActive(false);
            _helpButton.gameObject.SetActive(false);
            // Set the _scrollView active
            _scrollView.SetActive(true);
            // In default, list all the nclevels.
            ListAllLevels();
        });

        // Define the event of close button
        _closeButton = GameObject.Find("Canvas/RecordViewer/CloseButton").GetComponent<Button>();
        _closeButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            ClearChildrenInContent();
            // Open the title and start game button self.
            _title.SetActive(true);
            _startGameButton.gameObject.SetActive(true);
            _quitButton.gameObject.SetActive(true);
            _helpButton.gameObject.SetActive(true);
            // Hide the _scrollView
            _scrollView.SetActive(false);
        });

        void ClearChildrenInContent()
        {
            _recordButtons.Clear();

            // Clear the content of scroll bar //
            Transform scrollViewContentTransform = _scrollViewContent.transform;
            // Add all the children into the list
            List<GameObject> scrollViewContentChildren = new List<GameObject>();
            for (int i = 0; i < scrollViewContentTransform.childCount; i++)
            {
                scrollViewContentChildren.Add(scrollViewContentTransform.GetChild(i).gameObject);
            }
            // Destroy all the children
            foreach (GameObject child in scrollViewContentChildren)
            {
                Debug.Log("Clear: ", child);
                Destroy(child);
            }
        }

        void ListAllLevels(bool startServer = false, bool isRecord = true)
        {
            Debug.Log($"{_projectPath}");
            // Prior: find folders 
            List<string> LevelFolders = Directory.GetFiles($"{_projectPath}/Records", "*", SearchOption.AllDirectories).ToList();
            // Next: find files
            // string[] allLevels = Directory.GetFiles($"{_projectPath}/record", "*.dat", SearchOption.AllDirectories);
            // Compare them
            // foreach (string file in allLevels)
            // {
            //    bool haveFolder = false;
            //    foreach (string folder in LevelFolders)
            //    {
            //        if (folder == file) { haveFolder = true; break; }
            //    }
            //    if (!haveFolder)
            //    {
            //        LevelFolders.Add(file);
            //    }
            // }
            _levels = LevelFolders.ToArray();
            int cnt = 0;
            foreach (string folderName in _levels)
            {
                Debug.Log(folderName);
                // Create record button objects 
                GameObject newRecordButtonObject = Instantiate(_recordButtonPrefab);
                Button newRecordButton = newRecordButtonObject.GetComponent<Button>();
                TMP_Text recordText = newRecordButtonObject.GetComponentInChildren<TMP_Text>();
                // Get nclevel name
                int index = Math.Max(folderName.LastIndexOf('/'), folderName.LastIndexOf('\\'));
                string name = folderName[(index + 1)..];
                recordText.text = $" {name}";
                if (recordText.text.Length > 20)
                {
                    recordText.text = recordText.text.Substring(0,20)+" ...";
                }

                // Bind the event onto the button
                newRecordButton.onClick.AddListener(() =>
                {
                    // Play sound
                    _buttonSound.Play();
                    // Load the record file
                    _fileLoaded.File = folderName;
                    //_fileLoaded.File = Directory.GetFiles(folderName, "*.dat", SearchOption.AllDirectories)[0];
                    Debug.Log($"INFO: Record file name: {_fileLoaded.File}");
                    
                    SceneManager.LoadScene("Record");
                });

                // Put the button into the content
                newRecordButtonObject.transform.SetParent(_scrollViewContent.transform);
                newRecordButtonObject.transform.localScale = Vector3.one;
                newRecordButtonObject.GetComponent<RectTransform>().localPosition = -new Vector3(0,50+cnt*50,0);
                //Debug.Log(newRecordButtonObject.transform.position);
                //Debug.Log(newRecordButtonObject.GetComponent<RectTransform>().transform.position);
                _recordButtons.Add(newRecordButton);
                cnt += 1;
            }
        }


        // Define the event of record switch button
        _recordRefreshButton = GameObject.Find("Canvas/RecordViewer/RecordRefreshButton").GetComponent<Button>();
        _recordRefreshButton.onClick.AddListener(() =>
        {
            // Play sound
            _buttonSound.Play();
            // Clear the content of scroll bar
            ClearChildrenInContent();
            // Find and display the nclevels in the path
            ListAllLevels();
        });

        // Close the scroll view before the start game button is pressed
        _scrollView.SetActive(false);

    }


}
