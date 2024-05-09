using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGM : MonoBehaviour
{
    private List<AudioClip> _audios;
    private AudioSource _as;
    private int _num;
    // Start is called before the first frame update
    void Start()
    {
        _num = 0;
        _audios = new()
        {
            Resources.Load<AudioClip>("Music/Sound/BGM/1"),
            Resources.Load<AudioClip>("Music/Sound/BGM/2"),
            Resources.Load<AudioClip>("Music/Sound/BGM/3")
        };
        _as = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_as.isPlaying)
        {
            _as.clip = _audios[_num];
            _as.Play();
            _num += 1;
            if (_num >= _audios.Count)
            {
                _num = 0;
            }
        }
    }
}
