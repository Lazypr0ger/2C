namespace Contracts.Exceptions;

public class IncorrectDatesException : Exception
{
	public IncorrectDatesException(DateTime start, DateTime end) : base($"The end date must be later than the start date.. StartDate: {start:dd.MM.YYYY}. EndDate: {end:dd.MM.YYYY}") { }
}