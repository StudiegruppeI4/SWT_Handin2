using UsbSimulator;

namespace Ladeskab
{
    public class ChargeControl
    {
        private IUsbCharger _usb;

        public ChargeControl(IUsbCharger usb)
        {
            _usb = usb;
        }

        public void StartCharge()
        {
            _usb.StartCharge();
        }

        public void StopCharge()
        {
            _usb.StopCharge();
        }

        public bool IsConnected()
        {
            return _usb.Connected;
        }
    }
}