# NEO-dev-tools
Suite of development tools for NEO smart contracts.
Includes a cli disassembler and a GUI debugger. An helper library that helps loading .AVM files and create and load .neomap files is also included, and can be used to create other dev tools.

# Limitations
- Debugging ASM and C# only for now (see section below how to add new languages)
- Windows only for now, using .NET Framework
- Not possible yet to inspect variable values
- Most NEO syscalls/APIs not supported yet (work in progress)

# How to use

Open the .avm file in the NEO-dbg GUI application.
This will show either assembly code for the .avm or C# if a debug map file was found.
Currently the only way to generate a .neomap file is to compile the smart contracts with the modified NeoN compiler include in this repository.
