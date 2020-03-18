using System;

namespace Ladeskab
{
    public interface IChargeControl
    {
        IUsbCharger Usb { get; set; }

        void StartCharge();


        void StopCharge();


        bool IsConnected();

    }
}
