using Helpers;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public Observer<int> People { get; private set; } = new();
    public Observer<int> Housing { get; private set; } = new();
    public Observer<int> Food { get; private set; } = new();
    public Observer<int> Wood { get; private set; } = new();
    public Observer<int> Stone { get; private set; } = new();
    public Observer<float> Hunger { get; private set; } = new();
    public Observer<float> Happiness { get; private set; } = new();
    
    void Start()
    {
        Wood.Value = 100;
        Stone.Value = 100;
    }
    
    void Update()
    {
        
    }
}
