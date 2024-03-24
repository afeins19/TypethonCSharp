# Typethon Compiler 

This is a compiler written in C# which will implement our custom language Typethon.
Typethon is a strongly-typed language with bracketed delimiters. 

### REPL 
the first stage of this project involves implementing a basic lexical analyzer 
and parser, as well as a REPL for quick evaluation.

### Parse Tree
Say we have an expression of the following form: `1 + 2 * 3`. We'd like to build
a parse tree in the following way: 
```
        +
       / \
      1   *
         / \
        2   3
```     

The current Parse tree is given through the console in the following way:
```
c[_] >> 1 + 2 + 3

BinaryExpression
    BinaryExpression
        NumberExpression
            NumberToken  1  
        PlusToken
        NumberExpression
            NumberToken  2
    PlusToken
    NumberExpression
        NumberToken  3
```


