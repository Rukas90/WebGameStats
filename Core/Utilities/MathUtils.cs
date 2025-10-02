namespace Core.Utilities;

public static class MathUtils
{
    public static bool WillOverflow(int a, int b)
    {
        try
        {
            checked
            {
                int _ = a + b;
            }
            return false;
        }
        catch
        {
            return true;
        }
    }
}