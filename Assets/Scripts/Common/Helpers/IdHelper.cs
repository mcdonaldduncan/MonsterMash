public static class IdHelper
{
    static int Current;

    public static int GetNextID()
    {
        return Current++;
    }
}