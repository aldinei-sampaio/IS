namespace IS.Reading.Events;

public interface IPromptEventCaller<T>
{
    Task OpenAsync(T prompt);
}
