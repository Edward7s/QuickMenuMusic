using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MelonLoader;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

namespace QuickMenuMusic
{
    public class Main : MelonMod
    {
        private ABI_RC.Core.InteractionSystem.CVR_MenuManager _qmManager { get; set; }
        private ABI_RC.Core.InteractionSystem.ViewManager _bigMenu { get; set; }
        public override void OnApplicationStart()
        {
            MelonCoroutines.Start(WaitForUi());
            new Config();
        }

        private static IEnumerator WaitForUi()
        {
            while (GameObject.Find("/Cohtml") == null) yield return null;

            AudioSource audioSource = new GameObject("MenuMusicManager").AddComponent<AudioSource>();
            audioSource.transform.parent = GameObject.Find("/Cohtml").transform;
            WWW webReq = new WWW("File://" + Config.Instance.Path);
            yield return webReq;
            audioSource.clip = webReq.GetAudioClip(false, false);
            audioSource.playOnAwake = true;
        }
    }
}
