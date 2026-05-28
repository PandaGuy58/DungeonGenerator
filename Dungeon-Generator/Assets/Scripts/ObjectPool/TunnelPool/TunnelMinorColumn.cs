public class TunnelMinorColumn : ObjectPoolMasterclass
{
    public static TunnelMinorColumn instance;

    private void Awake()
    {
        instance = this;
    }
}
