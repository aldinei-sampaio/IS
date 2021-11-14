namespace IS.Reading.EventObjects;

public interface IPromptEventCaller<T>
{
    Task OpenAsync(T prompt);
}
