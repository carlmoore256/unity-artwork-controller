using UnityEngine;

public class Artwork : MonoBehaviour
{
    [SerializeField] private int _id;
    public int Id { get => _id; set => _id = value; }
}