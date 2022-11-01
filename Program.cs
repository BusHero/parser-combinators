// Firstly show this
Console.WriteLine(ParseChar('A', "Amdaris"));
Console.WriteLine(ParseChar('A', "Spam"));

// Then show this
Console.WriteLine(ParseChar('S', "Amdaris"));
Console.WriteLine(ParseChar('S', "Spam"));


// int -> int -> int
var f1 = (int p1, int p2) => p1 * p2;
var f2 = (int p1) => (int p2) => p1 * p2;
Console.WriteLine(f1(6, 7));
Console.WriteLine(f2(6)(7));



Result ParseChar(char ch, string input)
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
