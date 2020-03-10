using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladeskab
{
    public class Door : IDoor
    {
        public void LockDoor()
        {
            Console.WriteLine("Locking door..\n");
        }

        public void UnlockDoor()
        {
            Console.WriteLine("Unlocking door..\n");
        }
    }
}
