using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class AudioOptionsUI : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sliders (0~1)")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Inputs (0~100) - TMP_InputField")]
    [SerializeField] private TMP_InputField masterInput;
    [SerializeField] private TMP_InputField bgmInput;
    [SerializeField] private TMP_InputField sfxInput;

    private const string MASTER_PARAM = "MasterVolume";
    private const string BGM_PARAM = "BGMVolume";
    private const string SFX_PARAM = "SFXVolume";

    private const string PREF_MASTER = "vol_master";
    private const string PREF_BGM = "vol_bgm";
    private const string PREF_SFX = "vol_sfx";

    private const float MIN_LINEAR = 0.0001f;
    private const float MIN_DB = -80f;
    private const float SFX_MAX_DB = -10f;

    private bool _syncing;

    private void Awake()
    {
        if (masterSlider) masterSlider.onValueChanged.AddListener(OnMasterSliderChanged);
        if (bgmSlider) bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        if (sfxSlider) sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);

        if (masterInput) masterInput.onEndEdit.AddListener(OnMasterInputEndEdit);
        if (bgmInput) bgmInput.onEndEdit.AddListener(OnBgmInputEndEdit);
        if (sfxInput) sfxInput.onEndEdit.AddListener(OnSfxInputEndEdit);

        SetupNumericInput(masterInput);
        SetupNumericInput(bgmInput);
        SetupNumericInput(sfxInput);

        LoadAndApplyToMixerOnly();
    }

    private void OnEnable()
    {
        LoadSyncAndApply();
    }

    private void OnDestroy()
    {
        if (masterSlider) masterSlider.onValueChanged.RemoveListener(OnMasterSliderChanged);
        if (bgmSlider) bgmSlider.onValueChanged.RemoveListener(OnBgmSliderChanged);
        if (sfxSlider) sfxSlider.onValueChanged.RemoveListener(OnSfxSliderChanged);

        if (masterInput) masterInput.onEndEdit.RemoveListener(OnMasterInputEndEdit);
        if (bgmInput) bgmInput.onEndEdit.RemoveListener(OnBgmInputEndEdit);
        if (sfxInput) sfxInput.onEndEdit.RemoveListener(OnSfxInputEndEdit);
    }

    private void LoadAndApplyToMixerOnly()
    {
        if (!mixer) return;

        float master = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
        float bgm = PlayerPrefs.GetFloat(PREF_BGM, 1f);
        float sfx = PlayerPrefs.GetFloat(PREF_SFX, 1f);

        ApplyVolume(MASTER_PARAM, master);
        ApplyVolume(BGM_PARAM, bgm);
        ApplyVolume(SFX_PARAM, sfx);
    }

    private void LoadSyncAndApply()
    {
        if (!mixer) return;

        float master = PlayerPrefs.GetFloat(PREF_MASTER, 1f);
        float bgm = PlayerPrefs.GetFloat(PREF_BGM, 1f);
        float sfx = PlayerPrefs.GetFloat(PREF_SFX, 1f);

        SyncAllUI(master, bgm, sfx);

        ApplyVolume(MASTER_PARAM, master);
        ApplyVolume(BGM_PARAM, bgm);
        ApplyVolume(SFX_PARAM, sfx);
    }
    private void OnMasterSliderChanged(float value01)
    {
        if (_syncing) return;

        ApplyVolume(MASTER_PARAM, value01);
        SyncPair(masterSlider, masterInput, value01);
        Save(PREF_MASTER, value01);
    }

    private void OnBgmSliderChanged(float value01)
    {
        if (_syncing) return;

        ApplyVolume(BGM_PARAM, value01);
        SyncPair(bgmSlider, bgmInput, value01);
        Save(PREF_BGM, value01);
    }

    private void OnSfxSliderChanged(float value01)
    {
        if (_syncing) return;

        ApplyVolume(SFX_PARAM, value01);
        SyncPair(sfxSlider, sfxInput, value01);
        Save(PREF_SFX, value01);
    }

    private void OnMasterInputEndEdit(string text) => ApplyFromInput(masterSlider, masterInput, MASTER_PARAM, PREF_MASTER, text);
    private void OnBgmInputEndEdit(string text) => ApplyFromInput(bgmSlider, bgmInput, BGM_PARAM, PREF_BGM, text);
    private void OnSfxInputEndEdit(string text) => ApplyFromInput(sfxSlider, sfxInput, SFX_PARAM, PREF_SFX, text);

    private void ApplyFromInput(Slider slider, TMP_InputField input, string mixerParam, string prefKey, string text)
    {
        if (_syncing) return;
        if (!slider || !input) return;

        int fallback = Slider01ToPercent(slider.value);
        int percent = ParsePercentOrFallback(text, fallback);
        percent = Mathf.Clamp(percent, 0, 100);

        float value01 = percent / 100f;

        _syncing = true;
        slider.SetValueWithoutNotify(value01);
        input.SetTextWithoutNotify(percent.ToString());
        _syncing = false;

        ApplyVolume(mixerParam, value01);
        Save(prefKey, value01);
    }

    private void SyncAllUI(float master01, float bgm01, float sfx01)
    {
        _syncing = true;

        if (masterSlider) masterSlider.SetValueWithoutNotify(master01);
        if (bgmSlider) bgmSlider.SetValueWithoutNotify(bgm01);
        if (sfxSlider) sfxSlider.SetValueWithoutNotify(sfx01);

        if (masterInput) masterInput.SetTextWithoutNotify(Slider01ToPercent(master01).ToString());
        if (bgmInput) bgmInput.SetTextWithoutNotify(Slider01ToPercent(bgm01).ToString());
        if (sfxInput) sfxInput.SetTextWithoutNotify(Slider01ToPercent(sfx01).ToString());

        _syncing = false;
    }

    private void SyncPair(Slider slider, TMP_InputField input, float value01)
    {
        _syncing = true;

        if (slider) slider.SetValueWithoutNotify(value01);
        if (input) input.SetTextWithoutNotify(Slider01ToPercent(value01).ToString());

        _syncing = false;
    }

    private static int Slider01ToPercent(float value01)
        => Mathf.RoundToInt(Mathf.Clamp01(value01) * 100f);

    private static int ParsePercentOrFallback(string text, int fallback)
    {
        if (string.IsNullOrWhiteSpace(text)) return fallback;
        text = text.Trim();

        if (text.EndsWith("%"))
            text = text.Substring(0, text.Length - 1);

        return int.TryParse(text, out int v) ? v : fallback;
    }

    private static void SetupNumericInput(TMP_InputField input)
    {
        if (!input) return;

        input.contentType = TMP_InputField.ContentType.IntegerNumber;
        input.characterLimit = 3;
        input.richText = false;
    }

    private void ApplyVolume(string param, float linear01)
    {
        if (!mixer) return;

        if (linear01 <= 0f)
        {
            mixer.SetFloat(param, MIN_DB);
            return;
        }

        float clamped = Mathf.Clamp(linear01, MIN_LINEAR, 1f);

        float minDb = MIN_DB;
        float maxDb = 0f;

        if (param == SFX_PARAM)
        {
            maxDb = SFX_MAX_DB;
        }

        float db = Mathf.Lerp(minDb, maxDb, clamped);
        mixer.SetFloat(param, db);
    }

    private static void Save(string key, float value01)
    {
        PlayerPrefs.SetFloat(key, Mathf.Clamp01(value01));
        PlayerPrefs.Save();
    }
}