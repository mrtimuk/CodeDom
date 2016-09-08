# CodeDom

This creates an abstract syntax tree for a "Hello, World!" program, and from this produces a C# source file and compiled binary assembly.

The generated code imports the `System` namespace into the global namespace, then creates a local namespace containing a static class with a single static method which calls `Console.WriteLine` to output the required text.

`CSharpCodeProvider` is used to generate the source code and binary from a `CodeCompileUnit` which contains the abstract syntax tree that we have constructed.
