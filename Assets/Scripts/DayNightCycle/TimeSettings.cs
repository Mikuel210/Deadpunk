using UnityEngine;

namespace DayNightCycle 
{

	[CreateAssetMenu(fileName = "New Time Settings", menuName = "Scriptable Objects/Time Settings")]
	public class TimeSettings : ScriptableObject {

		public float timeMultiplier = 2_000;
		public float startHour = 12;
		public float sunriseHour = 6;
		public float sunsetHour = 18;

	}

}

