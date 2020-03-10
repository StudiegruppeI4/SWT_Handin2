using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsbSimulator;

namespace Ladeskab
{
    public class StationControl
    {
        // Enum med tilstande ("states") svarende til tilstandsdiagrammet for klassen
        private enum LadeskabState
        {
            Available,
            Locked,
            DoorOpen
        };

        // Her mangler flere member variable
        private LadeskabState _state;
        private IUsbCharger _charger;
        private IDoor _door;
        private IDisplay _display;
        private IRFIDReader _rfidreader;
        private int _oldId;

        private string logFile = "logfile.txt"; // Navnet på systemets log-fil

        // Her mangler constructor
        StationControl(IUsbCharger charger, IDoor door, IDisplay display, IRFIDReader rfidreader)
        {
            _state = LadeskabState.Available;
            _charger = charger;
            _door = door;
            _display = display;
            _rfidreader = rfidreader;
            _rfidreader.RFIDDetectedEvent += RfidDetected;
        }

        // Eksempel på event handler for eventet "RFID Detected" fra tilstandsdiagrammet for klassen
        private void RfidDetected(object sender, RFIDEventArgs e)
        {
            switch (_state)
            {
                case LadeskabState.Available:
                    // Check for ladeforbindelse
                    if (_charger.Connected)
                    {
                        _door.LockDoor();
                        _charger.StartCharge();
                        _oldId = e.RFID;
                        using (var writer = File.AppendText(logFile))
                        {
                            writer.WriteLine($"{DateTime.Now} : Skab låst med RFID: {e.RFID}");
                        }

                        _display.Display("Skabet er låst og din telefon lades. Brug dit RFID tag til at låse op.");
                        _state = LadeskabState.Locked;
                    }
                    else
                    {
                        _display.Display("Din telefon er ikke ordentlig tilsluttet. Prøv igen.");
                    }

                    break;

                case LadeskabState.DoorOpen:
                    // Ignore
                    break;

                case LadeskabState.Locked:
                    // Check for correct ID
                    if (e.RFID == _oldId)
                    {
                        _charger.StopCharge();
                        _door.UnlockDoor();
                        using (var writer = File.AppendText(logFile))
                        {
                            writer.WriteLine($"{DateTime.Now} : Skab låst op med RFID: {e.RFID}");
                        }

                        _display.Display("Tag din telefon ud af skabet og luk døren");
                        _state = LadeskabState.Available;
                    }
                    else
                    {
                        _display.Display("Forkert RFID tag");
                    }

                    break;
            }
        }

        

        // Her mangler de andre trigger handlere
    }
}
