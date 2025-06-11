using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using dungeonGen;
using UnityEditor.Rendering;

public class Player : MonoBehaviour
{
    [SerializeField] public int PlayerNumber = 1;

    [Header("Player Settings")]

    [SerializeField] private int maxActions = 1;
    [SerializeField] public int actionsLeft = 1;
    [SerializeField] private int maxHealth = 3;
    [SerializeField] public int health = 3;

    [SerializeField] public int maxDistance = 3;
    [SerializeField] public int distanceLeft = 3;


    [Header("Bonuses")]
    public int bonusActions = 0;
    public int bonusHealth = 0;
    public int bonusDistance = 0;

    [Header("Movement")]
    public Vector2Int currentPosition { get; set; }
    public Vector2Int targetPosition { get; set; }

    

    void Awake()
    {
        distanceLeft = maxDistance + bonusDistance;
        health = maxHealth + bonusHealth;
        actionsLeft = maxActions + bonusActions;
        
        

    }

    // This method is called when the player takes damage
    public void TakeDamage(int damage)
    {
        health -= damage;
    }

    public void startTurn()
    {
        distanceLeft = maxDistance + bonusDistance;
        actionsLeft = maxActions + bonusActions;
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

    public void checkEndTurn()
    {
        if (actionsLeft <= 0 && distanceLeft <= 0)
        {
            GameManager.Instance.changeTurn();
            
        }
    }

    public void useAction()
    {
        if (actionsLeft > 0)
        {
            actionsLeft--;
        }
        else
        {
            Debug.LogWarning("No actions left to use.");
        }
        checkEndTurn();
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
