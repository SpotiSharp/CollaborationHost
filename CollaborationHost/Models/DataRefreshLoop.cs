namespace CollaborationHost.Models;

public delegate void DataRefresh();

public class DataRefreshLoop
{
    private static DataRefreshLoop _dataRefreshLoop;
    public static DataRefreshLoop Instance => _dataRefreshLoop ??= new DataRefreshLoop();
    public event DataRefresh OnDataRefresh;

    private DataRefreshLoop() {}
    
    public void Loop()
    {
        while (true)
        {
            Thread.Sleep(new TimeSpan(24, 0, 0));
            OnDataRefresh?.Invoke(); 
        }
    }
}