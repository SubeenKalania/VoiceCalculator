using System;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class CalculatorManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button calculateButton;
    public TextMeshProUGUI outputLabel;
    public Button clearButton;



    // Method to check if a character is a number (including decimal points)
    public bool IsNumber(char ch)
    {
        return char.IsDigit(ch) || ch == '.';
    }

    // Method to apply an operator on two operands
    public double ApplyOperator(double left, double right, char operatorChar)
    {
        switch (operatorChar)
        {
            case '+': return left + right;
            case '-': return left - right;
            case 'x': return left * right;
            case '÷': return left / right;
            default: throw new InvalidOperationException("Invalid operator");
        }
    }

    // Method to check operator precedence
    public int GetPrecedence(char operatorChar)
    {
        if (operatorChar == '+' || operatorChar == '-') return 1;
        if (operatorChar == 'x' || operatorChar == '÷') return 2;
        return 0;
    }

    // Method to evaluate the expression
    public double Evaluate(string expression)
    {
        Stack<double> values = new Stack<double>();    // Stack to hold numbers
        Stack<char> operators = new Stack<char>();     // Stack to hold operators

        int i = 0;
        while (i < expression.Length)
        {
            char currentChar = expression[i];

            // If the current character is a number (may include decimals)
            if (IsNumber(currentChar))
            {
                double value = 0;
                bool isDecimal = false;
                double decimalPlace = 0.1;

                // Extract the full number (to handle multi-digit or decimal numbers)
                while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                {
                    if (expression[i] == '.')
                    {
                        isDecimal = true;
                    }
                    else
                    {
                        if (isDecimal)
                        {
                            value += (expression[i] - '0') * decimalPlace;
                            decimalPlace /= 10;
                        }
                        else
                        {
                            value = value * 10 + (expression[i] - '0');
                        }
                    }
                    i++;
                }
                values.Push(value);
                continue; // Skip to the next character after processing the number
            }

            // If the current character is an opening parenthesis '('
            if (currentChar == '(')
            {
                operators.Push(currentChar);
            }
            // If the current character is a closing parenthesis ')'
            else if (currentChar == ')')
            {
                // Evaluate the sub-expression inside the parentheses
                while (operators.Count > 0 && operators.Peek() != '(')
                {
                    char op = operators.Pop();
                    double right = values.Pop();
                    double left = values.Pop();
                    double result = ApplyOperator(left, right, op);
                    values.Push(result);
                }
                operators.Pop(); // Pop the '(' from the stack
            }
            // If the current character is an operator
            else if (currentChar == '+' || currentChar == '-' || currentChar == 'x' || currentChar == '÷')
            {
                // Handle negative numbers (unary minus)
                if (currentChar == '-' &&
                    (i == 0 || expression[i - 1] == '(' || operators.Contains(expression[i - 1])))
                {
                    // Treat it as a unary minus
                    i++;
                    double negativeValue = 0;
                    bool isDecimal = false;
                    double decimalPlace = 0.1;

                    // Extract the full number after the unary minus
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        if (expression[i] == '.')
                        {
                            isDecimal = true;
                        }
                        else
                        {
                            if (isDecimal)
                            {
                                negativeValue += (expression[i] - '0') * decimalPlace;
                                decimalPlace /= 10;
                            }
                            else
                            {
                                negativeValue = negativeValue * 10 + (expression[i] - '0');
                            }
                        }
                        i++;
                    }

                    values.Push(-negativeValue);
                    continue; // Skip to the next character after processing the negative number
                }

                // While the operator at the top of the stack has greater or equal precedence
                // than the current operator, apply the operator at the top of the stack
                while (operators.Count > 0 && GetPrecedence(operators.Peek()) >= GetPrecedence(currentChar))
                {
                    char op = operators.Pop();
                    double right = values.Pop();
                    double left = values.Pop();
                    double result = ApplyOperator(left, right, op);
                    values.Push(result);
                }
                // Push the current operator to the stack
                operators.Push(currentChar);
            }

            i++;
        }

        // After reading the entire expression, apply the remaining operators in the stack
        while (operators.Count > 0)
        {
            char op = operators.Pop();
            double right = values.Pop();
            double left = values.Pop();
            double result = ApplyOperator(left, right, op);
            values.Push(result);
        }

        // The final result will be the only value left in the values stack
        return values.Pop();
    }

    private void Start()
    {
        calculateButton.onClick.AddListener(CalculateOnButtonCall) ;
        clearButton.onClick.AddListener(Clear);

    }


    public void ConcatenateString(string inputValue)
    {
        int curPos = inputField.caretPosition;
        inputField.text = inputField.text.Insert(curPos,inputValue);
        inputField.caretPosition = curPos + inputValue.Length;

    }
    public void Clear()
    {
        inputField.text = "";
        outputLabel.text = "";

    }
    public void RemoveString()
    {
        int curPos = inputField.caretPosition;
        if (inputField.text.Length > 0 && curPos > 0)
        {
           
            inputField.text = inputField.text.Remove(curPos-1,1);
            inputField.caretPosition = curPos-1;

        }
    }




    private void CalculateOnButtonCall()
    {
        string expression = inputField.text;
        double result = Evaluate(expression);
        print($"Result: {result}");
        outputLabel.text = result.ToString();

    }
}

