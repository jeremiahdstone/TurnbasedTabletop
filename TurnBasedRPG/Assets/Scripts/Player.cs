using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using dungeonGen;
using UnityEditor.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] public int PlayerNumber { get; private set; } = 1;

    [Header("Player Settings")]
    [SerializeField] private int health = 100;
    [SerializeField] private int usedDistance = 0;
    [SerializeField] public int maxDistance = 3;
    

    [Header("Bonuses")]
    public int bonusHealth = 0;
    public int bonusDistance = 0;

    [Header("Movement")]
    public Vector2Int currentPosition { get;  set; }
    public Vector2Int targetPosition { get;  set; }

    // This method is called when the player takes damage
    public void TakeDamage(int damage)
    {
        health -= damage;
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
