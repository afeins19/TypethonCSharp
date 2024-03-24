using System.Reflection;

namespace ttc;

class Program
{
    static void Main(string[] args)
    {
        // lets us add wacky ASCII chars 
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        // REPL - reads user input while the program is running 
        while (true)
        {   
            Console.Write("c[_] >> ");
            
            var line = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
                return;
            
            var parser = new Parser(line);
            var expression = parser.Parse();

            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green; 
            
            PrettyPrint(expression);
            Console.ForegroundColor = color; // reset the color for non-tree spaces 
            
        }
        
    }

    static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = false)
    {
        
        
        
        var marker = isLast ? "\u2514\u2500\u2500" : "\u251c\u2500\u2500";
        
        Console.Write(indent);
        Console.Write(marker);
        Console.Write(node.Kind);

        if (node is SyntaxToken t && t.Value != null)
        {
            Console.Write(" ");
            Console.Write(t.Value);
            Console.Write("  ");
        }
        
        Console.WriteLine();
        
        //indent += "    "; // 4 spaces between nodes 
        indent += isLast ? "    " : "|   "; // unix tree type styling structure 

        var lastChild = node.GetChildren().LastOrDefault(); 

        foreach (var child in node.GetChildren())
        {
            // print the 'last' indent type if the node matches the last node of the children 
            PrettyPrint(child, indent, node==lastChild);
        }
    }
}

// defines the kinds of syntax components we have (operator, literal, identifier, etc.) 
enum SyntaxKind
{
    NumberToken, 
    WhiteSpaceToken, 
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken, 
    LeftParentheseToken,
    RightParenteseToken,
    
    BadToken,
    EndOfFileToken,
    
    NumberExpression,
    BinaryExpression
}

class SyntaxToken : SyntaxNode
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value; 
    }
    
    public override SyntaxKind Kind { get; } 
    public int Position { get; }
    public string Text { get;  }
    public object Value { get;  }
   
   public override IEnumerable<SyntaxNode> GetChildren() // abstract enumeration for easily walkin nodes of the tree
   {
       return Enumerable.Empty<SyntaxNode>(); 
   }
}

class Lexer
{
    // constructor 
    public Lexer(string text)
    {
        _text = text;
    }
    
    // class variable for the text field (global to this class)
    private readonly string _text;
    private int _position; 
    
    // gets the current char 
    private char Current
    {
        get
        {
            if (_position >= _text.Length)
                return '\0'; // invlaid position -> return zero-operator 

            return _text[_position]; 
        }
    }

    private void Next()
    {
        _position++; 
    }
    
    public SyntaxToken NextToken()
    {
        // numbers 
        // operators  
        // whitespace 
        
        // end of file token case 
        if (_position >= _text.Length)
            return new SyntaxToken(SyntaxKind.EndOfFileToken, _position, "\0", null);
        
        if (char.IsDigit(Current))
        {
            var start = _position;

            while (char.IsDigit(Current))
                Next();

            var length = _position - start;
            var text = _text.Substring(start, length); // keep reading until the current string is no longer a num 
            int.TryParse(text, out var value); // needs error handling 
            return new SyntaxToken(SyntaxKind.NumberToken, start, text, value); 
        }

        if (char.IsWhiteSpace(Current))
        {
            var start = _position;

            while (char.IsWhiteSpace(Current))
                Next();

            var length = _position - start;
            var text = _text.Substring(start, length); // keep reading until the current string is no longer a num 
            int.TryParse(text, out var value); // needs error handling 
            return new SyntaxToken(SyntaxKind.WhiteSpaceToken, start, text, null); 
        }
        
        if (Current == '+')
            return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null); 
        else if (Current == '-')
            return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null);
        else if (Current == '*')
            return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
        else if (Current == '/')
            return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
        else if (Current == '(')
            return new SyntaxToken(SyntaxKind.LeftParentheseToken, _position++, "(", null);
        else if (Current == ')')
            return new SyntaxToken(SyntaxKind.RightParenteseToken, _position++, ")", null); 
        
        // invalid operator return 
        return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1), null); 
        
        // operator syntax token creation 
        // switch (Current)
        // {
        //     case '+': return new SyntaxToken(SyntaxKind.PlusToken, _position++, "+", null); 
        //     case '-' : return new SyntaxToken(SyntaxKind.MinusToken, _position++, "-", null); 
        //     case '*' : return new SyntaxToken(SyntaxKind.StarToken, _position++, "*", null);
        //     case '/' : return new SyntaxToken(SyntaxKind.SlashToken, _position++, "/", null);
        //     case '(' : return new SyntaxToken(SyntaxKind.LeftParentheseToken, _position++, "(", null);
        //     case ')' : return new SyntaxToken(SyntaxKind.RightParenteseToken, _position++, ")", null); 
        // }
        
        // invalid (bad) syntax token was enetered :( 
        //return new SyntaxToken(SyntaxKind.BadToken, _position++, _text.Substring(_position - 1), null); 
    }
}

