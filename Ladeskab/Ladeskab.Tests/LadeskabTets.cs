using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;

namespace Ladeskab.Tests
{
    class LadeskabTets
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
                var wasCalled = false;
                door.DoorEvent += (sender, args) => wasCalled = true;
                door.OpenDoor();
                Assert.That(wasCalled);
            }

            public void CloseDoorRaisesEvent()
            {
                var wasCalled = false;
                door.DoorEvent += (sender, args) => wasCalled = true;
                door.CloseDoor();
                Assert.That(wasCalled);
            }

            [Test]
            public void LockedDoorCantOpen()
            {
                door.LockDoor();
                door.OpenDoor();
                Assert.That(door.Locked);
            }

            //More?

            // Testing RFID Reader

            [Test]
            public void ScanRFIDRaisesEvent()
            {
                var wasCalled = false;
                rfidReader.RFIDDetectedEvent += (sender, args) => wasCalled = true;
                rfidReader.ReadRFID(34);
                Assert.That(wasCalled);
            }
            //More? Maybe somehow check the int is correctly send? Or maybe that test should be in the StationControl tests?

        }
    }
}
