#region SDK
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#endregion

#region Enum
public enum ScreenSetting { Window, Fullscreen, Borderless }
#endregion

public class Settings : MonoBehaviour
{
    #region Values
    public static Settings instance = null;

    //Setting Values
    [SerializeField] private int resolutionIndex = 0;
    private Vector2[] resolution = new Vector2[] { new Vector2(1280, 720), new Vector2(1920, 1080), new Vector2(2560, 1440) };
    [SerializeField] private float masterSoundLevel = 100, musicLevel = 100, ambientLevel = 100;
    [SerializeField] private ScreenSetting screenSetting = ScreenSetting.Window;
    [SerializeField] private float fov = 60;

    //UI
    [SerializeField] private TMP_Dropdown resolutionDropdown = null;
    [SerializeField] private TMP_Dropdown screenDropdown = null;
    #endregion

    private void OnValidate()
    {
        masterSoundLevel = Mathf.Clamp(masterSoundLevel, 0, 100);
        musicLevel = Mathf.Clamp(musicLevel, 0, 100);
        ambientLevel = Mathf.Clamp(ambientLevel, 0, 100);

        fov = Mathf.Clamp(fov, 60, 90);

        Awake();
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(this);
    }

    private void Awake()
    {
        resolutionDropdown.ClearOptions();
        foreach (Vector2 vec in resolution)
            resolutionDropdown.options.Add(new TMP_Dropdown.OptionData() { text = (vec.x + " : " + vec.y) });
        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, resolution.Length - 1);
        resolutionDropdown.value = resolutionIndex;

        screenDropdown.value = (int)screenSetting;
    }

    #region Getters
    public float GetMasterLevel()
    {
        return masterSoundLevel;
    }
    public float GetMusicLevel()
    {
        return musicLevel;
    }
    public float GetAmbientLevel()
    {
        return ambientLevel;
    }

    public float GetFOV()
    {
        return fov;
    }
    #endregion

    #region Setters
    public void SetMasterLevel(float set)
    {
        masterSoundLevel = Mathf.Clamp(set, 0, 100);
    }
    public void SetMusicLevel(float set)
    {
        musicLevel = Mathf.Clamp(set, 0, 100);
    }
    public void SetAmbientLevel(float set)
    {
        ambientLevel = Mathf.Clamp(set, 0, 100);
    }

    public void SetFOV(float set)
    {
        fov = Mathf.Clamp(set, 60, 90);
    }

    public void SetScreenSetting(int set)
    {
        screenSetting = (ScreenSetting)set;
    }
    #endregion

    #region In
    private void SaveSettings()
    {

    }

    private void LoadSettings()
    {

    }
    #endregion
}