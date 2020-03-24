using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ladeskab.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            UsbChargerSimulator usb = new UsbChargerSimulator();
            IChargeControl charger = new ChargeControl(usb);
            IDoor door = new Door();
            RFIDReader rfidReader = new RFIDReader();
            IDisplay display = new ConcreteDisplay();
            StationControl control = new StationControl(charger, door, display, rfidReader);

            bool finish = false;
            do
            {
                string input;
                System.Console.WriteLine("Indtast E, O, C, R: ");
                input = Console.ReadLine();
                if (string.IsNullOrEmpty(input)) continue;

                switch (input[0])
                {
                    case 'E':
                        finish = true;
                        break;

                    case 'O':
                        door.OpenDoor();
                        break;

                    case 'C':
                        door.CloseDoor();
                        break;

                    case 'R':
                        System.Console.WriteLine("Indtast RFID id: ");
                        string idString = System.Console.ReadLine();

                        int id = Convert.ToInt32(idString);
                        rfidReader.ReadRFID(id);
                        break;

                    default:
                        break;
                }

            } while (!finish);
        }
    }
}
