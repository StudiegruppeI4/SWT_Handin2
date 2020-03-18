using System;
using Ladeskab;

namespace Ladeskab
{
    public class ChargeControl : IChargeControl
    {
        public IUsbCharger Usb { get; set; }

        public ChargeControl(IUsbCharger usb)
        {
            Usb = usb;
        }

        public void StartCharge()
        {
            Usb.StartCharge();
        }

        public void StopCharge()
        {
            Usb.StopCharge();
        }

        public bool IsConnected()
        {
            return Usb.Connected;
        }
    }
}