using System;

namespace Ladeskab
{
    public class RFIDEventArgs : EventArgs
    {
        public int RFID { get; set; }
    }

    public interface IRFIDReader
    {
        event EventHandler<RFIDEventArgs> RFIDDetectedEvent;
    }
}