namespace Shell.Commands;

public class CalcCommand : ICommand
{
    public string Name => "calc";
    public string Description => "Evaluate a simple math expression";

    public void Execute(string[] args)
    {
        if (args.Length > 1)
        {
            var expression = string.Join(" ", args.Skip(1));
            try
            {
                var result = EvaluateExpression(expression);
                Logger.Success($"{expression} = {result}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error evaluating expression: {ex.Message}");
            }
        }
        else
        {
            Logger.Warning("Usage: calc <expression>");
        }
    }

    // Local evaluator copied from Program.cs for isolation
    private double EvaluateExpression(string expression)
    {
        // Very basic expression evaluator supporting +, -, *, / and parentheses
        var tokens = Tokenize(expression);
        var pos = 0;

        double ParseExpression()
        {
            double value = ParseTerm();
            while (pos < tokens.Count)
            {
                var op = tokens[pos];
                if (op == "+" || op == "-")
                {
                    pos++;
                    var right = ParseTerm();
                    value = op == "+" ? value + right : value - right;
                }
                else break;
            }
            return value;
        }

        double ParseTerm()
        {
            double value = ParseFactor();
            while (pos < tokens.Count)
            {
                var op = tokens[pos];
                if (op == "*" || op == "/")
                {
                    pos++;
                    var right = ParseFactor();
                    value = op == "*" ? value * right : value / right;
                }
                else break;
            }
            return value;
        }

        double ParseFactor()
        {
            if (pos >= tokens.Count) throw new Exception("Unexpected end of expression");
            var token = tokens[pos++];
            if (token == "+") return ParseFactor();
            if (token == "-") return -ParseFactor();
            if (token == "(")
            {
                var value = ParseExpression();
                if (pos >= tokens.Count || tokens[pos] != ")") throw new Exception("Missing closing parenthesis");
                pos++;
                return value;
            }
            if (double.TryParse(token, out var number)) return number;
            throw new Exception($"Invalid token: {token}");
        }

        return ParseExpression();
    }

    private List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var current = "";
        foreach (var c in expression)
        {
            if (char.IsWhiteSpace(c))
            {
                if (current.Length > 0) { tokens.Add(current); current = ""; }
            }
            else if ("+-*/()".Contains(c))
            {
                if (current.Length > 0) { tokens.Add(current); current = ""; }
                tokens.Add(c.ToString());
            }
            else
            {
                current += c;
            }
        }
        if (current.Length > 0) tokens.Add(current);
        return tokens;
    }
}
