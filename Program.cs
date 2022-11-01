Console.WriteLine(ParseA("Amdaris"));
Console.WriteLine(ParseA("Spam"));

(bool, string) ParseA(string input)
{
	if (string.IsNullOrEmpty(input))
		return (false, input);
	if (input[0] != 'A')
		return (false, input);
	return (true, input[1..]);
}
