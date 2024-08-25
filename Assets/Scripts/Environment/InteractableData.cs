using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum InteractableType { ICE_TREE1, ICE_TREE2, ICE_TREE3, LAVENDAR, ORANGE_BUSH, PLAINS_TREE, PLAINS_TREE2 }

[SerializeField]
public class InteractableData
{
    public InteractableType type;
    public float[] position;
    public float[] scale;
    public float[] spriteSize;

    public InteractableData(Interactable interactable)
    {
        type = interactable.type;

        position[0] = interactable.transform.position.x;
        position[1] = interactable.transform.position.y;
        position[2] = interactable.transform.position.z;

        scale[0] = interactable.transform.localScale.x;
        scale[1] = interactable.transform.localScale.y;
        scale[2] = interactable.transform.localScale.z;
    }
}
