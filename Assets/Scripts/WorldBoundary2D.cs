using UnityEngine;

public class WorldBoundary2D : MonoBehaviour
{
    private BoxCollider2D boundaryCollider;

    void Start()
    {
        // Ensure the GameObject has a BoxCollider2D component
        boundaryCollider = GetComponentInChildren<BoxCollider2D>();
        if (boundaryCollider == null)
        {
            boundaryCollider = gameObject.AddComponent<BoxCollider2D>();
        }

        // Set the size and trigger of the BoxCollider2D
        boundaryCollider.isTrigger = true;
        //boundaryCollider.size = new Vector2(100f, 100f); // Set the initial size of the boundary
    }

    private void OnDrawGizmos()
    {
        if (boundaryCollider != null)
        {
            Gizmos.color = Color.red;
            Vector3 colliderCentre = boundaryCollider.transform.position + (Vector3)boundaryCollider.offset;
            Vector3 colliderSize = new Vector3(boundaryCollider.size.x, boundaryCollider.size.y, 0);
            Gizmos.DrawWireCube(colliderCentre, colliderSize);
        }
    }
}
