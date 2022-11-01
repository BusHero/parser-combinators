var parseA = ParseChar('A');
var parseS = ParseChar('S');
Console.WriteLine(parseA("Amdaris"));
Console.WriteLine(parseA("Spam"));
Console.WriteLine(parseS("Amdaris"));
Console.WriteLine(parseS("Spam"));



Parser ParseChar(char ch) => input =>
{
	if (string.IsNullOrEmpty(input))
		return new Failure("input is null or empty");
	if (input[0] != ch)
		return new Failure($"The first letter is '{input[0]}', instead of '{ch}'");
	return new Success(ch, input[1..]);
};

delegate Result Parser(string input);

record Result;
record Success(char result, string remaining) : Result;
record Failure(string Message = "") : Result;
