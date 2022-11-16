var parseA = ParseChar('A');
var parseS = ParseChar('S');
var parseE = ParseChar('E');
var parseAandS = AndThen(parseA, parseS);
var manyAandS = Map(Many(parseAandS), r => '[' + string.Join(",", r) + ']');
var manyA = Map(Many(parseA), r => '[' + string.Join(",", r) + ']');
var many1A = Map(Many1(parseA), r => '[' + string.Join(",", r) + ']');

Console.WriteLine(many1A("ABC"));
Console.WriteLine(many1A("BCD"));



Parser<List<T>> Many1<T>(Parser<T> parser) => input =>
{
	var result = parser(input);
	if (result is Success<T> s)
	{
		var list = new List<T> { s.result };
		var remaining = s.remaining;
		if (Many(parser)(input) is Success<List<T>> s2)
		{
			list.AddRange(s2.result);
			remaining = s2.remaining;
		}
		return new Success<List<T>>(list, remaining);
	}
	return new Failure<List<T>>("Many1 failed");
};

Parser<List<T>> Many<T>(Parser<T> parser) => input =>
{
	var list = new List<T>();
	var remaining = input;
	while (parser(remaining) is Success<T> s)
	{
		list.Add(s.result);
		remaining = s.remaining;
	}
	return new Success<List<T>>(list, remaining);
};


Parser<U> Map<T, U>(Parser<T> parser, Func<T, U> mapper) => input =>
{
	var r = parser(input);
	if (r is Success<T> s)
	{
		return new Success<U>(mapper(s.result), s.remaining);
	}
	else if (r is Failure<T> f)
	{
		return new Failure<U>(f.Message);
	}
	throw new Exception("???");
};

Parser<T> OrElse<T>(Parser<T> p1, Parser<T> p2) => input =>
{
	var r = p1(input);
	if (r is Success<T>)
		return r;
	return p2(input);
};


Parser<(T, U)> AndThen<T, U>(Parser<T> p1, Parser<U> p2) => input =>
{
	var r = p1(input);
	if (r is Success<T> s)
	{
		var r2 = p2(s.remaining);
		if (r2 is Success<U> s2)
		{
			return new Success<(T, U)>((s.result, s2.result), s2.remaining);
		}
		else if (r2 is Failure<T> f2)
		{
			return new Failure<(T, U)>(f2.Message);
		}
		throw new Exception("???");
	}
	else if (r is Failure<T> f)
	{
		return new Failure<(T, U)>(f.Message);
	}
	throw new Exception("???");
};


Parser<char> ParseChar(char ch) => input =>
{
	if (string.IsNullOrEmpty(input))
		return new Failure<char>("input is null or empty");
	if (input[0] != ch)
		return new Failure<char>($"The first letter is '{input[0]}', instead of '{ch}'");
	return new Success<char>(ch, input[1..]);
};

delegate Result<T> Parser<T>(string input);

record Result<T>;
record Success<T>(T result, string remaining) : Result<T>;
record Failure<T>(string Message = "") : Result<T>;
