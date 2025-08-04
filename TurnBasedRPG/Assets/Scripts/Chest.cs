using UnityEngine;

public class Chest : Interactable
{
    public override void Interact(Player player)
    {
        
        if (!CanInteract(player.gameObject)) return;

        player.GetComponent<Player>().useAction();

        Debug.Log($"{gameObject.name} has been opened by {player.gameObject.name}");
    }
}
