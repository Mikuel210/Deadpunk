using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace DayNightCycle {

	public class TimeManager : Singleton<TimeManager> {

		[SerializeField] private TimeSettings timeSettings;
		
		[Header("Effects")]
		[SerializeField] private TextMeshProUGUI timeText;
		
		[Space, SerializeField] private Light sun;
		[SerializeField] private Light moon;

		[Space, SerializeField] private AnimationCurve lightIntensityCurve;
		[SerializeField] private float maximumSunIntensity;
		[SerializeField] private float maximumMoonIntensity;

		[Space, SerializeField] private Color dayAmbientLight;
		[SerializeField] private Color nightAmbientLight;
		[SerializeField] private Volume volume;

		public TimeService TimeService { get; private set; }
		private ColorAdjustments _colorAdjustments;

		void Awake() {
			TimeService = new(timeSettings);
			volume.profile.TryGet(out _colorAdjustments);
		}

		void Update() {
			UpdateTimeOfDay();
			UpdateLightRotation();
			UpdateLightSettings();
		}

		private void UpdateTimeOfDay() {
			TimeService.Update(Time.deltaTime);
			timeText.text = TimeService.CurrentTime.ToString("'<mspace=0.75em>'HH'<mspace=0.35em>':'<mspace=0.75em>'mm'</mspace>'");
		}

		private void UpdateLightRotation() {
			Quaternion rotation = Quaternion.AngleAxis(TimeService.CalculateSunAngle(), Vector3.right);
			sun.transform.rotation = rotation;
			
			Vector3 eulerAngles = rotation.eulerAngles;
			moon.transform.rotation = Quaternion.Euler(eulerAngles.x + 180, eulerAngles.y, eulerAngles.z);
		}

		private void UpdateLightSettings() {
			float sunDotProduct = Vector3.Dot(sun.transform.forward, Vector3.down) / 2f;
			sun.intensity = Mathf.Lerp(0, maximumSunIntensity, lightIntensityCurve.Evaluate(sunDotProduct));
			
			float moonDotProduct = Vector3.Dot(moon.transform.forward, Vector3.down) / 2f;
			moon.intensity = Mathf.Lerp(0, maximumMoonIntensity, lightIntensityCurve.Evaluate(moonDotProduct));

			if (_colorAdjustments == null) return;
			
			_colorAdjustments.colorFilter.value = Color.Lerp(nightAmbientLight, dayAmbientLight, lightIntensityCurve.Evaluate(sunDotProduct) * 2f);
		}

	}

}