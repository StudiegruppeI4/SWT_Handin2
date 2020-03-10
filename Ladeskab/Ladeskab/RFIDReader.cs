using System;

namespace Ladeskab
{
    public class RFIDReader : IRFIDReader
    {
        public event EventHandler<RFIDEventArgs> RFIDDetectedEvent;

        public void ReadRFID(int id)
        {
            OnReadRFID(new RFIDEventArgs() {RFID = id});
        }

        protected virtual void OnReadRFID(RFIDEventArgs e)
        {
            RFIDDetectedEvent?.Invoke(this, e);
        }
    }
}