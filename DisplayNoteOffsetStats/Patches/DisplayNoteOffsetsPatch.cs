using Blittables;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisplayNoteOffsetStats.Patches
{
    internal class DisplayNoteOffsetsPatch
    {
        static float totalOffset = 0.0f;
        static int totalInputs = 0;
        static float previousTime = 0.0f;

        [HarmonyPatch(typeof(EnsoGameManager), "ProcExecMain")]
        [HarmonyPatch(MethodType.Normal)]
        [HarmonyPostfix]
        public static void EnsoGameManager_ProcExecMain_Postfix(EnsoGameManager __instance)
        {
            var frameResults = __instance.ensoParam.GetFrameResults();
            int num = 0;
            foreach (HitResultInfo info in frameResults.hitResultInfo)
            {
                if (num >= frameResults.hitResultInfoNum)
                {
                    break;
                }
                num++;
                if (info.hasOnpu != 0 && info.player == 0 && info.hitResult > -2 && info.hitResult != -1 && info.hitResult < 3)
                {
                    var currentTime = __instance.ensoParam.TotalTime - __instance.ensoInput.offsetTimeAdjustMent - __instance.ensoInput.playerInfo[0].delay;
                    if (info.onpuType != 6 && info.onpuType != 9 && info.onpuType != 10 && info.onpuType != 11 && info.onpuType != 12)
                    {
                        if (currentTime < previousTime)
                        {
                            totalOffset = 0.0f;
                            totalInputs = 0;
                        }

                        // Just the current bpm, does not include note scroll speed
                        //Plugin.Log.LogInfo("__instance.ensoParam.Tempo: " + __instance.ensoParam.Tempo);

                        previousTime = currentTime;
                        totalOffset += info.onpu.justTime - currentTime;
                        totalInputs++;
                        Plugin.Log.LogInfo("totalOffset: " + totalOffset);
                        Plugin.Log.LogInfo("totalInputs: " + totalInputs);
                        Plugin.Log.LogInfo("noteOffset: " + (info.onpu.justTime - currentTime));
                        Plugin.Log.LogInfo("Average Ms Offset: " + (totalOffset / totalInputs));
                        Plugin.Log.LogInfo("");
                        Plugin.Log.LogInfo("");
                    }
                }
                // Probably not a good idea to display stats for Player 1 AND Player 2 at the same time
                // Although it could work, so I'm keeping it here for now
                //else if (info.hasOnpu != 0 && info.player == 1 && info.hitResult > -2 && info.hitResult != -1 && info.hitResult < 3)
                //{
                //    var currentTime = __instance.ensoParam.TotalTime - __instance.ensoInput.offsetTimeAdjustMent - __instance.ensoInput.playerInfo[0].delay;
                //    if (info.onpuType != 6 && info.onpuType != 9 && info.onpuType != 10 && info.onpuType != 11 && info.onpuType != 12)
                //    {
                //        if (currentTime < previousTime)
                //        {
                //            totalOffset = 0.0f;
                //            totalInputs = 0;
                //        }

                //        previousTime = currentTime;
                //        totalOffset += info.onpu.justTime - currentTime;
                //        totalInputs++;
                //        Plugin.Log.LogInfo("totalOffset: " + totalOffset);
                //        Plugin.Log.LogInfo("totalInputs: " + totalInputs);
                //        Plugin.Log.LogInfo("noteOffset: " + (info.onpu.justTime - currentTime));
                //        Plugin.Log.LogInfo("Average Ms Offset: " + (totalOffset / totalInputs));
                //        Plugin.Log.LogInfo("");
                //        Plugin.Log.LogInfo("");
                //    }
                //}
            }
        }
    }
}
