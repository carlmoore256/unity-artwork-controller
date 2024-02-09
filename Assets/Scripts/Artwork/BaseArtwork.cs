using UnityEngine;


public abstract class BaseArtwork : MonoBehaviour, IArtwork
{
    public string Id => gameObject.name.Split("Artwork__")[1];
    public virtual string Name => Id;

    public ArtworkMetadata GetMetadata()
    {
        return new ArtworkMetadata { id = Id, name = Name };
    }

    public IInsert[] GetInserts()
    {
        return GetComponents<IInsert>();
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
