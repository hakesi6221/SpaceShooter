using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-90)]
public class SoundManager : MonoBehaviour
{
    #region privateオブジェクト
    [SerializeField, Header("BGMリスト")]
    private List<AudioInfomation> _bgmList = new List<AudioInfomation>();

    [SerializeField, Header("SEリスト")]
    private List<AudioInfomation> _seList = new List<AudioInfomation>();

    [SerializeField, Header("ボイスリスト")]
    private List<AudioInfomation> _voiceList = new List<AudioInfomation>();

    [SerializeField, Header("その他の音リスト")]
    private List<AudioInfomation> _othersList = new List<AudioInfomation>();

    [SerializeField, Header("親ミキサー")]
    private AudioMixer _mixer;

    [SerializeField, Header("BGMミキサー")]
    private AudioMixerGroup _bgmMixier;

    [SerializeField, Header("SEミキサー")]
    private AudioMixerGroup _seMixier;

    [SerializeField, Header("Voiceミキサー")]
    private AudioMixerGroup _voiceMixier;

    [SerializeField, Header("その他ミキサー")]
    private AudioMixerGroup _othersMixier;

    [SerializeField, Header("デフォルトフェード時間"), Range(0.0f, 5.0f)]
    private float _defaultFadeRate = 0.0f;

    private List<AudioSource> _bgmSource = new List<AudioSource>();
    private List<AudioSource> _seSource = new List<AudioSource>();
    private List<AudioSource> _voiceSource = new List<AudioSource>();
    private List<AudioSource> _othersSource = new List<AudioSource>();

    // Start is called before the first frame update
    void Start()
    {
        // 各音のリストを初期化
        _bgmSource = new List<AudioSource>();
        _seSource = new List<AudioSource>();
        _voiceSource = new List<AudioSource>();
        _othersSource = new List<AudioSource>();
        foreach (AudioInfomation audio in _bgmList)
        {
            _bgmSource.Add(CreateNewAudioSource(audio));
            _bgmSource.LastOrDefault().outputAudioMixerGroup = _bgmMixier;
        }
        foreach (AudioInfomation audio in _seList)
        {
            _seSource.Add(CreateNewAudioSource(audio));
            _seSource.LastOrDefault().outputAudioMixerGroup = _seMixier;
        }
        foreach (AudioInfomation audio in _voiceList)
        {
            _voiceSource.Add(CreateNewAudioSource(audio));
            _voiceSource.LastOrDefault().outputAudioMixerGroup = _voiceMixier;
        }
        foreach (AudioInfomation audio in _othersList)
        {
            _othersSource.Add(CreateNewAudioSource(audio));
            _othersSource.LastOrDefault().outputAudioMixerGroup = _othersMixier;
        }
    }

    private AudioSource CreateNewAudioSource(AudioInfomation audio)
    {
        AudioSource newSource = gameObject.AddComponent<AudioSource>();

        switch (audio.Type)
        {
            case AudioType.BGM:
                newSource.outputAudioMixerGroup = _bgmMixier;
                break;
            case AudioType.SE:
                newSource.outputAudioMixerGroup = _seMixier;
                break;
            case AudioType.VOICE:
                newSource.outputAudioMixerGroup = _voiceMixier;
                break;
            case AudioType.OTHERS:
                newSource.outputAudioMixerGroup = _othersMixier;
                break;
        }

        return newSource;
    }

    /// <summary>
    /// サウンド終了時の後始末処理
    /// </summary>
    /// <param name="source">監視したいAudioSource</param>
    /// <returns></returns>
    private IEnumerator FinishSoundProcess(AudioSource source)
    {
        if (source == null) yield break;

        yield return new WaitUntil(() => !source.isPlaying);

        source.clip = null;
        source.volume = 0f;
        source.loop = false;
        source.time = 0f;
    }

