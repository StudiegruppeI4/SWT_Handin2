using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladeskab
{
    public interface IChargeControl
    {
        event EventHandler<CurrentEventArgs> CurrentValueEvent;

        void StartCharge();


        void StopCharge();


        bool IsConnected();

    }
}
