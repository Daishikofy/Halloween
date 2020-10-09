using UnityEngine;

public abstract class DestroyableObject : MonoBehaviour
{
    public int lifePoints;
    public Collider2D objectCollider;
    [HideInInspector]
    public DestroyableObjectEvent destroyed = new DestroyableObjectEvent();

    public bool Damaged()
    {
        lifePoints--;
        if (lifePoints <= 0)
        {
            DestroyObject();
            return true;
        }
        return false;
    }

    protected virtual void DestroyObject()
    {
        //TODO: Destroy animation
        objectCollider.enabled = false;
        destroyed.Invoke(this);
    }
}
