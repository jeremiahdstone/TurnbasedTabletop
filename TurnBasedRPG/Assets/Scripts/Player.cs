using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    int health = 100;
    int moveDistance = 3;

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
