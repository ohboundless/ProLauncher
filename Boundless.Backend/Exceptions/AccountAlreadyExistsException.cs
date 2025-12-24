namespace Boundless.Backend.Exceptions;

public class AccountAlreadyExistsException(string username) : Exception($"Account already exists with username: '{username}'.");