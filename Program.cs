using System.Collections.Immutable;
var number = ParseToken(ParseInt());

var expressionPlaceholder = new ParserPlaceholder<int>();
var factor = ParseFactor(expressionPlaceholder.Parser);
var term = ParseTerm();
var expression = ParseExpression();
expressionPlaceholder.Initialize(expression);

while (true)
{
	var line = Console.ReadLine();
	if (expression(line) is Success<int> s)
	{
		Console.WriteLine(s.result);
	}
	else
	{
		Console.WriteLine("Invalid expression");
	}
}

Parser<T> ParseToken<T>(Parser<T> parser)
{
	var manySpaces = Many(ParseChar(' '));
	return Between(manySpaces, parser, manySpaces);
}

Parser<int> ParseExpression()
{
	var plus = ParseToken(ParseChar('+'));
	var minus = ParseToken(ParseChar('-'));
	var operatorAndTerm = AndThen(OrElse(plus, minus), term);
	return Map(
		AndThen(term, Many(operatorAndTerm)),
		tuple => tuple.Item2.Aggregate(
			tuple.Item1,
			(first, second) =>
			{
				return second.Item1 switch
				{
					'+' => first + second.Item2,
					'-' => first - second.Item2,
					_ => throw new Exception("???")
				};
			})
	);
}

Parser<int> ParseTerm()
{
	var multiply = ParseToken(ParseChar('*'));
	var divide = ParseToken(ParseChar('/'));
	var operatorAndFactor = AndThen(OrElse(multiply, divide), factor);
	return Map(
		AndThen(factor, Many(operatorAndFactor)),
		tuple => tuple.Item2.Aggregate(
			tuple.Item1,
			(first, second) =>
			{
				return second.Item1 switch
				{
					'*' => first * second.Item2,
					'/' => first / second.Item2,
					_ => throw new Exception("???")
				};
			})
	);
}

Parser<int> ParseFactor(Parser<int> expression)
{
	var rightParanthesis = ParseToken(ParseChar('('));
	var leftParanthesis = ParseToken(ParseChar(')'));
	return OrElse(
		number,
		Between(rightParanthesis, expression, leftParanthesis));
}

Parser<ImmutableList<T>> ParseList<T>(params Parser<T>[] parsers)
{
	var result = Return(ImmutableList.Create<T>());
	foreach (var parser in parsers)
	{
		var foo = AndThen(result, parser);
		result = Map(foo, tuple => tuple.Item1.Add(tuple.Item2));
	}
	return result;
}

Parser<T> Return<T>(T value) => input => new Success<T>(value, input);

Parser<TU> Between<T, TU, TZ>(Parser<T> right, Parser<TU> parser, Parser<TZ> left)
{
	var rightThenParserThenLeft = AndThen(right, AndThen(parser, left));
	var middle = Map(rightThenParserThenLeft, tuple => tuple.Item2.Item1);
	return middle;
};

Parser<int> ParseInt()
{
	var digitParser = AnyOfChar('0', '1', '2', '3', '4', '5', '6', '7', '8', '9');
	var manyDigits = Many1(digitParser);
	var positiveInteger = Map(manyDigits, list => int.Parse(string.Join("", list)));

	var optionalMinus = ParserOptional(ParseChar('-'));

	var optionalMinusAndPositiveInteger = AndThen(optionalMinus, positiveInteger);
	var integer = Map(optionalMinusAndPositiveInteger, tuple =>
	{
		if (tuple.Item1.HasValue)
		{
			return -tuple.Item2;
		}
		return tuple.Item2;
	});
	return integer;
}

Parser<Optional<T>> ParserOptional<T>(Parser<T> parser) => input =>
{
	var result = parser(input);
	if (result is Success<T> s)
	{
		return new Success<Optional<T>>(new Optional<T>(s.result), s.remaining);
	}
	return new Success<Optional<T>>(new Optional<T>(), input);
};

Parser<char> AnyOfChar(params char[] characters)
{
	var characterParsers = characters
		.Select(ch => ParseChar(ch))
		.ToArray();
	return AnyOf(characterParsers);
}

Parser<T> AnyOf<T>(params Parser<T>[] parsers)
{
	return parsers.Aggregate((first, second) => OrElse(first, second));
}

Parser<List<T>> Many1<T>(Parser<T> parser) => input =>
{
	var result = parser(input);
	if (result is Success<T> s)
	{
		var list = new List<T> { s.result };
		var remaining = s.remaining;
		if (Many(parser)(s.remaining) is Success<List<T>> s2)
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
		else if (r2 is Failure<U> f2)
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


struct Optional<T>
{
	public T Value { get; }
	public bool HasValue { get; }

	public Optional()
	{
		Value = default;
		HasValue = false;
	}

	public Optional(T value)
	{
		Value = value;
		HasValue = true;
	}

	public override string ToString() => HasValue ? $"Some({Value})" : "None";
}

class ParserPlaceholder<T>
{
	public ParserPlaceholder()
	{
		Parser = input =>
		{
			if (_parser is null)
			{
				throw new InvalidOperationException("Parser is not initialized. Forgot to call Initialize()?");
			}
			return _parser(input);
		};
	}

	public void Initialize(Parser<T> parser) => this._parser = parser;

	private Parser<T> _parser;
	public Parser<T> Parser { get; }
}
