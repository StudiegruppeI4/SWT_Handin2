using System;
using Ladeskab;

namespace Ladeskab
{
    public class ChargeControl : IChargeControl
    {
        private IUsbCharger _usb;

        public ChargeControl(IUsbCharger usb)
        {
            _usb = usb;
        }

        public event EventHandler<CurrentEventArgs> CurrentValueEvent;

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