    /// <summary>
    /// サウンドを実際に鳴らす
    /// </summary>
    /// <param name="source">鳴らすソース</param>
    /// <param name="audio">鳴らす音の情報</param>
    private void OnPlaySound(AudioSource source, AudioInfomation audio)
    {
        source.clip = audio.Clip;
        source.volume = audio.Volume;
        source.loop = audio.Loop;
        source.time = audio.Ofset;

        source.Play();
        StartCoroutine(FinishSoundProcess(source));
    }

    /// <summary>
    /// サウンドの一時停止解除
    /// </summary>
    /// <param name="source">解除したいソース</param>
    private void OnUnPauseSound(AudioSource source)
    {
        source.UnPause();
    }

    /// <summary>
    /// サウンドの一時停止解除
    /// フェードインあり
    /// </summary>
    /// <param name="source">解除したいソース</param>
    /// <param name="endVolume">最終的なボリューム</param>
    /// <param name="fadeTime">フェードの時間</param>
    private void OnUnPauseSoundWithFadeIn(AudioSource source, float endVolume, float fadeTime)
    {
        source.UnPause();
        StartCoroutine(FadeMoveSound(source, fadeTime, endVolume));
    }

    /// <summary>
    /// サウンドを実際に鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="source">鳴らすソース</param>
    /// <param name="audio">鳴らす音の情報</param>
    /// <param name="startVolume">最初のボリューム</param>
    /// <param name="endVolume">最終的なボリューム</param>
    /// <param name="fadeTime">フェードの時間</param>
    private void OnPlaySoundWithFadeIn(AudioSource source, AudioInfomation audio, float startVolume, float endVolume, float fadeTime)
    {
        source.clip = audio.Clip;
        source.volume = startVolume;
        source.loop = audio.Loop;
        source.time = audio.Ofset;

        source.Play();
        StartCoroutine(FadeMoveSound(audio, fadeTime, endVolume));
        StartCoroutine(FinishSoundProcess(source));
    }

    /// <summary>
    /// サウンドを直接止める
    /// </summary>
    /// <param name="source">鳴らすソース</param>
    private void OnStopSound(AudioSource source)
    {
        source.Stop();
        source.clip = null;
        source.volume = 0f;
        source.loop = false;
        source.time = 0f;
    }

    /// <summary>
    /// サウンドを直接止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="source">止めるソース</param>
    /// <param name="audio">止める音の情報</param>
    /// <param name="endVolume">最終的なボリューム</param>
    /// <param name="fadeTime">フェードの時間</param>
    private IEnumerator OnStopSoundWithFadeOut(AudioSource source, AudioInfomation audio, float endVolume, float fadeTime)
    {
        yield return StartCoroutine(FadeMoveSound(audio, fadeTime, endVolume));

        source.Stop();
        source.clip = null;
        source.volume = 0f;
        source.loop = false;
        source.time = 0f;
    }

    /// <summary>
    /// サウンドの一時停止
    /// </summary>
    /// <param name="source">解除したいソース</param>
    private void OnPauseSound(AudioSource source)
    {
        source.Pause();
    }

    /// <summary>
    /// サウンドの一時停止
    /// フェードアウトあり
    /// </summary>
    /// <param name="source">解除したいソース</param>
    /// <param name="endVolume">最終的なボリューム</param>
    /// <param name="fadeTime">フェードの時間</param>
    private IEnumerator OnPauseSoundWithFadeOut(AudioSource source, float endVolume, float fadeTime)
    {
        yield return StartCoroutine(FadeMoveSound(source, fadeTime, endVolume));
        source.Pause();
    }

    /// <summary>
    /// サウンドのタイプで所属しているAudioSourceのリストを特定
    /// </summary>
    /// <param name="type">調べたい音のAudioType</param>
    /// <returns></returns>
    private List<AudioSource> SearchSourceListByAudioType(AudioType type)
    {
        List<AudioSource> sources = new List<AudioSource>();

        // 指定された音のタイプによってソースのリストを決定
        switch (type)
        {
            case AudioType.BGM:
                sources = _bgmSource;
                break;
            case AudioType.SE:
                sources = _seSource;
                break;
            case AudioType.VOICE:
                sources = _voiceSource;
                break;
            case AudioType.OTHERS:
                sources = _othersSource;
                break;
            default:
                break;
        }

        return sources;
    }

