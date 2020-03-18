using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace Ladeskab.Tests
{
    [TestFixture]
    public class LadeskabTests
    {
        //Classes to test
        private StationControl stationControl;
        private Door door;
        private ChargeControl chargeControl;
        private RFIDReader rfidReader;
        private ConcreteDisplay display;
        private UsbChargerSimulator usb;

        //Stubs
        private IDoor stubdoor;
        private IChargeControl stubCharger;
        private IDisplay stubDisplay;
        private IRFIDReader stubRFIDReader;
        private IUsbCharger stubUsb;

        // EventArgs
        private RFIDEventArgs _rfidEventArgs;
        private DoorEventArgs _doorEventArgs;
        private CurrentEventArgs _currentEventArgs;

        [SetUp]
        public void Setup()
        {
            //Creating stubs for dependencies
            stubdoor = Substitute.For<IDoor>();
            stubCharger = Substitute.For<IChargeControl>();
            stubDisplay = Substitute.For<IDisplay>();
            stubRFIDReader = Substitute.For<IRFIDReader>();
            stubUsb = Substitute.For<IUsbCharger>();

            //Initialising classes for tests
            usb = new UsbChargerSimulator();
            rfidReader = new RFIDReader();
            display = new ConcreteDisplay();
            door = new Door();
            chargeControl = new ChargeControl(stubUsb);
            stationControl = new StationControl(stubCharger, stubdoor, stubDisplay, stubRFIDReader);

            _rfidEventArgs = null;
            rfidReader.RFIDDetectedEvent += (o, args) => { _rfidEventArgs = args; };

            _doorEventArgs = null;
            door.DoorEvent += (o, args) => { _doorEventArgs = args; };

            _currentEventArgs = null;
            usb.CurrentValueEvent += (o, args) => { _currentEventArgs = args; };

        }

        /*
         ---ToDo----
         100% coverage
         Jenkins
         Journal

         Write tests for:
            states
            boundary values
            Equivalance partitions
            All states
            All events

         */


        // Testing door

        [Test]
        public void OpenDoorRaisesEvent()
        {
            door.OpenDoor();
            Assert.That(_doorEventArgs.Status, Is.EqualTo("Open"));
        }

        [Test]
        public void CloseDoorRaisesEvent()
        {
            door.CloseDoor();
            Assert.That(_doorEventArgs.Status, Is.EqualTo("Closed"));
        }

        [Test]
        public void LockedDoorCantOpen()
        {
            door.LockDoor();
            door.OpenDoor();
            Assert.That(door.Locked);
            Assert.That(_doorEventArgs, Is.EqualTo(null));
        }

        [Test]
        public void LockedDoorCantClose()
        {
            door.LockDoor();
            door.CloseDoor();
            Assert.That(door.Locked);
            Assert.That(_doorEventArgs, Is.EqualTo(null));
        }

        // Testing RFID Reader

        [TestCase(34)]
        [TestCase(24)]
        [TestCase(100)]
        [TestCase(2)]
        [TestCase(-5)]
        [TestCase(0)]
        public void ScanRFIDRaisesEvent(int rfid)
        {
            rfidReader.ReadRFID(rfid);
            Assert.That(_rfidEventArgs.RFID, Is.EqualTo(rfid));
        }

        // Testing the Usb Charger

        [Test]
        public void ctor_IsConnected()
        {
            Assert.That(usb.Connected, Is.True);
        }

        [Test]
        public void ctor_CurentValueIsZero()
        {
            Assert.That(usb.CurrentValue, Is.Zero);
        }

        [Test]
        public void SimulateDisconnected_ReturnsDisconnected()
        {
            usb.SimulateConnected(false);
            Assert.That(usb.Connected, Is.False);
        }

        [Test]
        public void Started_WaitSomeTime_ReceivedSeveralValues()
        {
            int numValues = 0;
            usb.CurrentValueEvent += (o, args) => numValues++;

            usb.StartCharge();

            System.Threading.Thread.Sleep(1100);

            Assert.That(numValues, Is.GreaterThan(4));
        }

        [Test]
        public void Started_WaitSomeTime_ReceivedChangedValue()
        {
            double lastValue = 1000;
            usb.CurrentValueEvent += (o, args) => lastValue = args.Current;

            usb.StartCharge();

            System.Threading.Thread.Sleep(300);

            Assert.That(lastValue, Is.LessThan(500.0));
        }

        [Test]
        public void StartedNoEventReceiver_WaitSomeTime_PropertyChangedValue()
        {
            usb.StartCharge();

            System.Threading.Thread.Sleep(300);

            Assert.That(usb.CurrentValue, Is.LessThan(500.0));
        }

        [Test]
        public void Started_WaitSomeTime_PropertyMatchesReceivedValue()
        {
            double lastValue = 1000;
            usb.CurrentValueEvent += (o, args) => lastValue = args.Current;

            usb.StartCharge();

            System.Threading.Thread.Sleep(1100);

            Assert.That(lastValue, Is.EqualTo(usb.CurrentValue));
        }


        [Test]
        public void Started_SimulateOverload_ReceivesHighValue()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            double lastValue = 0;

            usb.CurrentValueEvent += (o, args) =>
            {
                lastValue = args.Current;
                pause.Set();
            };

            // Start
            usb.StartCharge();

            // Next value should be high
            usb.SimulateOverload(true);

            // Reset event
            pause.Reset();

            // Wait for next tick, should send overloaded value
            pause.WaitOne(300);

            Assert.That(lastValue, Is.GreaterThan(500.0));
        }

        [Test]
        public void Started_SimulateDisconnected_ReceivesZero()
        {
            ManualResetEvent pause = new ManualResetEvent(false);
            double lastValue = 1000;

            usb.CurrentValueEvent += (o, args) =>
            {
                lastValue = args.Current;
                pause.Set();
            };


            // Start
            usb.StartCharge();

            // Next value should be zero
            usb.SimulateConnected(false);

            // Reset event
            pause.Reset();

            // Wait for next tick, should send disconnected value
            pause.WaitOne(300);

            Assert.That(lastValue, Is.Zero);
        }

        [Test]
        public void SimulateOverload_Start_ReceivesHighValueImmediately()
        {
            double lastValue = 0;

            usb.CurrentValueEvent += (o, args) =>
            {
                lastValue = args.Current;
            };

            // First value should be high
            usb.SimulateOverload(true);

            // Start
            usb.StartCharge();

            // Should not wait for first tick, should send overload immediately

            Assert.That(lastValue, Is.GreaterThan(500.0));
        }

        [Test]
        public void SimulateDisconnected_Start_ReceivesZeroValueImmediately()
        {
            double lastValue = 1000;

            usb.CurrentValueEvent += (o, args) =>
            {
                lastValue = args.Current;
            };

            // First value should be high
            usb.SimulateConnected(false);

            // Start
            usb.StartCharge();

            // Should not wait for first tick, should send zero immediately

            Assert.That(lastValue, Is.Zero);
        }

        [Test]
        public void StopCharge_IsCharging_ReceivesZeroValue()
        {
            double lastValue = 1000;
            usb.CurrentValueEvent += (o, args) => lastValue = args.Current;

            usb.StartCharge();

            System.Threading.Thread.Sleep(300);

            usb.StopCharge();

            Assert.That(lastValue, Is.EqualTo(0.0));
        }

        [Test]
        public void StopCharge_IsCharging_PropertyIsZero()
        {
            usb.StartCharge();

            System.Threading.Thread.Sleep(300);

            usb.StopCharge();

            Assert.That(usb.CurrentValue, Is.EqualTo(0.0));
        }

        [Test]
        public void StopCharge_IsCharging_ReceivesNoMoreValues()
        {
            double lastValue = 1000;
            usb.CurrentValueEvent += (o, args) => lastValue = args.Current;

            usb.StartCharge();

            System.Threading.Thread.Sleep(300);

            usb.StopCharge();
            lastValue = 1000;

            // Wait for a tick
            System.Threading.Thread.Sleep(300);

            // No new value received
            Assert.That(lastValue, Is.EqualTo(1000.0));
        }

        // Testing Station Control

        [Test]
        public void RFIDDetectedStateAvailableChargerConnected()
        {
            stubCharger.IsConnected().Returns(true);
            stubRFIDReader.RFIDDetectedEvent += Raise.EventWith(new RFIDEventArgs() {RFID = 30});
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.Locked));
        }

        [Test]
        public void RFIDDetectedStateAvailableChargerNotConnected()
        {
            stubCharger.IsConnected().Returns(false);
            stubRFIDReader.RFIDDetectedEvent += Raise.EventWith(new RFIDEventArgs() { RFID = 30 });
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.Available));
            stubDisplay.Received(1).Display("Din telefon er ikke ordentlig tilsluttet. Prøv igen.");
        }

        [Test]
        public void RFIDDetectedStateLockedCorrectRFID()
        {
            // Get Station Controls state to locked first
            stubCharger.IsConnected().Returns(true);
            stubRFIDReader.RFIDDetectedEvent += Raise.EventWith(new RFIDEventArgs() { RFID = 30 });

            // Then raise rfid event again
            stubRFIDReader.RFIDDetectedEvent += Raise.EventWith(new RFIDEventArgs() {RFID = 30});
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.Available));
            stubDisplay.Received(1).Display("Tag din telefon ud af skabet og luk døren");
        }

        [Test]
        public void RFIDDetectedStateLockedWrongRFID()
        {
            // Get Station Controls state to locked first
            stubCharger.IsConnected().Returns(true);
            stubRFIDReader.RFIDDetectedEvent += Raise.EventWith(new RFIDEventArgs() { RFID = 30 });

            // Then raise rfid event again
            stubRFIDReader.RFIDDetectedEvent += Raise.EventWith(new RFIDEventArgs() { RFID = 25 });
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.Locked));
            stubDisplay.Received(1).Display("Forkert RFID tag");
        }

        [Test]
        public void DoorEventStateAvailable_OpenDoor()
        {
            stubdoor.DoorEvent += Raise.EventWith(new DoorEventArgs() {Status = "Open"});
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.DoorOpen));
            stubDisplay.Received(1).Display("Tilslut telefon");
        }

        [Test]
        public void DoorEventStateAvailable_CloseDoor()
        {
            stubdoor.DoorEvent += Raise.EventWith(new DoorEventArgs() { Status = "Closed" });
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.Available));
        }

        [Test]
        public void DoorEventStateDoorOpen_OpenDoor()
        {
            stubdoor.DoorEvent += Raise.EventWith(new DoorEventArgs() { Status = "Open" });
            stubdoor.DoorEvent += Raise.EventWith(new DoorEventArgs() { Status = "Open" });
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.DoorOpen));
        }

        [Test]
        public void DoorEventStateDoorOpen_CloseDoor()
        {
            stubdoor.DoorEvent += Raise.EventWith(new DoorEventArgs() { Status = "Open" });
            stubdoor.DoorEvent += Raise.EventWith(new DoorEventArgs() { Status = "Closed" });
            Assert.That(stationControl.State, Is.EqualTo(StationControl.LadeskabState.Available));
            stubDisplay.Received(1).Display("Indlæs RFID");
        }

        // *** _charger mangler subscription på CurrentValueChanged evented ***
        // Lav Tests om til TestCases!!

        [Test]
        public void CurrentValueEvent_ValueBetweenZeroAndFive()
        {
            stubUsb.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() {Current = 3});
            stubDisplay.Received(1).Display("Phone is fully charged, please disconnect..");
        }

        [Test]
        public void CurrentValueEvent_ValueBetweenFiveAndFiveHundred()
        {
            stubUsb.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 230 });
            stubDisplay.Received(1).Display("Phone is charging..");
        }

        [Test]
        public void CurrentValueEvent_ValueLargerThanFiveHundred()
        {
            stubUsb.CurrentValueEvent += Raise.EventWith(new CurrentEventArgs() { Current = 560 });
            stubDisplay.Received(1).Display("Something went wrong charging the phone, please disconnect immediately..");
        }
    }
}
