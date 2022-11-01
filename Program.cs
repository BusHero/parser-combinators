﻿Console.WriteLine(ParseChar('A')("Amdaris"));
Console.WriteLine(ParseChar('A')("Spam"));
Console.WriteLine(ParseChar('S')("Amdaris"));
Console.WriteLine(ParseChar('S')("Spam"));



Func<string, Result> ParseChar(char ch) => input =>
{
	if (string.IsNullOrEmpty(input))
		return new Failure("input is null or empty");
	if (input[0] != ch)
		return new Failure($"The first letter is '{input[0]}', instead of '{ch}'");
	return new Success(ch, input[1..]);
};

record Result;
record Success(char result, string remaining) : Result;
record Failure(string Message = "") : Result;