    /// <summary>
    /// 現在再生中ではないAudioSourceをリストの中から検索
    /// すべて埋まっていた場合、新しいものを作る
    /// </summary>
    /// <param name="sources">検索したいAudioSourceのリスト</param>
    /// <returns></returns>
    private AudioSource SearchEmptySource(List<AudioSource> sources)
    {
        foreach (AudioSource source in sources)
        {
            if (!source.isPlaying)
                return source;
        }

        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        sources.Add(newSource);

        return newSource;
    }

    /// <summary>
    /// 指定したAudioClipを再生しているAudioSourceをリストの中から検索
    /// </summary>
    /// <param name="sources">検索したいAudioSourceのリスト</param>
    /// <param name="clip">検索したいAudioClip</param>
    /// <returns></returns>
    private AudioSource SearchSourceByClip(List<AudioSource> sources, AudioClip clip)
    {
        foreach (AudioSource source in sources)
        {
            if (source.clip == clip)
                return source;
        }

        return null;
    }

    /// <summary>
    /// 指定したサウンドをフェードして音量を移動
    /// </summary>
    /// <param name="audio">フェードしたい音</param>
    /// <param name="fadeOutSec">フェード時間</param>
    /// <param name="volume">フェード後の音量</param>
    /// <returns></returns>
    private IEnumerator FadeMoveSound(AudioInfomation audio, float fadeOutSec, float volume)
    {
        if (audio == null) yield break;

        // 属しているAudioSourceを検索
        List<AudioSource> sources = SearchSourceListByAudioType(audio.Type);
        if (sources == null) yield break;
        AudioSource source = SearchSourceByClip(sources, audio.Clip);
        if (source == null) yield break;

        // 変化後の音量がもとと同じ、必要ないので
        if (volume == source.volume) yield break;

        if (fadeOutSec > 0f)
        {
            float timeCnt = 0;
            float startVolume = source.volume;
            float endVolume = volume;
            while (timeCnt <= fadeOutSec)
            {
                timeCnt += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, endVolume, timeCnt / fadeOutSec);
                yield return null;
            }
        }

