namespace IS.Reading.EventObjects;

public interface ISimpleEventCaller
{
    Task ChangeAsync(string value);
}
