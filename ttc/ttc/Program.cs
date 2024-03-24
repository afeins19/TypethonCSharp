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
            
            // temporary exit condition (empty line) 
            if (string.IsNullOrEmpty(line))
                return;

            var lexer = new Lexer(line);
            while (true)
            {
                var token = lexer.NextToken();
                
                if (token.Kind == SyntaxKind.EndOfFileToken)
                    break; // end of file reached 
                
                Console.Write($"{ token.Kind } : '{ token.Text }' ");
                if (token.Value != null)
                    Console.Write($" | val={token.Value}");
                
                Console.WriteLine();
                

            }

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
    EndOfFileToken
}

class SyntaxToken
{
    public SyntaxToken(SyntaxKind kind, int position, string text, object value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value; 
    }
    
    public SyntaxKind Kind { get; } 
    public int Position { get; }
    public string Text { get;  }
   public object Value { get;  }
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

class SyntaxNode
{
    
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

        if (index >= _tokens.length)
            return _tokens[_tokens.Length - 1];

        return _tokens[index]; 
    }
    
    private SyntaxToken Current => Peek(0); 
}