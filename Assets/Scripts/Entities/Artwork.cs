using UnityEngine;

public class Artwork : MonoBehaviour
{
    [SerializeField] private int _id;
    public int Id { get => _id; set => _id = value; }

    public void AddController(IArtworkController controller)
    {
        gameObject.AddComponent(controller.GetType());
        // controller.OnArtworkAdded(this);
    }
}