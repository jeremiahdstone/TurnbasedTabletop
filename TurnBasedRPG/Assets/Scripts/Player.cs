using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using dungeonGen;

public class Player : MonoBehaviour
{

    [Header("Player Settings")]
    int health = 100;
    int usedDistance = 0;
    int maxDistance = 3;
    

    [Header("Bonuses")]
    public int bonusHealth = 0;
    public int bonusDistance = 0;

    [Header("Movement")]
    Vector2Int currentPosition;
    Vector2Int targetPosition;

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
