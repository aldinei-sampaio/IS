namespace IS.Reading.EventObjects
{
    public interface IOpenCloseEventCaller : ISimpleEventCaller
    {
        void Open();
        void Close();
    }
}
