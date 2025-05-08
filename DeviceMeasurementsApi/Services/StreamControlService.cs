namespace DeviceMeasurementsApi.Services
{
    public class StreamControlService
    {
        private volatile bool _stopStream = false;

        public bool ShouldStop => _stopStream;

        public void RequestStop() => _stopStream = true;

        public void Reset() => _stopStream = false;
    }
}
