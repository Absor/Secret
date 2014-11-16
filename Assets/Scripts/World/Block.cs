public class Block
{
    public bool render;

    private bool transparent;
    public bool Transparent
    {
        get
        {
            return transparent || !render;
        }
        set
        {
            transparent = value;
        }
    }
}
