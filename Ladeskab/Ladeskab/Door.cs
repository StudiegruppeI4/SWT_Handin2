using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladeskab
{
    public class Door : IDoor
    {
        public bool Locked { get; set; } = false;
        
        public void UnlockDoor()
        {
            Console.WriteLine("Stationcontrol unlocking door..");
            Locked = false;
        }

        public event EventHandler<DoorEventArgs> DoorEvent; 
        public void CloseDoor()
        {
            if (!Locked)
            {
                OnDoorEvent(new DoorEventArgs() { Status = "Closed" });
            }
            else
            {
                Console.WriteLine("Door is locked, can't close..");
            }
        }

        public void OpenDoor()
        {
            if (!Locked)
            {
                OnDoorEvent(new DoorEventArgs() { Status = "Open" });
            }
            else
            {
                Console.WriteLine("Door is locked, can't open..");
            }
        }

        public void LockDoor()
        {
            Console.WriteLine("Stationcontrol locking door..");
            Locked = true;
        }

        protected virtual void OnDoorEvent(DoorEventArgs e)
        {
            DoorEvent?.Invoke(this, e);
        }
    }
}
