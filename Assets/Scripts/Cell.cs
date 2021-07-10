using UnityEngine;

public class Cell : MonoBehaviour {
    float width;
    float height;
    public CollisionMode collisionMode;
    private Color collisionModeColor;
    private bool showCollision = false;
    public int life;

    public CollisionMode CollisionMode1 {
        get => collisionMode;
        set {
            collisionMode = value;
            if (value == CollisionMode.Floor) {
                collisionModeColor = new Color(0.8f, 0.9f, 0.8f);
            } else if (value == CollisionMode.Wall) {
                collisionModeColor = new Color(0.9f, 0.8f, 0.8f);
            } else if (value == CollisionMode.Trap) {
                collisionModeColor = new Color(0.8f, 0.8f, 0.9f);
            }
        }
    }

    public void Init(Vector3 position, float width, float height, CollisionMode collisionMode = CollisionMode.Floor) {
        this.transform.position = position;
        this.width = width;
        this.height = height;
        CollisionMode1 = collisionMode;
    }

    public enum CollisionMode {
        Wall,
        Floor,
        Trap,
        Ice
    }

    public void ToggleCollision() {
        if (showCollision) {
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
        } else {
            GetComponent<SpriteRenderer>().color = collisionModeColor;
        }
        showCollision = !showCollision;
    }
}
