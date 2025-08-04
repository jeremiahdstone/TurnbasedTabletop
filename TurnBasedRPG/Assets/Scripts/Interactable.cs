using UnityEngine;

public class Interactable : MonoBehaviour, IInteractable
{
    public virtual void Interact(Player player)
    {
        if (!CanInteract(player.gameObject)) return;

        player.GetComponent<Player>().useAction();

        Debug.Log($"{gameObject.name} was interacted with by {player.gameObject.name}");
        // Derived classes override this
    }

    protected virtual bool CanInteract(GameObject interactor)
    {
        GameObject currentPlayer = GameManager.Instance.currentTurnPlayer;
        if (interactor != currentPlayer) return false;

        float distance = Vector2.Distance(transform.position, interactor.transform.position);
        return distance <= 1.0f;
    }

    private void OnMouseDown()
    {
        Debug.Log("poked");
        Interact(GameManager.Instance.currentTurnPlayer.GetComponent<Player>());
    }
}
