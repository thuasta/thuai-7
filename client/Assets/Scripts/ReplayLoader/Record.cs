using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Record : MonoBehaviour
{
    public const float ObjPrefabScaling=0.4f;
    public enum PlayState
    {
        Prepare,
        Play,
        Pause,
        End,
        Jump
    }
    public static float MapLength = 256;
    public class RecordInfo
    {
        // 20 frame per second
        public const float FrameTime = 0.05f;
        public PlayState NowPlayState = PlayState.Pause;
        public int NowTick = 200;
        /// <summary>
        /// Now record serial number
        /// </summary>
        public int NowRecordNum = 0;
        /// <summary>
        /// The speed of the record which can be negative
        /// </summary>
        public float RecordSpeed = 1f;
        public const float MinSpeed = -5f;
        public const float MaxSpeed = 5f;

        /// <summary>
        /// Contains all the item in the game
        /// </summary>
        public double NowFrameTime
        {
            get
            {
                return FrameTime / RecordSpeed;
            }
        }
        /// <summary>
        /// If NowDeltaTime is larger than NowFrameTime, then play the next frame
        /// </summary>
        public float NowDeltaTime = 0;
        public long NowTime = 0;
        /// <summary>
        /// The target tick to jump
        /// </summary>
        public int JumpTargetTick = int.MaxValue;
        /// <summary>
        /// Current max tick
        /// </summary>
        public int MaxTick;
        public void Reset()
        {
            // RecordSpeed = 1f;
            NowTick = 0;
            NowRecordNum = 0;
            JumpTargetTick = int.MaxValue;
        }
    }
    // meta info
    public RecordInfo _recordInfo;

    // GUI
    private Button _backButton;
    private Sprite _backButtonSprite;
    private Button _showBorderButton;
    private Sprite _showBorderButtonSprite;
    private Sprite _hideBorderButtonSprite;
    private Button _stopButton;
    private Sprite _stopButtonSprite;
    private Sprite _continueButtonSprite;
    private readonly Button _replayButton;
    private readonly Slider _recordSpeedSlider;
    private readonly TMP_Text _recordSpeedText;
    private readonly float _recordSpeedSliderMinValue;
    private readonly float _recordSpeedSliderMaxValue;
    private readonly Slider _processSlider;
    private readonly TMP_Text _jumpTargetTickText;
    private readonly TMP_Text _maxTickText;
    private  TMP_Text _currentTickText;
    private GameObject _groundPrefab;
    private GameObject _playerPrefab;
    private bool[,] _isWalls;
    private ParticleSystem _poisonousCircle;
    private TMP_Text _infoText;
    private GameObject _spotLight;

    private readonly List<GameObject> _obstaclePrefabs = new List<GameObject>();
    private GameObject _borderPrefab;
    private GameObject _borderParent;
    // private readonly List<GameObject> _borders = new List<GameObject>();
    private readonly Dictionary<string, List<GameObject>> itemInstances = new Dictionary<string, List<GameObject>>();
    // record data
    private readonly string _recordFilePath = null;
    private JArray _recordArray;
    private string _recordFile;
    private Observe _observe;

    private GameObject _supplyParent;

    private Dictionary<string, GameObject> _propDict;
    private readonly string[] _allAvailableSupplies = new string[]
    {
            // Weapons
            "S686", "M16", "VECTOR", "AWM",
            // Medicines
            "BANDAGE", "FIRST_AID",
            // Armors
            "PRIMARY_ARMOR", "PREMIUM_ARMOR",
            // Bullets
            "BULLET",
            // Grenades
            "GRENADE"
    };
    private Camera _camera;
    // viewer

    private Dictionary<string, AudioClip> _audioClipDict;
    private GameObject _grenadeExplosionPrefab;
    private GameObject _grenadeBeamPrefab;
    private AudioSource _as;
private void Start()
    {
        if (Debug.isDebugBuild)
        {
            Debug.unityLogger.logEnabled = true;
        }
        else
        {
            Debug.unityLogger.logEnabled = false;
        }
        // Initialize the _recordInfo
        _recordInfo = new();
        //// Initialize the ItemCreator
        // _entityCreator = GameObject.Find("EntityCreator").GetComponent<EntityCreator>();
        // Get json file
        FileLoaded fileLoaded = GameObject.Find("RecordReader").GetComponent<FileLoaded>();
        // Check if the file is Level json
        _recordFile = fileLoaded.File;
        _observe = GameObject.Find("Camera").GetComponent<Observe>();
        _recordInfo.NowPlayState = PlayState.Pause;

        _infoText = GameObject.Find("Canvas/Info").GetComponent<TMP_Text>();
        // Prefab
        _groundPrefab = Resources.Load<GameObject>("Prefabs/Ground_01");
        _playerPrefab = Resources.Load<GameObject>("Prefabs/Player");
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Rock_01"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Rock_02"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Rock_03"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Rock_04"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Rock_05"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Tree_01"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Tree_02"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Tree_03"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Tree_04"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Tree_05"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Stump_01"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Bush_01"));
        _obstaclePrefabs.Add(Resources.Load<GameObject>("Prefabs/Bush_02"));

        _borderPrefab = Resources.Load<GameObject>("Square/Square");

        _currentTickText = GameObject.Find("Canvas/Tick").GetComponent<TMP_Text>();

        _poisonousCircle = GameObject.Find("PoisonousCircle").GetComponent<ParticleSystem>();
        _propDict = new()
        {
            { "BANDAGE", Resources.Load<GameObject>("Prefabs/Bandage") },
            { "FIRST_AID", Resources.Load<GameObject>("Prefabs/FirstAid") },
            { "AWM", Resources.Load<GameObject>("Prefabs/AWM") },
            { "VECTOR", Resources.Load<GameObject>("Prefabs/Vector") },
            { "S686", Resources.Load<GameObject>("Prefabs/S686") },
            { "M16", Resources.Load<GameObject>("Prefabs/M16") },
            { "BULLET", Resources.Load<GameObject>("Prefabs/Bullet") },
            { "PRIMARY_ARMOR", Resources.Load<GameObject>("Prefabs/Primary_armor") },
            { "PREMIUM_ARMOR", Resources.Load<GameObject>("Prefabs/Premium_armor") },
            { "GRENADE", Resources.Load<GameObject>("Prefabs/Grenade") }
        };
        _supplyParent = GameObject.Find("Supplies");

        _audioClipDict = new()
        {
            { "AWM", Resources.Load<AudioClip>("Music/Audio/AWM") },
            { "VECTOR", Resources.Load<AudioClip>("Music/Audio/VECTOR") },
            { "S686", Resources.Load<AudioClip>("Music/Audio/S686") },
            { "M16", Resources.Load<AudioClip>("Music/Audio/M16") },
            { "FISTS", Resources.Load<AudioClip>("Music/Audio/FISTS") },
            { "FireInTheHole", Resources.Load<AudioClip>("Music/Audio/ct_fireinhole")},
            { "Go", Resources.Load<AudioClip>("Music/Audio/go") },
            { "Die", Resources.Load<AudioClip>("Music/Audio/die") },
            { "Grenade", Resources.Load<AudioClip>("Music/Audio/grenade") },
            { "Pickup", Resources.Load<AudioClip>("Music/Audio/pickup") },
            { "Heal", Resources.Load<AudioClip>("Music/Audio/heal") },
            { "Hurt", Resources.Load<AudioClip>("Music/Audio/hurt") }
        };
        _grenadeExplosionPrefab = Resources.Load<GameObject>("Prefabs/BigExplosionEffect");
        _grenadeBeamPrefab = Resources.Load<GameObject>("Beam/GrenadeBeam");
        // GUI //

        // Get stop button 
        _stopButton = GameObject.Find("Canvas/StopButton").GetComponent<Button>();
        // Get stop button sprites
        _stopButtonSprite = Resources.Load<Sprite>("GUI/Button/StopButton");
        _continueButtonSprite = Resources.Load<Sprite>("GUI/Button/ContinueButton");
        // Pause at beginning
        _stopButton.GetComponent<Image>().sprite = _continueButtonSprite;
        // Add listener to stop button
        _stopButton.onClick.AddListener(() =>
        {
           if (_recordInfo.NowPlayState == PlayState.Play)
           {
               _stopButton.GetComponent<Image>().sprite = _continueButtonSprite;
               _recordInfo.NowPlayState = PlayState.Pause;
           }
           else if (_recordInfo.NowPlayState == PlayState.Pause)
           {
                _stopButton.GetComponent<Image>().sprite = _stopButtonSprite;
                _recordInfo.NowPlayState = PlayState.Play;
                _recordInfo.NowTime = System.DateTime.Now.Ticks;
                _as.PlayOneShot(_audioClipDict["Go"]);
           }
        });

        _showBorderButton = GameObject.Find("Canvas/ShowBorderButton").GetComponent<Button>();
        _showBorderButtonSprite = Resources.Load<Sprite>("GUI/Button/ShowButton");
        _hideBorderButtonSprite = Resources.Load<Sprite>("GUI/Button/HideButton");
        _showBorderButton.GetComponent<Image>().sprite = _showBorderButtonSprite;
        _showBorderButton.onClick.AddListener(() =>
        {
            if (_borderParent.activeSelf)
            {
                _showBorderButton.GetComponent<Image>().sprite = _showBorderButtonSprite;
                _borderParent.SetActive(false);
            }
            else
            {
                _showBorderButton.GetComponent<Image>().sprite = _hideBorderButtonSprite;
                _borderParent.SetActive(true);
            }
        });

        _backButton = GameObject.Find("Canvas/BackButton").GetComponent<Button>();
        _backButtonSprite = Resources.Load<Sprite>("GUI/Button/BackButton");
        _backButton.GetComponent<Image>().sprite = _backButtonSprite;
        _backButton.onClick.AddListener(() =>
        {
            PlayerSource.Clear();
            SceneManager.LoadScene("UI");
        });

        _spotLight = GameObject.Find("Light");

        _camera = GameObject.Find("Camera").GetComponent<Camera>();
        // Get Replay button
        // _replayButton = GameObject.Find("Canvas/ReplayButton").GetComponent<Button>();
        // _replayButton.onClick.AddListener(() =>
        //{
        //     _recordInfo.Reset();
        //     _entityCreator.DeleteAllEntities();
        //});


        //// Record playing rate slider
        // _recordSpeedSlider = GameObject.Find("Canvas/RecordSpeedSlider").GetComponent<Slider>();
        // _recordSpeedText = GameObject.Find("Canvas/RecordSpeedSlider/Value").GetComponent<TMP_Text>();

        // _recordSpeedSliderMinValue =  _recordSpeedSlider.minValue;
        // _recordSpeedSliderMaxValue =  _recordSpeedSlider.maxValue;
        //// Set the default slider speed to 1;
        //// Linear: 0~1
        //float speedRate = (1 - RecordInfo.MinSpeed) / (RecordInfo.MaxSpeed - RecordInfo.MinSpeed);
        // _recordSpeedSlider.value =  _recordSpeedSliderMinValue + ( _recordSpeedSliderMaxValue -  _recordSpeedSliderMinValue) * speedRate;
        //// Add listener
        // _recordSpeedSlider.onValueChanged.AddListener((float value) =>
        //{
        //    // Linear
        //    float sliderRate = (value -  _recordSpeedSliderMinValue) / ( _recordSpeedSliderMaxValue -  _recordSpeedSliderMinValue);
        //    // Compute current speed
        //     _recordInfo.RecordSpeed = RecordInfo.MinSpeed + (RecordInfo.MaxSpeed - RecordInfo.MinSpeed) * sliderRate;
        //    // Update speed text
        //    _recordSpeedText.text = $"Speed: {Mathf.Round( _recordInfo.RecordSpeed * 100) / 100f:F2}";
        //    foreach (Player player in EntitySource.PlayerDict.Values)
        //    {
        //        player.PlayerAnimations.SetAnimatorSpeed( _recordInfo.RecordSpeed);
        //    }
        //});


        // Check
        if (_recordFile == null)
        {
            Debug.Log("Loading file error!");
            return;
        }
        _recordArray = LoadRecordData();
        _recordInfo.MaxTick = (int)_recordArray.Last["currentTicks"];
        // Generate Map and Supplies
        GenerateMap();
        GenerateSupplies();
        // Generate record Dict according to record array
        //foreach (JToken eventJson in  _recordArray)
        //{
        //    string identifier = eventJson["identifier"].ToString();
        //    if ( _recordDict.ContainsKey(identifier))
        //    {
        //         _recordDict[identifier].Add(eventJson);
        //    }
        //    else
        //    {
        //         _recordDict.Add(identifier, new JArray(eventJson));
        //    }
        //}

        //// Process slider
        // _processSlider = GameObject.Find("Canvas/ProcessSlider").GetComponent<Slider>();
        // _processSlider.value = 1;
        // _jumpTargetTickText = GameObject.Find("Canvas/ProcessSlider/Handle Slide Area/Handle/Value").GetComponent<TMP_Text>();
        // _maxTickText = GameObject.Find("Canvas/ProcessSlider/Max").GetComponent<TMP_Text>();
        // _recordInfo.MaxTick = (int)( _recordArray.Last["tick"]);
        // _maxTickText.text = $"{ _recordInfo.MaxTick}";
        //// Add listener
        // _processSlider.onValueChanged.AddListener((float value) =>
        //{
        //    int nowTargetTick = (int)(value *  _recordInfo.MaxTick) + 1; // Add 1 owing to interpolation
        //    if (PlayState.Play ==  _recordInfo.NowPlayState && Mathf.Abs( _recordInfo.NowTick - nowTargetTick) > 1)
        //    {
        //        // Jump //
        //        // Reset the scene if the jump tick is smaller than now tick
        //        if ( _recordInfo.NowTick > nowTargetTick)
        //        {
        //             _recordInfo.Reset();
        //             _entityCreator.DeleteAllEntities();
        //            // Reset All blocks;
        //            // foreach (JToken blockChangeEventJson in  _recordDict["after_block_change"])
        //        }
        //        // Change current state
        //         _recordInfo.NowPlayState = PlayState.Jump;
        //        // Change target tick
        //         _recordInfo.JumpTargetTick = nowTargetTick;

        //        _registeredAgents.Clear();

        //    }
        //});

        _as = GameObject.Find("AudioSourceObj").GetComponent<AudioSource>();
    }

    private JArray LoadRecordData()
    {
        JObject recordJsonObject = JsonUtility.UnzipRecord(_recordFile);
        // Load the record array
        JArray recordArray = (JArray)recordJsonObject["records"]; 

        if (recordArray == null)
        {
            Debug.Log("Record file is empty!");
            return null;
        }
        return recordArray; 
    }

    #region Event Definition

    private void GenerateMap()
    {
        // Generate map according to the _recordArray
        // Find the JObject with "messageType": "MAP"
        JObject mapJson = null;
        foreach (JToken eventJson in _recordArray)
        {
            if (eventJson["messageType"].ToString() == "MAP")
            {
                mapJson = (JObject)eventJson["data"];
                break;
            }
        }
        if (mapJson == null)
        {
            Debug.Log("Map not found!");
            return;
        }
        // Generate map according to the mapJson, and store the map in the _blocks
        int width = (int)mapJson["width"];
        int height = (int)mapJson["height"];
        MapLength = width;
        JArray mapArray = (JArray)mapJson["walls"];
        //// Initialize the ground
        //Transform groundParent = GameObject.Find("Map/Ground").transform;
        //for (int i = 0; i < width; i++)
        //{
        //    for (int j = 0; j < height; j++)
        //    {
        //        // offset 0.5
        //        GameObject ground = Instantiate(_groundPrefab, new Vector3(i+0.5f, 0, j+0.5f), Quaternion.identity);

        //        ground.transform.SetParent(groundParent);
        //        // The direction of ground is random
        //        ground.transform.Rotate(0, UnityEngine.Random.Range(0, 4) * 90, 0);
        //        ground.transform.localScale *= ObjPrefabScaling;
        //    }
        //}

        _isWalls = new bool[width, height];
        // Initialize the walls
        foreach (JToken wallJson in mapArray)
        {
            int x = (int)wallJson["x"];
            int y = (int)wallJson["y"];
            _isWalls[x, y] = true;
        }
        // Randomly initialize the walls according to the _isWalls
        Transform obstacleParent = GameObject.Find("Map/Obstacles").transform;
        _borderParent = GameObject.Find("Map/Borders");
        _borderParent.SetActive(false);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (_isWalls[i, j])
                {
                    GameObject obstacle = Instantiate(
                        _obstaclePrefabs[UnityEngine.Random.Range(0, _obstaclePrefabs.Count)], new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.identity
                    );
                    obstacle.transform.SetParent(obstacleParent);
                    // The scale and direction of ground is random
                    obstacle.transform.Rotate(0, UnityEngine.Random.Range(0, 360) , 0);

                    obstacle.transform.localScale = ObjPrefabScaling * new Vector3(
                        obstacle.transform.localScale.x*UnityEngine.Random.Range(0.7f,1.4f),
                        obstacle.transform.localScale.y* UnityEngine.Random.Range(1.0f,1.8f),
                        obstacle.transform.localScale.z* UnityEngine.Random.Range(0.7f,1.4f)
                    );
                    GameObject newborder = Instantiate(_borderPrefab, new Vector3(i + 0.5f, 0f, j + 0.5f), Quaternion.identity);
                    newborder.transform.SetParent(_borderParent.transform);
                }
            }
        }
        for (int i = -width; i < width * 2; i++)
        {
            for (int j = -height; j < height * 2; j++)
            {
                if (i >= -5 && i < width + 5 && j >= -5 && j < height + 5)
                { continue; }

                // Random rate: 0.1
                if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.0075)
                {
                    continue;
                }

                GameObject obstacle = Instantiate(
                    _obstaclePrefabs[UnityEngine.Random.Range(0, _obstaclePrefabs.Count)], new Vector3(i + 0.5f, 0, j + 0.5f), Quaternion.identity
                );
                obstacle.transform.SetParent(obstacleParent);
                // The scale and direction of ground is random
                obstacle.transform.Rotate(0, UnityEngine.Random.Range(0, 360), 0);

                obstacle.transform.localScale = ObjPrefabScaling * new Vector3(
                    obstacle.transform.localScale.x * UnityEngine.Random.Range(4.0f, 8.0f),
                    obstacle.transform.localScale.y * UnityEngine.Random.Range(4.0f, 8.0f),
                    obstacle.transform.localScale.z * UnityEngine.Random.Range(4.0f, 8.0f)
                );
            }
        }
    }
    void GenerateSupplies()
    {
        // TODO:
        // Generate supplies according to the _recordArray
        // Find the JObject with "messageType": "MAP"
        JObject suppliesJson = null;
        foreach (JToken eventJson in _recordArray)
        {
            if (eventJson["messageType"].ToString() == "SUPPLIES")
            {
                suppliesJson = (JObject)eventJson["data"];
                break;
            }
        }
        if (suppliesJson == null)
        {
            Debug.Log("Supplies not found!");
            return;
        }
        JArray suppliesArray = (JArray)suppliesJson["supplies"];
        foreach (JToken supplyJson in suppliesArray)
        {
            string name = supplyJson["name"].ToString();
            if (_propDict.ContainsKey(name))
            {
                Vector3 supplyPosition = new Vector3((float)supplyJson["position"]["x"]+0.5f, 0.1f, (float)supplyJson["position"]["y"]+0.5f);
                GameObject newSupply= Instantiate(_propDict[name],_supplyParent.transform);
                newSupply.transform.position = supplyPosition;
                newSupply.transform.Rotate(0,0, UnityEngine.Random.Range(0, 360));
                if (!itemInstances.ContainsKey(name) || itemInstances[name] == null)
                {
                    itemInstances[name] = new List<GameObject>();
                }
                itemInstances[name].Add(newSupply);
            }
        }
    }
    private void UpdatePlayers(JArray players)
    {
        if (players is null)
            return;

        string infoString = $"Camera Position: ({_camera.transform.position.x:F2}, {_camera.transform.position.z:F2})\n";
        foreach (JObject player in players)
        {
            int playerId = player["playerId"].ToObject<int>();
            string playerToken = player["token"] == null ? playerId.ToString()  : player["token"].ToString();
            Position playerPosition = new Position((float)player["position"]["x"], (float)player["position"]["y"]);

            // Check if the player is in dict
            PlayerSource.AddPlayer(playerId, playerToken);
            Dictionary<string, int> inventory = new();
            foreach (JObject item in (JArray)player["inventory"])
            {
                string name=item["name"].ToString();
                if (Array.IndexOf(_allAvailableSupplies, item["name"].ToString())!=-1 ){
                    inventory.Add(name, (int)item["numb"]);
                }
            }
            int health = player["health"].ToObject<int>();
            Player nowPlayer = PlayerSource.GetPlayers()[playerId];
            if(nowPlayer is not null)
            {
                if (nowPlayer.Health- health > 5)
                {
                    // Play hurt audio
                    _as.PlayOneShot(_audioClipDict["Hurt"]);
                }
                if (health < 1 && !nowPlayer.IsDead)
                {
                    _as.PlayOneShot(_audioClipDict["Die"]);
                }
            }
            PlayerSource.UpdatePlayer(
                playerId,
                health,
                player["armor"].ToString() switch
                {
                    "NO_ARMOR" => ArmorTypes.NoArmor,
                    "PRIMARY_ARMOR" => ArmorTypes.PrimaryArmor,
                    "PREMIUM_ARMOR" => ArmorTypes.PremiumArmor,
                    _ => ArmorTypes.NoArmor
                },
                player["speed"].ToObject<float>(),
                player["firearm"]["name"].ToString() switch
                {
                    "S686" => FirearmTypes.S686,
                    "AWM" => FirearmTypes.Awm,
                    "VECTOR" => FirearmTypes.Vector,
                    "FISTS" => FirearmTypes.Fists,
                    "M16" => FirearmTypes.M16,
                    _ => FirearmTypes.Fists,
                },
                inventory,
                playerPosition,
                (float)player["firearm"]["distance"]
            );
            if (players.Count <= 2 || (_observe.cameraStatus == Observe.CameraStatus.player && playerId == _observe.PlayerNumber))
            {
                infoString += $"<Player {(PlayerSource.GetPlayers()[playerId].Name.Length <= 6 ? PlayerSource.GetPlayers()[playerId].Name : PlayerSource.GetPlayers()[playerId].Name.Substring(0, 6) + "...") }> Health {health}\nPosition ({playerPosition.x:F2}, {playerPosition.y.ToString("F2")})\nInventory: ";
                foreach(KeyValuePair<string, int> keyValue in inventory)
                {
                    infoString += $"{keyValue.Key} {keyValue.Value}; ";
                }
                infoString += "\n";
                
                infoString += $"Armor: {player["armor"]}\n";
                infoString += $"Firearm: {player["firearm"]["name"]}\n";
                if (player.ContainsKey("firearmsPool"))
                {
                    infoString += $"Firearm Pool: ";
                    foreach (JObject firearm in (JArray)player["firearmsPool"])
                    {
                        infoString += $"{firearm["name"]}  ";
                    }
                    infoString += "\n";
                }
                infoString += $"Speed: {player["speed"]}\n";
                infoString += $"-----------------------\n";
                infoString += $"-----------------------\n";
            }
        }
        _infoText.text = infoString;
    }
    private void UpdateCircle(Vector2 newPos, float newRadius)
    {
        _poisonousCircle.transform.position = new Vector3(newPos.x, _poisonousCircle.transform.position.y, newPos.y);
        ParticleSystem.ShapeModule shape = _poisonousCircle.shape;
        shape.radius = newRadius;

        Light spotLight = _spotLight.GetComponent<Light>();
        spotLight.spotAngle = spotLight.innerSpotAngle = Mathf.Rad2Deg*(2 * Mathf.Atan2(newRadius, _spotLight.transform.position.y));
        _spotLight.transform.position = new Vector3(newPos.x, _spotLight.transform.position.y, newPos.y);
    }
    private void AfterPlayerPickUpEvent(JObject eventJson)
    {
        int playerId = eventJson["data"]["playerId"].ToObject<int>();
        Player player = PlayerSource.GetPlayers()[playerId];
        string itemName = eventJson["data"]["turgetSupply"].ToString();
        if (itemInstances.ContainsKey(itemName))
        {
            foreach (GameObject itemInstance in itemInstances[itemName])
            {
                if ((int)(itemInstance.transform.position.x) == (int)player.PlayerPosition.x && (int)(itemInstance.transform.position.z) == (int)player.PlayerPosition.y)
                {
                    Debug.Log("Found obj picked up!");
                    Destroy(itemInstance);
                    itemInstances[itemName].Remove(itemInstance);
                    if (itemInstances[itemName].Count == 0)
                    {
                        itemInstances.Remove(itemName);
                    }
                    _as.PlayOneShot(_audioClipDict["Pickup"]);
                    break;
                }
            }
        }
    }

    private void AfterPlayerAbandonEvent(JObject eventJson)
    {
        int playerId = eventJson["data"]["playerId"].ToObject<int>();
        Player player = PlayerSource.GetPlayers()[playerId];
        JToken abandonedSupplies = (JToken)eventJson["data"]["abandonedSupplies"];

        string itemName = abandonedSupplies.ToString();
        //int itemCount = (int)abandonedSupplies["count"];
        Vector3 position = new Vector3((int)player.PlayerPosition.x + 0.5f, 0.1f, (int)player.PlayerPosition.y + 0.5f);
        //for (int i = 0; i < itemCount; i++)
        //{
        GameObject supplyPrefab = _propDict.ContainsKey(itemName) ? _propDict[itemName] : null;
        if (supplyPrefab != null)
        {
            GameObject newSupply = Instantiate(supplyPrefab, position, Quaternion.identity, _supplyParent.transform);
            newSupply.transform.Rotate(0, UnityEngine.Random.Range(0, 360), 0);
            if (!itemInstances.ContainsKey(itemName) || itemInstances[itemName] == null)
            {
                itemInstances[itemName] = new List<GameObject>();
            }
            itemInstances[itemName].Add(newSupply);
        }
        //}
    }

    private void AfterPlayerAttackEvent(JObject eventJson)
    {
        int playerId = eventJson["data"]["playerId"].ToObject<int>();
        Position targetPosition = new Position((float)eventJson["data"]["turgetPosition"]["x"], (float)eventJson["data"]["turgetPosition"]["y"]);
        Player player = PlayerSource.GetPlayers()[playerId];

        if (eventJson["data"]["range"] == null)
        {
            Debug.Log("Range is null!");
            player.Attack(targetPosition, player.FirearmRange);
        }
        else
        {
            player.Attack(targetPosition, eventJson["data"]["range"].ToObject<float>());
        }

        string firearmString = player.Firearm switch
        {
            FirearmTypes.S686 => "S686",
            FirearmTypes.Awm => "AWM",
            FirearmTypes.Vector => "VECTOR",
            FirearmTypes.Fists => "FISTS",
            FirearmTypes.M16 => "M16",
            _ => "M16"
        };
        // Play sound
        _as.PlayOneShot(_audioClipDict[firearmString]);
    }

    private void AfterPlayerUseMedicineEvent(JObject eventJson)
    {
        int playerId = eventJson["data"]["playerId"].ToObject<int>();
        _as.PlayOneShot(_audioClipDict["Heal"]);
        PlayerSource.GetPlayers()[playerId].UseMedicine();
    }

    private void AfterPlayerSwitchArmEvent(JObject eventJson)
    {
        int playerId = eventJson["data"]["playerId"].ToObject<int>();
    }

    private void AfterPlayerUseGrenadeEvent(JObject eventJson)
    {
        int playerId = eventJson["data"]["playerId"].ToObject<int>();
        _as.PlayOneShot(_audioClipDict["FireInTheHole"]);
        float x=(float) eventJson["data"]["turgetPosition"]["x"];
        float y=(float) eventJson["data"]["turgetPosition"]["y"];
        // TODO:
        GameObject beamPrefab = Instantiate(_grenadeBeamPrefab);
        Vector3 endPoint = new Vector3(x, 0, y) ;
        beamPrefab.transform.position = endPoint;
        beamPrefab.GetComponentInChildren<MeshRenderer>().material.color = PlayerSource.GetPlayers()[playerId].playerColor;
        beamPrefab.GetComponent<BeamAnimations>().Blink(5.0f);
    }

    private void AfterGrenadeExplosionEvent(JObject eventJson)
    {
        double x, y;
        if ((JToken)eventJson["data"] != null)
        {
            x = (double)eventJson["data"]["explodePosition"]["x"];
            y = (double)eventJson["data"]["explodePosition"]["y"];
        }
        else
        {
            x = (double)eventJson["explodePosition"]["x"];
            y = (double)eventJson["explodePosition"]["y"];
        }
        // Instantiate Prefab
        GameObject prefab = Instantiate(_grenadeExplosionPrefab);
        prefab.transform.position = new Vector3((float)x, 0.2f, (float)y);
        prefab.GetComponent<ParticleSystem>().Play();
        prefab.AddComponent<AutoDelete>();

        _as.PlayOneShot(_audioClipDict["Grenade"]);

    }

    #endregion



    private void UpdateTick()
    {
        //try
        //{
        if (_recordInfo.RecordSpeed < 0)
        {
            return;
        }

        int recordTick = _recordInfo.NowTick;

        while (recordTick == _recordInfo.NowTick) {

            if (_recordArray[_recordInfo.NowRecordNum].Value<string>("currentTicks") != null &&
                _recordArray[_recordInfo.NowRecordNum]["messageType"].ToString() == "COMPETITION_UPDATE")
            {

                //Debug.Log(_recordArray[_recordInfo.NowRecordNum]["currentTicks"].ToString());
                _recordInfo.NowTick = (int)(_recordArray[_recordInfo.NowRecordNum]["currentTicks"]);
                //if (_recordInfo.NowTick < 200)
                //{
                //_recordInfo.NowRecordNum++;
                //    continue;
                //}
                UpdatePlayers((JArray)_recordArray[_recordInfo.NowRecordNum]["data"]["players"]);

                _currentTickText.text = $"{_recordInfo.NowTick}";
                JArray events = (JArray)_recordArray[_recordInfo.NowRecordNum]["data"]["events"];
                if (events != null)
                {
                    foreach (JObject eventJson in events)
                    {
                        JObject eventJsonInfo = (JObject)eventJson["Json"];
                        switch(eventJson["Json"]["eventType"].ToString())
                        {
                        case "PLAYER_ATTACK":
                            AfterPlayerAttackEvent(eventJsonInfo);
                            break;
                        case "PLAYER_SWITCH_ARM":
                            AfterPlayerSwitchArmEvent(eventJsonInfo);
                            break;
                        case "PLAYER_PICK_UP":
                            AfterPlayerPickUpEvent(eventJsonInfo);
                            break;
                        case "PLAYER_USE_MEDICINE":
                            AfterPlayerUseMedicineEvent(eventJsonInfo);
                            break;
                        case "PLAYER_USE_GRENADE":
                            AfterPlayerUseGrenadeEvent(eventJsonInfo);
                            break;
                        case "PLAYER_ABANDON":
                            AfterPlayerAbandonEvent(eventJsonInfo);
                            break;
                        case "PLAYER_PREPARE":
                            break;
                        case "GRENADE_EXPLODE":
                            AfterGrenadeExplosionEvent(eventJsonInfo);
                            break;
                        default:
                            break;
                        }
                    }
                }

            }
            if (_recordArray[_recordInfo.NowRecordNum]["messageType"]!=null && _recordArray[_recordInfo.NowRecordNum]["messageType"].ToString() == "SAFE_ZONE")
            {
                UpdateCircle(new Vector2((float)_recordArray[_recordInfo.NowRecordNum]["data"]["center"]["x"], (float)_recordArray[_recordInfo.NowRecordNum]["data"]["center"]["y"]),
                    (float)_recordArray[_recordInfo.NowRecordNum]["data"]["radius"]);
            }

            if (_recordArray[_recordInfo.NowRecordNum]["eventType"] != null && _recordArray[_recordInfo.NowRecordNum]["eventType"].ToString() == "GRENADE_EXPLODE")
            {
                AfterGrenadeExplosionEvent((JObject)_recordArray[_recordInfo.NowRecordNum]);
            }
            _recordInfo.NowRecordNum++;
        }
    }

    private void FixedUpdate()
    {
        if (!(_recordInfo.NowPlayState == PlayState.Play && _recordInfo.NowTick < _recordInfo.MaxTick))
        {
            return;
        }

        if ((float)(System.DateTime.Now.Ticks - _recordInfo.NowTime)/1e7 > _recordInfo.NowFrameTime)
        {
            _recordInfo.NowTime = _recordInfo.NowTime + (long)(_recordInfo.NowFrameTime*1e7);
            UpdateTick();
            _recordInfo.NowDeltaTime = 0;
        }
    }
}
