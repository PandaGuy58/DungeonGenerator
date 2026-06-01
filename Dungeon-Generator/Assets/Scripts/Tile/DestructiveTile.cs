public class DestructiveTile : TileMasterClass
{
    private void Awake()
    {
        poolChildReference = GetComponent<PoolChild>();
    }
}
