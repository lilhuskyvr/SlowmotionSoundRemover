// ReSharper disable InconsistentNaming
// ReSharper disable PossibleNullReferenceException
// ReSharper disable UnusedType.Local
// ReSharper disable Unity.InefficientPropertyAccess
// ReSharper disable UnusedMember.Local
// ReSharper disable RedundantAssignment
// ReSharper disable UnusedParameter.Local

using System;
using System.Collections;
using System.Reflection;
using HarmonyLib;
using ThunderRoad;
using UnityEngine;

namespace SlowmotionSoundRemover.Patch
{
    // ReSharper disable once UnusedType.Global
    public class SlowmotionSoundRemoverPatchLevelModule : LevelModule
    {
        private Harmony _harmony;
        public bool hasHeartbeatSound = false;
        public bool hasAudioMixer = false;

        public override IEnumerator OnLoadCoroutine()
        {
            try
            {
                _harmony = new Harmony("SlowmotionSoundRemover");
                _harmony.PatchAll(Assembly.GetExecutingAssembly());

                Debug.Log("Slowmotion Sound Remover Loaded");
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            var slowmotionSoundRemoverSettings =
                GameManager.local.gameObject.AddComponent<SlowmotionSoundRemoverSettings>();

            slowmotionSoundRemoverSettings.hasHeartbeatSound = hasHeartbeatSound;
            slowmotionSoundRemoverSettings.hasAudioMixer = hasAudioMixer;

            return base.OnLoadCoroutine();
        }

        [HarmonyPatch(typeof(GameManager))]
        [HarmonyPatch("SetSlowMotion")]
        private static class GameManagerSetSlowMotionPatch
        {
            [HarmonyPrefix]
            private static void Prefix(bool active,
                float scale,
                AnimationCurve curve,
                ref EffectData effectData,
                ref bool snapshotTransition)
            {
                var slowmotionSoundRemoverSettings =
                    GameManager.local.gameObject.GetComponent<SlowmotionSoundRemoverSettings>();

                if (!slowmotionSoundRemoverSettings.hasHeartbeatSound)
                {
                    effectData = Catalog.GetData<EffectData>("MutedSpellSlowTime");
                }

                if (!slowmotionSoundRemoverSettings.hasAudioMixer)
                {
                    snapshotTransition = false;
                }
            }
        }
    }
}