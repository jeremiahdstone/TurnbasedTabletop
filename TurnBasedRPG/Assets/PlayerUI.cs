using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void useAction()
    {
        GameManager.Instance.useAction();
    }

    public void changeTurn()
    {
        GameManager.Instance.changeTurn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
