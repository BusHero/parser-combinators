var parseA = ParseChar('A');
var parseS = ParseChar('S');
var parseAandS = AndThen(parseA, parseS);
var parseAorS = OrElse(parseA, parseS);

Console.WriteLine(parseAorS("Amdaris"));
Console.WriteLine(parseAorS("Spam"));
Console.WriteLine(parseAorS("Eggs"));



Parser OrElse(Parser p1, Parser p2) => input =>
{
	var r = p1(input);
	if (r is Success)
		return r;
	return p2(input);
};


Parser AndThen(Parser p1, Parser p2) => input =>
{
	var r = p1(input);
	if (r is Success s)
		return p2(s.remaining);
	return r;
};


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
