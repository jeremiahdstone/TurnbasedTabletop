using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using dungeonGen;
using UnityEditor.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] public int PlayerNumber = 1;

    [Header("Player Settings")]
    [SerializeField] private int maxHealth = 3;
    [SerializeField] public int health = 3;
    
    [SerializeField] public int maxDistance = 3;
    [SerializeField] public int distanceLeft { get; private set; } = 3;
    

    [Header("Bonuses")]
    public int bonusHealth = 0;
    public int bonusDistance = 0;

    [Header("Movement")]
    public Vector2Int currentPosition { get;  set; }
    public Vector2Int targetPosition { get;  set; }

    void Awake()
    {
        distanceLeft = maxDistance + bonusDistance;
        health = maxHealth + bonusHealth;
    }

    // This method is called when the player takes damage
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void startTurn()
    {
        distanceLeft = maxDistance + bonusDistance;
        // actions available = 1 + bonus
    }

    public void useDistance(int distance)
    {
        if (distanceLeft >= distance)
        {
            distanceLeft -= distance;
        }
        else
        {
            Debug.LogWarning("Not enough distance left to use.");
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
