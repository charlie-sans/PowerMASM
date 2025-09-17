# PowerMASM

**PowerMASM** is a powerful assembler and interactive micro-assembly editor for the Micro-Assembly CPU lineup. It features a modern Blazor web interface, advanced macro system, and robust error reporting.

## Features
- Supports all Micro-Assembly CPU instructions and directives
- Powerful macro system for reusable code
- Advanced optimization techniques for efficient code generation
- User-friendly syntax and error reporting
- Interactive web-based editor with syntax highlighting (Ace Editor)
- Snippets panel for quick code insertion
- Extensible architecture (.NET 9)

## Getting Started

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- Modern web browser (if using the web editor))

### Running the Web Editor

1. Clone the repository:
   ```sh
   git clone https://github.com/charlie-sans/PowerMASM.git
   cd PowerMASM
   ```
2. Build and run the Blazor web project:
   ```sh
   dotnet run --project PowerMASM.Web.Runtime
   ```
3. Open your browser and navigate to `http://localhost:5000` (or the port shown in the console).

### Using the Editor
- Write micro-assembly code in the editor panel.
- Use the snippets panel to insert common code patterns.
- Click **Run** to assemble and execute your code.
- View output and error messages below the editor.

## Project Structure
- `PowerMASM.Core` – Core assembler logic and CPU definitions
- `PowerMASM.Web.Runtime` – Blazor web editor and runtime
- `PowerMASM.Runtime` – Execution engine for micro-assembly programs
- `PowerMASM.UIDebugger` – Debugging tools (optional)
- `PowerMASM.Test` – Unit tests
- `PowerMASM.Extensions` – Additional language extensions
- `PowerMASM.Lisp` – Lisp integration (optional)
- `UICML` – UI components and markup language

## License
This project is licensed under the [GNU GPL v3](LICENSE.txt).

## Contributing
Contributions are welcome!
Please open issues or pull requests for bug fixes, features, or documentation improvements.

## Contact
For questions or support, open an issue on GitHub or contact the maintainer.

