using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DayNightCycle;
using Helpers;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class GameManager : Singleton<GameManager>
{
    public Observer<int> People { get; private set; } = new();
    public Observer<int> Housing { get; private set; } = new();
    public Observer<int> Food { get; private set; } = new();
    public Observer<int> Wood { get; private set; } = new();
    public Observer<int> Stone { get; private set; } = new();
    public Observer<int> Metal { get; private set; } = new();
    
    public Observer<float> Happiness { get; private set; } = new();
    public Observer<float> Hunger { get; private set; } = new();

    [SerializeField] private BuildingSO startingBuilding;
    
    [field: Space, SerializeField] public int NightsWithoutWaves { get; private set; }
    [field: SerializeField] public List<Wave> Waves { get; private set; }
    
    [Serializable]
    public class Burst
    {
        public GameObject prefab;
        public int amount;
        public float delay;
        public float initialDelay;

        public float GetDuration() => initialDelay + delay * amount;
    }

    [Serializable]
    public class Wave
    {
        public List<Burst> bursts;
        
        public float GetDuration() => bursts.Max(e => e.GetDuration());
    }


    public State CurrentState { get; private set; }
    public enum State {
        Day,
        Night
    }
    
    
    public int CurrentCycle { get; private set; }
    public int CurrentWave { get; private set; }
    public int NextWave => CurrentState == State.Day ? CurrentWave + 1 : CurrentWave;
    
    
    void Awake() => People.OnValueChanged += _ => UpdateFoodInterval();
    
    void Start()
    {
        Wood.Value = 100;
        Stone.Value = 100;
        People.Value = 10;

        Happiness.Value = 100;
        Hunger.Value = 0;

        BuildingSystem.Instance.Build(
            BuildingSystem.Instance.Grid.GetGridPosition(new(-5, 0, 15)), 
            startingBuilding, 
            BuildingSO.Direction.Up
        );
        
        TimeManager.Instance.TimeService.OnSunrise += OnSunrise;
        TimeManager.Instance.TimeService.OnSunset += OnSunset;
    }
    
    void Update() {
        UpdateFood();
        UpdateHappiness();
    }

    
    private void OnSunrise() {
        CurrentState = State.Day;
    }
    
    private void OnSunset() {
        CurrentState = State.Night;
        CurrentCycle++;

        if (CurrentCycle < NightsWithoutWaves) return;

        CurrentWave++;

        Wave currentWave = Waves[Mathf.Min(CurrentWave - 1, Waves.Count - 1)];
        SpawnWave(currentWave);
    }
    
    
    private void SpawnWave(Wave wave) {
        (Vector3 center, Vector3 deviation) = GetWaveData();
        
        float baseDistance = 25f;
        float minimumDistance = 50f;

        deviation *= 2;
        deviation = new(
            Mathf.Max(deviation.x + baseDistance, minimumDistance), 0,
            Mathf.Max(deviation.z + baseDistance, minimumDistance)
        );
        
        foreach (Burst burst in wave.bursts)
            StartCoroutine(SpawnBurst(burst, center, deviation));
    }
    
    private IEnumerator SpawnBurst(Burst burst, Vector3 center, Vector3 deviation)
    {
        yield return new WaitForSeconds(burst.initialDelay);
        
        for (int i = 0; i < burst.amount; i++)
        {
            int angle = Random.Range(0, 360);
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * deviation.x;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * deviation.z;

            Vector3 spawnPosition = new Vector3(x, 0, z) + new Vector3(center.x, 0, center.z);
            
            Instantiate(burst.prefab, spawnPosition, Quaternion.identity);
            yield return new WaitForSeconds(burst.delay);
        }
    }

    private (Vector3 center, Vector3 deviation) GetWaveData() {
        List<GameObject> buildings = GameObject.FindGameObjectsWithTag("Building").ToList();
        List<Vector3> buildingPositions = buildings.Select(e => e.transform.Find("Mesh").transform.position).ToList();

        if (buildingPositions.Count == 0) 
            return (Vector3.zero, Vector3.zero);
        
        // Calculate center
        Vector3 center = Vector3.zero;
        
        foreach (Vector3 point in buildingPositions) center += point;
        center /= buildingPositions.Count;

        // Calculate deviation
        List<float> distancesX = new();
        List<float> distancesZ = new();

        foreach (Vector3 point in buildingPositions) {
            distancesX.Add(Mathf.Pow(point.x - center.x, 2));
            distancesZ.Add(Mathf.Pow(point.x - center.x, 2));
        }
        
        float varianceX = distancesX.Sum() / (distancesX.Count - 1);
        float varianceZ = distancesZ.Sum() / (distancesZ.Count - 1);
        
        float deviationX = Mathf.Sqrt(varianceX);
        float deviationZ = Mathf.Sqrt(varianceZ);
        
        Vector3 deviation = new(deviationX, 0, deviationZ);
        
        return (center, deviation);
    }
    
    
    
    
    private float _foodTime;
    private float _foodInterval;

    private void UpdateFood() {
        _foodTime += Time.deltaTime;
        
        if (_foodTime < _foodInterval) return;

        float deltaHunger = 5f;

        if (Food > 0) {
            Food.Value--;
            Hunger.Value -= deltaHunger;
        }
        else {
            Hunger.Value += deltaHunger;
        }

        Hunger.Value = Mathf.Clamp(Hunger, 0, 100);
        _foodTime = 0;
    }

    private void UpdateFoodInterval() => _foodInterval = 60f / People * 1f;

    const float HappinessScalingFactor = 0.15f;
    
    private void UpdateHappiness() {
        int peopleWithHousing = Mathf.Clamp(Housing, 0, People);
        int peopleWithoutHousing = People - peopleWithHousing;
        
        Happiness.Value += (peopleWithoutHousing == 0 ? 1f + peopleWithHousing * 0.1f : -peopleWithoutHousing) * Time.deltaTime * HappinessScalingFactor;
        Happiness.Value = Mathf.Clamp(Happiness.Value, 0, 100);   
    }
    
}
