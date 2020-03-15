using System;

namespace Ladeskab { 
class Program
    {
        static void Main(string[] args)
        {
            // Assemble your system here from all the classes
            UsbChargerSimulator usbChargerSimulator = new UsbChargerSimulator();
            ChargeControl chargeControl = new ChargeControl(usbChargerSimulator);
            Door door = new Door();
            ConcreteDisplay display = new ConcreteDisplay();
            RFIDReader rfidReader = new RFIDReader();

            StationControl stationControl = new StationControl(chargeControl, door, display, rfidReader);

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
