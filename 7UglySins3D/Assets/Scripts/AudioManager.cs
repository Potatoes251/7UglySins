using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

[DefaultExecutionOrder(-100)]
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Lifecycle")]
    public bool persistAcrossScenes = true;

    [Header("Mixer (optional)")]
    public AudioMixer mixer;
    public string masterVolParam = "MasterVol";
    public string musicVolParam = "MusicVol";
    public string sfxVolParam = "SfxVol";

    [Header("SFX Pool")]
    [Min(1)] public int initialPoolSize = 12;
    [Min(1)] public int maxPoolSize = 48;

    [Header("Music")]
    [Min(0f)] public float defaultMusicFade = 0.8f;

    // ---- internals ----
    readonly List<AudioSource> _pool = new();
    readonly List<AudioSource> _activeSfx = new();

    AudioSource _musicA, _musicB;
    bool _usingA = true;
    Coroutine _musicCo;

    // ---------- lifecycle ----------
    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (persistAcrossScenes) DontDestroyOnLoad(gameObject);

        for (int i = 0; i < initialPoolSize; i++) _pool.Add(CreateSfxSource());
        _musicA = CreateMusicSource("MusicA");
        _musicB = CreateMusicSource("MusicB");

        if (mixer)
        {
            SetMixer01(masterVolParam, 1f);
            SetMixer01(musicVolParam, 1f);
            SetMixer01(sfxVolParam, 1f);
        }
    }

    void OnEnable()
    {
        // NOTE: assuming EventManager has StartListening/StopListening/TriggerEvent
        EventManager.StartListening(GameEvent.PlaySfx, OnPlaySfx);
        EventManager.StartListening(GameEvent.StopAllSfx, OnStopAllSfx);
        EventManager.StartListening(GameEvent.PlayMusic, OnPlayMusic);
        EventManager.StartListening(GameEvent.StopMusic, OnStopMusic);
    }

    void OnDisable()
    {
        EventManager.StopListening(GameEvent.PlaySfx, OnPlaySfx);
        EventManager.StopListening(GameEvent.StopAllSfx, OnStopAllSfx);
        EventManager.StopListening(GameEvent.PlayMusic, OnPlayMusic);
        EventManager.StopListening(GameEvent.StopMusic, OnStopMusic);
    }

    void Update()
    {
        // reclaim finished one-shots
        for (int i = _activeSfx.Count - 1; i >= 0; i--)
        {
            var s = _activeSfx[i];
            if (!s.loop && !s.isPlaying)
            {
                s.clip = null;
                _activeSfx.RemoveAt(i);
            }
        }
    }

    // ---------- Event handlers ----------
    void OnPlaySfx(Dictionary<string, object> data)
    {
        if (!TryGet(data, "clip", out AudioClip clip) || clip == null) return;

        // optional params
        Vector3? pos = TryGet(data, "pos", out Vector3 p) ? p : (Vector3?)null;
        Transform follow = TryGet(data, "follow", out Transform f) ? f : null;
        bool loop = TryGet(data, "loop", out bool l) ? l : false;
        float vol = TryGet(data, "volume", out float v) ? Mathf.Clamp01(v) : 1f;
        float pit = TryGet(data, "pitch", out float q) ? Mathf.Clamp(q, 0.5f, 2f) : 1f;
        float sp = TryGet(data, "spatial", out float s) ? Mathf.Clamp01(s) : ((pos.HasValue || follow) ? 1f : 0f);

        var src = GetFreeSource();
        src.clip = clip;
        src.loop = loop;
        src.volume = vol;
        src.pitch = pit;
        src.spatialBlend = sp;

        // 3D settings sane defaults
        if (sp > 0f) { src.minDistance = 1f; src.maxDistance = 30f; src.rolloffMode = AudioRolloffMode.Logarithmic; }

        if (follow) src.transform.position = follow.position;
        else if (pos.HasValue) src.transform.position = pos.Value;
        else src.transform.localPosition = Vector3.zero;

        src.Play();

        if (!_activeSfx.Contains(src)) _activeSfx.Add(src);

        // if following a transform and looping, keep it updated
        if (follow && loop) StartCoroutine(FollowLoop(src, follow));
    }

    void OnStopAllSfx(Dictionary<string, object> _)
    {
        foreach (var s in _activeSfx)
        {
            s.Stop();
            s.clip = null;
        }
        _activeSfx.Clear();
    }

    void OnPlayMusic(Dictionary<string, object> data)
    {
        if (!TryGet(data, "clip", out AudioClip clip) || clip == null) return;

        float fade = TryGet(data, "fade", out float f) ? Mathf.Max(0f, f) : defaultMusicFade;
        float vol = TryGet(data, "volume", out float v) ? Mathf.Clamp01(v) : 1f;
        bool loop = TryGet(data, "loop", out bool l) ? l : true;

        if (_musicCo != null) StopCoroutine(_musicCo);
        _musicCo = StartCoroutine(CorCrossfadeMusic(clip, vol, loop, fade));
    }

    void OnStopMusic(Dictionary<string, object> data)
    {
        float fade = TryGet(data, "fade", out float f) ? Mathf.Max(0f, f) : defaultMusicFade;
        if (_musicCo != null) StopCoroutine(_musicCo);
        _musicCo = StartCoroutine(CorStopMusic(fade));
    }

    // ---------- helpers ----------
    static bool TryGet<T>(Dictionary<string, object> d, string key, out T value)
    {
        value = default;
        if (d == null) return false;
        if (!d.TryGetValue(key, out var obj) || obj == null) return false;
        if (obj is T t) { value = t; return true; }
        return false;
    }

    AudioSource GetFreeSource()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            var s = _pool[i];
            if (!s.isPlaying && s.clip == null) return s;
        }

        if (_pool.Count < maxPoolSize)
        {
            var s = CreateSfxSource();
            _pool.Add(s);
            return s;
        }

        // steal the oldest non-looping
        var steal = _activeSfx.FirstOrDefault(s => !s.loop);
        if (steal != null) { steal.Stop(); steal.clip = null; return steal; }

        return _pool[0];
    }

    AudioSource CreateSfxSource()
    {
        var go = new GameObject("SFX");
        go.transform.SetParent(transform);
        var s = go.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.spatialBlend = 0f;
        s.dopplerLevel = 0f;
        s.rolloffMode = AudioRolloffMode.Logarithmic;
        return s;
    }

    AudioSource CreateMusicSource(string name)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);
        var s = go.AddComponent<AudioSource>();
        s.playOnAwake = false;
        s.loop = true;
        s.spatialBlend = 0f;
        s.dopplerLevel = 0f;
        return s;
    }

    IEnumerator FollowLoop(AudioSource s, Transform t)
    {
        while (s && t && s.isPlaying && s.loop)
        {
            s.transform.position = t.position;
            yield return null;
        }
    }

    IEnumerator CorCrossfadeMusic(AudioClip clip, float targetVol, bool loop, float fade)
    {
        var from = _usingA ? _musicA : _musicB;
        var to = _usingA ? _musicB : _musicA;
        _usingA = !_usingA;

        to.clip = clip;
        to.loop = loop;
        to.volume = 0f;
        to.Play();

        float t = 0f;
        float fromStart = from.isPlaying ? from.volume : 0f;

        while (t < fade)
        {
            t += Time.unscaledDeltaTime;
            float k = fade <= 0f ? 1f : Mathf.Clamp01(t / fade);
            to.volume = targetVol * k;
            from.volume = fromStart * (1f - k);
            yield return null;
        }

        to.volume = targetVol;
        if (from.isPlaying) { from.Stop(); from.clip = null; }
        _musicCo = null;
    }

    IEnumerator CorStopMusic(float fade)
    {
        var src = _usingA ? _musicA : _musicB;
        float start = src.volume, t = 0f;
        while (t < fade)
        {
            t += Time.unscaledDeltaTime;
            float k = fade <= 0f ? 1f : Mathf.Clamp01(t / fade);
            src.volume = start * (1f - k);
            yield return null;
        }
        src.Stop();
        src.clip = null;
        _musicCo = null;
    }

    void SetMixer01(string param, float v01)
    {
        if (!mixer || string.IsNullOrEmpty(param)) return;
        v01 = Mathf.Clamp01(v01);
        float dB = (v01 <= 0.0001f) ? -80f : Mathf.Log10(v01) * 20f;
        mixer.SetFloat(param, dB);
    }
}