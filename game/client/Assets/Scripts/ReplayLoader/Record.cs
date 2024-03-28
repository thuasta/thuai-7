using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
using Thubg.Sdk;
using System.Data;
using Mono.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Record : MonoBehaviour
{
    public enum PlayState
    {
        Prepare,
        Play,
        Pause,
        End,
        Jump
    }

    public class RecordInfo
    {
        // 20 frame per second
        public const float FrameTime = 0.05f;
        public PlayState NowPlayState = PlayState.Pause;
        public int NowTick = 0;
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
        public float NowFrameTime
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
            //this.RecordSpeed = 1f;
            NowTick = 0;
            NowRecordNum = 0;
            JumpTargetTick = int.MaxValue;
        }
    }

    // meta info
    private readonly RecordInfo _recordInfo;

    // GUI
    private readonly Button _stopButton;
    private readonly Button _replayButton;
    private readonly Slider _recordSpeedSlider;
    private readonly TMP_Text _recordSpeedText;
    private readonly float _recordSpeedSliderMinValue;
    private readonly float _recordSpeedSliderMaxValue;
    private readonly Slider _processSlider;
    private readonly TMP_Text _jumpTargetTickText;
    private readonly TMP_Text _maxTickText;

    // database
    private readonly Sqlite _dbManager;
    private IDataReader _dataReader;

    // game status
    private Map _map;
    private readonly List<Supply> _supplies;
    private readonly JArray _recordArray;
    private void LoadMapData()
    {
        _dataReader = _dbManager.ReadFromDatabase("Map");
        while (_dataReader.Read())
        {
            _map = JsonConvert.DeserializeObject<Map>(_dataReader.GetString(0));
        }
    }
    private void LoadRecordData()
    {
        _dataReader = _dbManager.ReadFromDatabase("CompetitionUpdate");
        while (_dataReader.Read())
        {
            _recordArray.Add(JObject.Parse(_dataReader.GetString(1)));
        }
        _dataReader.Close();
    }

    private void UpdateTick()
    {
        try
        {
            if (_recordInfo.RecordSpeed > 0)
            {
                
            }
        }
        catch
        {

        }
    }

    private void Update()
    {
        if ((_recordInfo.NowPlayState == PlayState.Play && _recordInfo.NowRecordNum < _recordInfo.MaxTick) || (_recordInfo.NowPlayState == PlayState.Jump))
        {
            if (_recordInfo.NowDeltaTime > _recordInfo.NowFrameTime || _recordInfo.NowPlayState == PlayState.Jump)
            {
                UpdateTick();
                _recordInfo.NowDeltaTime = 0;
            }
            _recordInfo.NowDeltaTime += Time.deltaTime;
        }
    }
}