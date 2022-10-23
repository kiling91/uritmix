using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Helpers.DataAccess.Relational.Query;

/// <summary>
///     [CustomAuthorize(Roles = "A && (!B || C) ^ D")]
///     https://gist.github.com/meziantou/10603804
/// </summary>
public class ExpressionParser
{
    private static Node Parse(string text)
    {
        if (text == null) throw new ArgumentNullException(nameof(text));

        var tokens = new List<string>();
        var sb = new StringBuilder();

        foreach (var c in text)
            switch (c)
            {
                case ' ':
                case '\t':
                case '\r':
                case '\n':
                    if (sb.Length == 0)
                        continue;
                    sb.Append(c);
                    break;
                case '{':
                case '}':
                case '!':
                    if (sb.Length > 0)
                    {
                        tokens.Add(sb.ToString());
                        sb.Clear();
                    }

                    tokens.Add(c.ToString(CultureInfo.InvariantCulture));
                    break;
                case '&':
                case '|':
                    if (sb.Length != 0)
                    {
                        var prev = sb[^1];
                        if (c == prev) // && or ||
                        {
                            sb.Remove(sb.Length - 1, 1); // remove last char
                            var t = sb.ToString().Trim();
                            if (t != "")
                                tokens.Add(sb.ToString().Trim());
                            sb.Clear();

                            tokens.Add(c == '&' ? "&&" : "||");
                            break;
                        }
                    }

                    sb.Append(c);
                    break;

                default:
                    sb.Append(c);
                    break;
            }

        if (sb.Length > 0) tokens.Add(sb.ToString());

        return Parse(tokens.ToArray());
    }

    private static Node Parse(string[] tokens)
    {
        var index = 0;
        return ParseExp(tokens, ref index);
    }

    private static Node ParseExp(string[] tokens, ref int index)
    {
        var leftExp = ParseSubExp(tokens, ref index);
        if (index >= tokens.Length)
            return leftExp;

        var token = tokens[index];

        if (token == "&&")
        {
            index++;
            var rightExp = ParseExp(tokens, ref index);
            return new AndNode(leftExp, rightExp);
        }

        if (token == "||")
        {
            index++;
            var rightExp = ParseExp(tokens, ref index);
            return new OrNode(leftExp, rightExp);
        }

        throw new InvalidDataException("Expected '&&' or '||' or EOF");
    }

    private static Node ParseSubExp(string[] tokens, ref int index)
    {
        var token = tokens[index];

        if (token == "{")
        {
            index++;
            var node = ParseExp(tokens, ref index);

            //if (tokens[index] != ")")
            //    throw new Exception("Expected ')'");

            //index++; // Skip ')'
            while (index < tokens.Length && tokens[index] == "}")
                index++;

            return node;
        }

        if (token == "!")
        {
            index++;
            var node = ParseExp(tokens, ref index);
            return new NotNode(node);
        }

        index++;
        while (index < tokens.Length && tokens[index] == "}")
            index++;
        return new RoleNode(token);
    }

    private string? RoleName(string roleName)
    {
        var array = roleName.Trim().Split("^^");
        if (array.Length != 3)
            return null;

        var field = array[0];
        var expression = array[2];
        var type = array[1];

        return type switch
        {
            "contains" => $"{field}.Contains(\"{expression}\")",
            "notcontains" => $"not {field}.Contains(\"{expression}\")",
            "startswith" => $"{field}.StartsWith(\"{expression}\")",
            "endswith" => $"{field}.EndsWith(\"{expression}\")",
            "=" => $"{field} == \"{expression}\"",
            "<>" => $"{field} <> \"{expression}\"",
            _ => throw new InvalidDataException("Type Error")
        };
    }

    private string? ToLinq(Node node)
    {
        switch (node)
        {
            case BinaryNode binaryNode:
            {
                var res = "(";
                res += ToLinq(binaryNode.LeftExpression);
                if (node is AndNode)
                    res += " and ";
                if (node is OrNode)
                    res += " or ";
                res += ToLinq(binaryNode.RightExpression);
                res += ")";
                return res;
            }
            case RoleNode roleNode:
                return RoleName(roleNode.RoleName);
            default:
                throw new ArgumentException(nameof(node));
        }
    }

    public string? Filter(string filterExpression)
    {
        var expression = Parse(filterExpression);
        return ToLinq(expression);
    }
    /*
    *  Exp -> SubExp '&&' Exp // AND
    *  Exp -> SubExp '||' Exp // OR
    *  Exp -> SubExp '^' Exp  // XOR
    *  SubExp -> '(' Exp ')'
    *  SubExp -> '!' Exp         // NOT
    *  SubExp -> RoleName
    *  RoleName -> [a-z0-9]
    */

    private abstract class Node
    {
    }

    private abstract class UnaryNode : Node
    {
        protected UnaryNode(Node expression)
        {
            Expression = expression;
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        private Node Expression { get; }
    }

    private abstract class BinaryNode : Node
    {
        protected BinaryNode(Node leftExpression, Node rightExpression)
        {
            LeftExpression = leftExpression;
            RightExpression = rightExpression;
        }

        public Node LeftExpression { get; }

        public Node RightExpression { get; }
    }

    private class AndNode : BinaryNode
    {
        public AndNode(Node leftExpression, Node rightExpression)
            : base(leftExpression, rightExpression)
        {
        }
    }

    private class OrNode : BinaryNode
    {
        public OrNode(Node leftExpression, Node rightExpression)
            : base(leftExpression, rightExpression)
        {
        }
    }

    private class NotNode : UnaryNode
    {
        public NotNode(Node expression)
            : base(expression)
        {
        }
    }

    private class RoleNode : Node
    {
        public RoleNode(string roleName)
        {
            RoleName = roleName;
        }

        public string RoleName { get; }
    }
}