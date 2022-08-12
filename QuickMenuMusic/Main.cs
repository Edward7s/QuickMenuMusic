using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ABI_RC.Core.InteractionSystem;
using UnityEngine;
using HarmonyLib;
using System.Reflection;
using ABI_RC.Core.Savior;
using BepInEx;
namespace QuickMenuMusic
{
    [BepInPlugin("org.bepinex.plugins.QuickMenuMusicNocturnal", "QuickMenuMusic", "1.0.0.0")]

    public class Main : BaseUnityPlugin
    {
        private static CVR_MenuManager s_qmManager { get; set; }
        private static ViewManager s_bigMenu { get; set; }
        private static AudioSource s_audioSource { get; set; }
        private static FieldInfo s_fieldInfoQm { get; set; }
        private static FieldInfo s_fieldInfoBigMenu { get; set; }
        private static CVRSettingsValue s_audioField { get; set; }

        private Harmony _instance = new Harmony(Guid.NewGuid().ToString());

        public void Start()
        {
            _instance.Patch(typeof(ViewManager).GetMethods().FirstOrDefault(m => m.Name == "UiStateToggle" && m.GetParameters().Length == 1), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(BigMenuToggle), BindingFlags.NonPublic | BindingFlags.Static)));
            _instance.Patch(typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu)), null, new HarmonyMethod(typeof(Main).GetMethod(nameof(QmToggled), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)));
            new Config();
            StartCoroutine(WaitForUi());
        }

        private static IEnumerator WaitForUi()
        {
            while (GameObject.Find("/Cohtml") == null) yield return null;
            s_audioSource = new GameObject("MenuMusicManager").AddComponent<AudioSource>();
            s_audioSource.transform.parent = GameObject.Find("/Cohtml").transform;
            WWW webReq = new WWW("File://" + QuickMenuMusic.Config.Instance.Path);
            yield return webReq;
            s_audioSource.clip = webReq.GetAudioClip(false, false);
            s_audioSource.playOnAwake = true;
            s_audioSource.loop = true;
            s_audioSource.enabled = false;
            s_bigMenu = GameObject.Find("/Cohtml/CohtmlWorldView").GetComponent<ViewManager>();
            s_qmManager = GameObject.Find("/Cohtml").GetComponent<CVR_MenuManager>();
            s_fieldInfoQm = typeof(CVR_MenuManager).GetField("_quickMenuOpen", BindingFlags.NonPublic | BindingFlags.Instance);
            s_fieldInfoBigMenu = typeof(ViewManager).GetField("_gameMenuOpen", BindingFlags.NonPublic | BindingFlags.Instance);
            List<CVRSettingsValue> settings = (List<CVRSettingsValue>)typeof(CVRSettings).GetField("_settings", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(MetaPort.Instance.settings);
            s_audioField = settings.FirstOrDefault(x => x.GetName() == "AudioUi");
        }

        private static void QmToggled(bool __0)
        {
            
            if (__0)
            {
                s_audioSource.enabled = __0;
                s_audioSource.volume = (float)s_audioField.GetValueInt() / 100 ;
                return;
            }
            if ((bool)s_fieldInfoBigMenu.GetValue(s_bigMenu)) return;
            s_audioSource.enabled = false;
        }

        private static void BigMenuToggle(bool __0)
        {
            if (__0)
            {
                s_audioSource.enabled = __0;
                s_audioSource.volume = (float)s_audioField.GetValueInt() / 100;
                return;
            }
            if ((bool)s_fieldInfoQm.GetValue(s_qmManager)) return;
            s_audioSource.enabled = false;
        }


    }
}
