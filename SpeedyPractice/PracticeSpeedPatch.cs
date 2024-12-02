using HMUI;
using UnityEngine;
using HarmonyLib;
using System;
using System.Collections;

namespace SpeedyPractice {
	[HarmonyPatch(typeof(PracticeViewController), "DidActivate")]
	static class PracticeSpeedPatch {
		static void Postfix(bool firstActivation, PracticeViewController __instance) {
			if(!firstActivation)
				return;

			var slider = __instance._speedSlider;

			int minSpeed = PluginConfig.instance.minSpeed;
			int maxSpeed = PluginConfig.instance.maxSpeed;
			int stepSize = Math.Max(1, PluginConfig.instance.stepSize);

			maxSpeed -= (maxSpeed - minSpeed) % stepSize;

			slider.minValue = minSpeed / 100f;
			slider.maxValue = maxSpeed / 100f;
			slider.numberOfSteps = (maxSpeed - minSpeed) / stepSize + 1;

			slider.value = Mathf.Clamp(__instance._practiceSettings.songSpeedMul, slider.minValue, slider.maxValue);
			__instance._practiceSettings.songSpeedMul = slider.value;

			HANDLER(slider, slider.value, __instance._beatmapLevel);

			slider.valueDidChangeEvent += (a, b) => {
				HANDLER(a, b, __instance._beatmapLevel);
			};
		}

		public static void HANDLER(RangeValuesTextSlider slider, float value, BeatmapLevel ____level) {
			var t = slider.GetComponentInChildren<CurvedTextMeshPro>();

			t.text = string.Format("{0} {1:0.0} BPM", t.text, value * ____level.beatsPerMinute);

			((RectTransform)t.transform).sizeDelta += new Vector2(20, 0);
		}
	}
}