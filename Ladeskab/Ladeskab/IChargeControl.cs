namespace Ladeskab
{
    public interface IChargeControl
    {
        void StartCharge();
        void StopCharge();
        bool IsConnected();
    }
}