using UnityEngine;

public class SetOrderInLayerForChildren : MonoBehaviour
{
    public int newOrderInLayer = 0;

    [ContextMenu("Appliquer Order in Layer aux enfants")]
    void SetOrderInChildren()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sr in sprites)
        {
            sr.sortingOrder = newOrderInLayer;
        }
        Debug.Log("Order in Layer mis à jour pour " + sprites.Length + " objets.");
    }
}