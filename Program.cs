// Firstly show this
Console.WriteLine(ParseChar('A', "Amdaris"));
Console.WriteLine(ParseChar('A', "Spam"));

// Then show this
Console.WriteLine(ParseChar('S', "Amdaris"));
Console.WriteLine(ParseChar('S', "Spam"));


(bool, string) ParseChar(char ch, string input)
{
	if (string.IsNullOrEmpty(input))
		return (false, input);
	if (input[0] != ch)
		return (false, input);
	return (true, input[1..]);
};
