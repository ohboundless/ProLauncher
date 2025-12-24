namespace Boundless.Backend.Exceptions;

public class InvalidUsernameException(string username) : Exception($"Username '{username}' did not match any account.");