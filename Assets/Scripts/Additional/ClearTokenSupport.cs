namespace Additional
{
    public static class ClearTokenSupport
    {
        public static void ClearToken(ref System.Threading.CancellationTokenSource cts)
        {
            if (cts == null) return;

            if (!cts.IsCancellationRequested)
            {
                cts.Cancel();
            }

            cts.Dispose();
            cts = null;
        }
    }
}