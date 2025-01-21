using System.Collections.Generic;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [Header("AudioSource")]
    [Tooltip("BGM‚ğÄ¶‚·‚éAudioSource")]
    [SerializeField] AudioSource _audioBgm;
    [Tooltip("SE‚ğÄ¶‚·‚éAudioSource")]
    [SerializeField] AudioSource _audioSe;

    [Space]

    [Header("AudioClip")]
    [Tooltip("BGM")]
    [SerializeField] List<BgmSoundData> _bgmSoundDatas;
    [Tooltip("SE")]
    [SerializeField] List<SeSoundData> _seSoundDatas;

    [SerializeField]
    float _masterVolume = 1;
    [SerializeField]
    float _bgmMasterVolume = 1;
    [SerializeField]
    float _seMasterVolume = 1;

    /// <summary>
    /// BGM‚ğÄ¶‚·‚é‚æ‚¤‚É‚·‚é
    /// </summary>
    /// <param name="bgm">Ä¶‚µ‚½‚¢BGM‚Ìenum</param>
    public void PlayBGM(BgmSoundData.BGM bgm)
    {
        int index = (int)bgm;
        if (_audioBgm == null || _bgmSoundDatas.Count <= index) { return; }
        BgmSoundData data = _bgmSoundDatas[index];
        _audioBgm.clip = data._audioClip;

        Debug.Log(bgm);

        //‰¹—Ê‚Ì’²ß
        _audioBgm.volume = data._volume * _bgmMasterVolume * _masterVolume;
        _audioBgm.Play();

        Debug.Log(bgm);
    }

    public void StopBGM()
    {
        _audioBgm.Stop();
    }

    /// <summary>
    /// SE‚ğÄ¶‚·‚é‚æ‚¤‚É‚·‚é
    /// </summary>
    /// <param name="se">Ä¶‚µ‚½‚¢SE‚Ìenum</param>
    public void PlaySE(SeSoundData.SE se)
    {
        int index = (int)se;
        if (_seSoundDatas.Count <= index || _audioSe == null) { return; }
        SeSoundData data = _seSoundDatas[index];

        Debug.Log(se);

        //‰¹—Ê‚Ì’²ß
        _audioSe.volume = data.Volume * _seMasterVolume * _masterVolume;
        _audioSe.PlayOneShot(data.AudioClip);
    }



    [System.Serializable]
    public class BgmSoundData
    {
        public enum BGM
        {
            Title,
            Game,
            GameOver,
            Clear,
        }

        public BGM _bgm;
        public AudioClip _audioClip;
        [Range(0f, 1f)]
        public float _volume = 1f;
    }


    [System.Serializable]
    public class SeSoundData
    {
        public enum SE
        {
            Click,
            Flag,
            Open,
        }

        public SE Se;
        public AudioClip AudioClip;
        [Range(0, 1)]
        public float Volume = 1f;
    }
}
