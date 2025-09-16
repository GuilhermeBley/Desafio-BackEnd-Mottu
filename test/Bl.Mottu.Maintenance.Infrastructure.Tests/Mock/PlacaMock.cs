namespace Bl.Mottu.Maintenance.Infrastructure.Tests.Mock;
static class PlacaMock
{
    private static object _lock = new object();
    private static int _count = 0;

    public static string NextPlaca()
    {
        int num;
        lock(_lock)
        {
            num = _count++;
        }
        return $"ABC{_count.ToString().PadLeft(4, '0')}";
    }
}
