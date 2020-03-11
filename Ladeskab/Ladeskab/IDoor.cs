using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal.Execution;

namespace Ladeskab
{
    public class DoorEventArgs : EventArgs
    {
        public string Status { get; set; }
    }
    
    public interface IDoor
    {
        void CloseDoor();
        void OpenDoor();
        void LockDoor();
        void UnlockDoor();
        event EventHandler<DoorEventArgs> DoorEvent;
    }
}
