using UnityEngine;

public class PoolChild : MonoBehaviour
{
    public int id { get; private set; }   

    public void Initialise(int id)
    {
        this.id = id;
    }
}