        source.volume = volume;
    }

    /// <summary>
    /// 指定したサウンドをフェードして音量を移動
    /// </summary>
    /// <param name="audio">フェードしたい音</param>
    /// <param name="fadeOutSec">フェード時間</param>
    /// <param name="volume">フェード後の音量</param>
    /// <returns></returns>
    private IEnumerator FadeMoveSound(AudioSource source, float fadeOutSec, float volume)
    {
        if (source == null) yield break;

        // 変化後の音量がもとと同じ、必要ないので
        if (volume == source.volume) yield break;

        if (fadeOutSec > 0f)
        {
            float timeCnt = 0;
            float startVolume = source.volume;
            float endVolume = volume;
            while (timeCnt <= fadeOutSec)
            {
                timeCnt += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, endVolume, timeCnt / fadeOutSec);
                yield return null;
            }
        }

        source.volume = volume;
    }
    #endregion

    #region パラメータ関係

    /// <summary>
    /// BGMのボリュームを設定
    /// </summary>
    /// <param name="volume">新しいボリューム</param>
    public void SetBGMVolume(float volume)
    {
        _mixer.SetFloat("BGM", volume);
    }
    /// <summary>
    /// BGMのボリューム
    /// </summary>
    public float GetBGMVolume
    {
        get
        {
            float volume = 0f;
            if (_mixer.GetFloat("BGM", out volume))
                return volume;
            else
                return -1f;
        }
    }

    /// <summary>
    /// SEのボリュームを設定
    /// </summary>
    /// <param name="volume">新しいボリューム</param>
    public void SetSEVolume(float volume)
    {
        _mixer.SetFloat("SE", volume);
    }
    /// <summary>
    /// SEのボリューム
    /// </summary>
    public float GetSEVolume
    {
        get
        {
            float volume = 0f;
            if (_mixer.GetFloat("SE", out volume))
                return volume;
            else
                return -1f;
        }
    }

    /// <summary>
    /// ボイスのボリュームを設定
    /// </summary>
    /// <param name="volume">新しいボリューム</param>
    public void SetVoiceVolume(float volume)
    {
        _mixer.SetFloat("Voice", volume);
    }
    /// <summary>
    /// ボイスのボリューム
    /// </summary>
    public float GetVoiceVolume
    {
        get
        {
            float volume = 0f;
            if (_mixer.GetFloat("Voice", out volume))
                return volume;
            else
                return -1f;
        }
    }

    /// <summary>
    /// その他のボリュームを設定
    /// </summary>
    /// <param name="volume">新しいボリューム</param>
    public void SetOthersVolume(float volume)
    {
        _mixer.SetFloat("Others", volume);
    }
    /// <summary>
    /// その他のボリューム
    /// </summary>
    public float GetOthersVolume
    {
        get
        {
            float volume = 0f;
            if (_mixer.GetFloat("Others", out volume))
                return volume;
            else
                return -1f;
        }
    }

    /// <summary>
    /// BGMミュート解除
    /// </summary>
    public void OnBGM()
    {
        foreach (AudioSource source in _bgmSource)
        {
            source.mute = false;
        }
    }
    /// <summary>
    /// BGMミュート
    /// </summary>
    public void OffBGM()
    {
        foreach (AudioSource source in _bgmSource)
        {
            source.mute = true;
        }
    }

    /// <summary>
    /// SEミュート解除
    /// </summary>
    public void OnSE()
    {
        foreach (AudioSource source in _seSource)
        {
            source.mute = false;
        }
    }
    /// <summary>
    /// SEミュート
    /// </summary>
    public void OffSE()
    {
        foreach (AudioSource source in _seSource)
        {
            source.mute = true;
        }
    }

    /// <summary>
    /// Voiceミュート解除
    /// </summary>
    public void OnVoice()
    {
        foreach (AudioSource source in _voiceSource)
        {
            source.mute = false;
        }
    }
    /// <summary>
    /// Voiceミュート
    /// </summary>
    public void OffVoice()
    {
        foreach (AudioSource source in _voiceSource)
        {
            source.mute = true;
        }
    }

    /// <summary>
    /// その他ミュート解除
    /// </summary>
    public void OnOthers()
    {
        foreach (AudioSource source in _othersSource)
        {
            source.mute = false;
        }
    }
    /// <summary>
    /// その他ミュート
    /// </summary>
    public void OffOthers()
    {
        foreach (AudioSource source in _othersSource)
        {
            source.mute = true;
        }
    }
    #endregion

    #region BGM操作
    /// <summary>
    /// 指定したインデックスのBGMを鳴らす
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void PlayBGM(int index)   // 効果音を鳴らす(単発)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_bgmSource);
        if (source == null) return;

        OnPlaySound(source, audio);
    }

    /// <summary>
    /// 指定したインデックスのBGMを鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void PlayBGMWithFadeIn(int index)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_bgmSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, 0f, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのBGMを鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void PlayBGMWithFadeIn(int index, float startVolume, float endVolume, float fadeInSec)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_bgmSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, startVolume, endVolume, fadeInSec);
    }

    /// <summary>
    /// 指定したインデックスBGMを止める
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void StopBGM(int index)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        OnStopSound(source);
    }

    /// <summary>
    /// 指定したインデックスBGMを止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void StopBGMWithFadeOut(int index)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスBGMを止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void StopBGMWithFadeOut(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのBGMを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void PauseBGM(int index)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        OnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのBGMを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void PauseBGMWithFadeOut(int index)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのBGMを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void PauseBGMWithFadeOut(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, endVolume, fadeOutTime));
    }

    /// <summary>
    /// 指定したインデックスのBGMを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void UnPauseBGM(int index)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのBGMを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void UnPauseBGMWithFadeIn(int index)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのBGMを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">BGMのインデックス</param>
    public void UnPauseBGMWithFadeIn(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, endVolume, fadeOutTime);
    }

    /// <summary>
    /// 指定したBGMのボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいBGMのインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeBGMByIndex(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, fadeOutSec, endVolume));
    }

    /// <summary>
    /// 指定したBGMのボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいBGMのインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeBGMByIndex(int index, float endVolume)
    {
        if (index < 0 || _bgmList.Count <= index) return;
        AudioInfomation audio = _bgmList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_bgmSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, _defaultFadeRate, endVolume));
    }
    #endregion

    #region SE操作
    /// <summary>
    /// 指定したインデックスのSEを鳴らす
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void PlaySE(int index)   // 効果音を鳴らす(単発)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_seSource);
        if (source == null) return;

        OnPlaySound(source, audio);
    }

    /// <summary>
    /// 指定したインデックスのSEを鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void PlaySEWithFadeIn(int index)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_seSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, 0f, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのSEを鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void PlaySEWithFadeIn(int index, float startVolume, float endVolume, float fadeInSec)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_seSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, startVolume, endVolume, fadeInSec);
    }

    /// <summary>
    /// 指定したインデックスSEを止める
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void StopSE(int index)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        OnStopSound(source);
    }

    /// <summary>
    /// 指定したインデックスSEを止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void StopSEWithFadeOut(int index)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスSEを止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void StopSEWithFadeOut(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのSEを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void PauseSE(int index)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        OnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのSEを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void PauseSEWithFadeOut(int index)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのSEを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void PauseSEWithFadeOut(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, endVolume, fadeOutTime));
    }

    /// <summary>
    /// 指定したインデックスのSEを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void UnPauseSE(int index)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのSEを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void UnPauseSEWithFadeIn(int index)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのSEを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">SEのインデックス</param>
    public void UnPauseSEWithFadeIn(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, endVolume, fadeOutTime);
    }

    /// <summary>
    /// 指定したSEのボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいSEのインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeSEByIndex(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, fadeOutSec, endVolume));
    }

    /// <summary>
    /// 指定したSEのボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいSEのインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeSEByIndex(int index, float endVolume)
    {
        if (index < 0 || _seList.Count <= index) return;
        AudioInfomation audio = _seList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_seSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, _defaultFadeRate, endVolume));
    }
    #endregion

    #region VOICE操作
    /// <summary>
    /// 指定したインデックスのVoiceを鳴らす
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void PlayVoice(int index)   // 効果音を鳴らす(単発)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_voiceSource);
        if (source == null) return;

        OnPlaySound(source, audio);
    }

    /// <summary>
    /// 指定したインデックスのVoiceを鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void PlayVoiceWithFadeIn(int index)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_voiceSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, 0f, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのVoiceを鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void PlayVoiceWithFadeIn(int index, float startVolume, float endVolume, float fadeInSec)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_voiceSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, startVolume, endVolume, fadeInSec);
    }

    /// <summary>
    /// 指定したインデックスVoiceを止める
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void StopVoice(int index)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        OnStopSound(source);
    }

    /// <summary>
    /// 指定したインデックスVoiceを止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void StopVoiceWithFadeOut(int index)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスVoiceを止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void StopVoiceWithFadeOut(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのVoiceを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void PauseVoice(int index)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        OnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのVoiceを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void PauseVoiceWithFadeOut(int index)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのVoiceを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void PauseVoiceWithFadeOut(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, endVolume, fadeOutTime));
    }

    /// <summary>
    /// 指定したインデックスのVoiceを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void UnPauseVoice(int index)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのVoiceを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void UnPauseVoiceWithFadeIn(int index)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのVoiceを一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">Voiceのインデックス</param>
    public void UnPauseVoiceWithFadeIn(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, endVolume, fadeOutTime);
    }

    /// <summary>
    /// 指定したVoiceのボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいVoiceのインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeVoiceByIndex(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, fadeOutSec, endVolume));
    }

    /// <summary>
    /// 指定したVoiceのボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいVoiceのインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeVoiceByIndex(int index, float endVolume)
    {
        if (index < 0 || _voiceList.Count <= index) return;
        AudioInfomation audio = _voiceList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_voiceSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, _defaultFadeRate, endVolume));
    }
    #endregion

    #region OTHERS操作
    /// <summary>
    /// 指定したインデックスのその他の音を鳴らす
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void PlayOthers(int index)   // 効果音を鳴らす(単発)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_othersSource);
        if (source == null) return;

        OnPlaySound(source, audio);
    }

    /// <summary>
    /// 指定したインデックスのその他の音を鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void PlayOthersWithFadeIn(int index)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_othersSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, 0f, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのその他の音を鳴らす
    /// フェードインあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void PlayOthersWithFadeIn(int index, float startVolume, float endVolume, float fadeInSec)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchEmptySource(_othersSource);
        if (source == null) return;

        OnPlaySoundWithFadeIn(source, audio, startVolume, endVolume, fadeInSec);
    }

    /// <summary>
    /// 指定したインデックスその他の音を止める
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void StopOthers(int index)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        OnStopSound(source);
    }

    /// <summary>
    /// 指定したインデックスその他の音を止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void StopOthersWithFadeOut(int index)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスその他の音を止める
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void StopOthersWithFadeOut(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnStopSoundWithFadeOut(source, audio, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのその他の音を一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void PauseOthers(int index)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        OnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのその他の音を一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void PauseOthersWithFadeOut(int index)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, 0f, _defaultFadeRate));
    }

    /// <summary>
    /// 指定したインデックスのその他の音を一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void PauseOthersWithFadeOut(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(OnPauseSoundWithFadeOut(source, endVolume, fadeOutTime));
    }

    /// <summary>
    /// 指定したインデックスのその他の音を一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void UnPauseOthers(int index)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSound(source);
    }

    /// <summary>
    /// 指定したインデックスのその他の音を一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void UnPauseOthersWithFadeIn(int index)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, audio.Volume, _defaultFadeRate);
    }

    /// <summary>
    /// 指定したインデックスのその他の音を一時停止する
    /// フェードアウトあり
    /// </summary>
    /// <param name="index">その他の音のインデックス</param>
    public void UnPauseOthersWithFadeIn(int index, float endVolume, float fadeOutTime)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        OnUnPauseSoundWithFadeIn(source, endVolume, fadeOutTime);
    }

    /// <summary>
    /// 指定したその他の音のボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいその他の音のインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeOthersByIndex(int index, float endVolume, float fadeOutSec)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, fadeOutSec, endVolume));
    }

    /// <summary>
    /// 指定したその他の音のボリュームをフェードで変更する
    /// </summary>
    /// <param name="index">変更したいその他の音のインデックス</param>
    /// <param name="endVolume">変更先のボリューム</param>
    /// <param name="fadeOutSec">フェードの時間</param>
    public void FadeMoveVolumeOthersByIndex(int index, float endVolume)
    {
        if (index < 0 || _othersList.Count <= index) return;
        AudioInfomation audio = _othersList[index];
        if (audio == null) return;
        AudioSource source = SearchSourceByClip(_othersSource, audio.Clip);
        if (source == null) return;

        StartCoroutine(FadeMoveSound(audio, _defaultFadeRate, endVolume));
    }
    #endregion



    #region シングルトンパターン
    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion
}
