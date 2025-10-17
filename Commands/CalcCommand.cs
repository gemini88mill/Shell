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

    private double EvaluateExpression(string expression)
    {
        // Simple expression evaluator - handles basic arithmetic
        expression = expression.Replace(" ", "");

        // Handle parentheses
        while (expression.Contains('('))
        {
            var start = expression.LastIndexOf('(');
            var end = expression.IndexOf(')', start);
            if (end == -1) throw new ArgumentException("Mismatched parentheses");

            var subExpression = expression.Substring(start + 1, end - start - 1);
            var result = EvaluateSimpleExpression(subExpression);
            expression = expression.Substring(0, start) + result + expression.Substring(end + 1);
        }

        return EvaluateSimpleExpression(expression);
    }

    private double EvaluateSimpleExpression(string expression)
    {
        // Handle multiplication and division first
        var parts = expression.Split(new[] { '+', '-' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            return EvaluateMultiplicationDivision(expression);
        }

        var result = EvaluateMultiplicationDivision(parts[0]);
        var currentIndex = parts[0].Length;

        for (int i = 1; i < parts.Length; i++)
        {
            var operatorIndex = currentIndex;
            while (operatorIndex < expression.Length && expression[operatorIndex] != '+' && expression[operatorIndex] != '-')
                operatorIndex++;

            if (operatorIndex >= expression.Length) break;

            var op = expression[operatorIndex];
            var value = EvaluateMultiplicationDivision(parts[i]);

            if (op == '+')
                result += value;
            else
                result -= value;

            currentIndex = operatorIndex + 1 + parts[i].Length;
        }

        return result;
    }

    private double EvaluateMultiplicationDivision(string expression)
    {
        var parts = expression.Split(new[] { '*', '/' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
        {
            if (double.TryParse(parts[0], out var value))
                return value;
            throw new ArgumentException($"Invalid number: {parts[0]}");
        }

        var result = double.Parse(parts[0]);
        var currentIndex = parts[0].Length;

        for (int i = 1; i < parts.Length; i++)
        {
            var operatorIndex = currentIndex;
            while (operatorIndex < expression.Length && expression[operatorIndex] != '*' && expression[operatorIndex] != '/')
                operatorIndex++;

            if (operatorIndex >= expression.Length) break;

            var op = expression[operatorIndex];
            var value = double.Parse(parts[i]);

            if (op == '*')
                result *= value;
            else
            {
                if (value == 0) throw new ArgumentException("Division by zero");
                result /= value;
            }

            currentIndex = operatorIndex + 1 + parts[i].Length;
        }

        return result;
    }
}
