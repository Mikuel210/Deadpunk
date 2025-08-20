using System;
using Helpers;
using UnityEngine;

namespace DayNightCycle {

	public class TimeService {

		public DateTime CurrentTime { get; private set; }
		
		private readonly TimeSettings _settings;
		private readonly TimeSpan _sunriseTime;
		private readonly TimeSpan _sunsetTime;

		public event Action OnSunrise;
		public event Action OnSunset;
		public event Action OnHourChanged;

		private readonly Observer<bool> _isDayTime = new();
		private readonly Observer<int> _currentHour = new();

		public TimeService(TimeSettings settings) {
			_settings = settings;
			CurrentTime = DateTime.Now.Date + TimeSpan.FromHours(settings.startHour);
			_sunriseTime = TimeSpan.FromHours(settings.sunriseHour);
			_sunsetTime = TimeSpan.FromHours(settings.sunsetHour);
			
			_isDayTime.OnValueChanged += isDayTime => (isDayTime ? OnSunrise : OnSunset)?.Invoke();
			_currentHour.OnValueChanged += _ => OnHourChanged?.Invoke();
		}

		public void Update(float deltaTime) {
			CurrentTime = CurrentTime.AddSeconds(deltaTime * _settings.timeMultiplier);

			bool isDayTime = IsDayTime();
			int currentHour = CurrentTime.Hour;

			if (_isDayTime.Value != isDayTime) _isDayTime.Value = isDayTime;
			if (_currentHour.Value != currentHour) _currentHour.Value = currentHour;
		}

		public float CalculateSunAngle() {
			float startDegree = _isDayTime ? 0 : 180;
			TimeSpan start = _isDayTime ? _sunriseTime : _sunsetTime;
			TimeSpan end = _isDayTime ? _sunsetTime : _sunriseTime;
			
			TimeSpan totalTime = CalculateDifference(start, end);
			TimeSpan elapsedTime = CalculateDifference(start, CurrentTime.TimeOfDay);
			
			double percentage = elapsedTime.TotalMinutes / totalTime.TotalMinutes;
			return Mathf.Lerp(startDegree, startDegree + 180, (float)percentage);
		}
		
		public bool IsDayTime() => CurrentTime.TimeOfDay >= _sunriseTime && CurrentTime.TimeOfDay < _sunsetTime;

		private TimeSpan CalculateDifference(TimeSpan from, TimeSpan to) {
			TimeSpan difference = to - from;
			return difference.TotalHours < 0 ? difference + TimeSpan.FromHours(24) : difference;
		}
		
		

	}

}