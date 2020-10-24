namespace IS.Reading.EventObjects
{
    public interface IPromptEventCaller<T>
    {
        void Open(T prompt);
    }
}
