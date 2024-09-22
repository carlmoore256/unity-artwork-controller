using UnityEngine;

public abstract class BaseArtwork : MonoBehaviour, IArtwork
{
    public string Id => gameObject.name.Split("Artwork__")[1];
    public virtual string Name => Id;
    public bool IsEnabled => gameObject.activeSelf;

    public ArtworkMetadata GetMetadata()
    {
        return new ArtworkMetadata { id = Id, name = Name, isEnabled = IsEnabled};
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
