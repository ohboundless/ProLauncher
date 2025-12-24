namespace Boundless.Backend.Exceptions;

public class IncorrectPasswordException(string username) : Exception($"Incorrect password entered for account '{username}'.");