abstract class SyntaxNode
{
    // behavior will be implemented based on the kind? 
    public abstract SyntaxKind Kind { get; }
    
    // iterator object for the node (vale, text, type etc.)
    public abstract IEnumerable<SyntaxNode> GetChildren(); 

}


abstract class ExpressionSyntax : SyntaxNode
{
    
}

sealed class NumberExpressionSyntax : ExpressionSyntax
{
    public NumberExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken; 
    }

    public override SyntaxKind Kind => SyntaxKind.NumberExpression; 
    public SyntaxToken NumberToken { get; }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken; 
    }
}

// operators like +, -, * etc. have the structure <leftexp> <operator> <rightexp> 
// sealed implies it cant be further inherited 
sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Left = left;
        OperatorToken = operatorToken; 
        Right = right; 
    }
    
    public ExpressionSyntax Left { get;  }
    public SyntaxToken OperatorToken { get;  }
    public ExpressionSyntax Right { get; }

    // override means it overrides an abstract property from the parent class 
    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        // statefull iterators...lets you create an iterator tuple? 
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}

class Parser
{
    // internal storage of tokens 
    private readonly SyntaxToken[] _tokens;
    private int _position; 

    public Parser(string text)
    {
        // instantiating our list of tokens 
        var tokens = new List<SyntaxToken>(); 
        
        // instantiating our Lexer for reading the file 
        var lexer = new Lexer(text);
        
        SyntaxToken token;
        do
        {
            token = lexer.NextToken();
            
            // add non-whitespace and in-range tokens 
            if (token.Kind != SyntaxKind.WhiteSpaceToken && token.Kind != SyntaxKind.BadToken)
                tokens.Add(token);
                
        } while (token.Kind != SyntaxKind.EndOfFileToken);

        _tokens = tokens.ToArray(); 
    }
    
    // quick look behind until non-eof position is reached 
    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;

        if (index >= _tokens.Length)
            return _tokens[_tokens.Length - 1];

        return _tokens[index]; 
    }
    
    private SyntaxToken Current => Peek(0);

    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current; 
    }
    
    private SyntaxToken Match(SyntaxKind kind)
    {
        // match the syntax tokenm 
        if (Current.Kind == kind)
            return NextToken();
        return new SyntaxToken(kind, _position, null, null); 
    }
    public ExpressionSyntax Parse()
    {
        // parse leaves and build strucutres on top of that 
        var left = ParsePrimaryExpression();
        
        
        // building a binary expression - <l> <op> <r> 
        while (Current.Kind == SyntaxKind.PlusToken || Current.Kind == SyntaxKind.MinusToken)
        {
            var operatorToken = NextToken();
            var right = ParsePrimaryExpression();
            left = new BinaryExpressionSyntax(left, operatorToken, right); 
        }

        return left; 
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        var numberToken = Match(SyntaxKind.NumberToken);
        return new NumberExpressionSyntax(numberToken);
    }
}