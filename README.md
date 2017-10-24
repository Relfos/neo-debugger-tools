# NEO-debugger-tools
Suite of development tools for NEO smart contracts.
Includes a cli disassembler and a GUI debugger. An helper library that helps loading .AVM files and create and load .neomap files is also included, and can be used to create other dev tools.

![Debugger Screenshot](images/debugger.png)


## Features
- Supports any NEO .AVM, regardless of the language / compiler used
- Source viewer with syntax highlight powered by ScintillaNET
- Run, step and set breakpoints in order to debug smart contracts
- Toggle between source code and assembly code

## Limitations
- Debugging ASM and C# only for now (see section below how to add new languages)
- Windows only for now, using .NET Framework
- Smart contract source is limited to a single file for now
- Not possible yet to inspect variable values
- Most NEO syscalls/APIs not supported yet (work in progress)

# How to use

Open the .avm file in the NEO-dbg GUI application.
This will show either assembly code for the .avm or C# if a debug map file was found.
Currently the only way to generate a .neomap file is to compile the smart contracts with the modified NeoN compiler include in this repository.

## Shortcuts
| Key        | Action | Comments  |
| ------------- |:-------------:| -----:|
| F5 | Executes the smart contract ||
| F10 | Steps through the smart contract ||
| F12 | Toggle between assembly and source  code | Only works when a .neomap file is available |

# Support for other languages

NEO smart contracts can be coded in many different languages, and in theory, this compiler already supports any language as long as a .neomap file exists in the same directory as the .avm file.
However since only NeoN was modified to emit those map files during compilation, to add other languages it would be necessary to modify other compilers to emit a .neomap.
The .neomap file format is simple, for each line you need to list a starting offset, ending offset, the source line and the corresponding source file, all values separated by a comma.

## Roadmap
- Customize smart contract arguments
- Stack viewer
- Storage emulation
- Transactions emulation
- Persistency of smart contract storage to disk
- Debugger map generation for Java / Python / others
