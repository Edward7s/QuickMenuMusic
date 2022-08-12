using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABI_RC.Core.InteractionSystem;
using MelonLoader;
using UnityEngine;
using Harmony;
using System.Reflection;
using ABI_RC.Core.Savior;

namespace QuickMenuMusic
{
    public class Main : MelonMod
    {
        private static CVR_MenuManager s_qmManager { get; set; }
        private static ViewManager s_bigMenu { get; set; }
        private static AudioSource s_audioSource { get; set; }
        private static FieldInfo s_fieldInfoQm { get; set; }
        private static FieldInfo s_fieldInfoBigMenu { get; set; }
        private static CVRSettingsValue s_audioField { get; set; }

        private HarmonyInstance _instance = new HarmonyInstance(Guid.NewGuid().ToString());

        public override void OnApplicationStart()
        {
            _instance.Patch(typeof(ViewManager).GetMethods().FirstOrDefault(m => m.Name == "UiStateToggle" && m.GetParameters().Length == 1), null, typeof(Main).GetMethod(nameof(BigMenuToggle), BindingFlags.NonPublic | BindingFlags.Static).ToNewHarmonyMethod());
            _instance.Patch(typeof(CVR_MenuManager).GetMethod(nameof(CVR_MenuManager.ToggleQuickMenu)), null, typeof(Main).GetMethod(nameof(QmToggled), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).ToNewHarmonyMethod());
            new Config();
            MelonCoroutines.Start(WaitForUi());
        }

        private static IEnumerator WaitForUi()
        {
            while (GameObject.Find("/Cohtml") == null) yield return null;
            s_audioSource = new GameObject("MenuMusicManager").AddComponent<AudioSource>();
            s_audioSource.transform.parent = GameObject.Find("/Cohtml").transform;
            WWW webReq = new WWW("File://" + Config.Instance.Path);
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
