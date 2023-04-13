# MemoryScanner
MemoryScanner is a VB.NET module for scanning the memory of another process for a specific pattern of bytes. This module is useful for debugging and reverse engineering purposes, as well as for testing the security of software applications.


## Features
- Supports scanning for patterns with wildcards.
- Can scan the memory of any running process.
- Returns a list of addresses where the pattern was found.


## Usage
1. Import the MemoryScanner module
```vb.net
Imports MemoryScanner
```

2. Use the ScanMemory function to scan the memory of a target process:
```vb.net
Dim targetProcessName As String = "notepad.exe"
Dim pattern As String = "68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 33 C0"
Dim results As List(Of Integer) = MemoryScanner.ScanMemory(targetProcessName, pattern)
```
The targetProcessName parameter is the name of the process to scan, and the pattern parameter is the pattern of bytes to search for, with ?? representing wildcards that match any byte.

3. The function returns a list of addresses where the pattern was found. Here's an example output:
```vb.net
Target process: notepad.exe
Pattern: 68 ?? ?? ?? ?? E8 ?? ?? ?? ?? 33 C0
Found 1 match(es) at addresses:
0x14019A533
```


## How It Works
The ScanMemory function opens a handle to the target process using the OpenProcess function from the kernel32.dll. Then, it loops through the memory of the process using the VirtualQueryEx and ReadProcessMemory functions to scan each block of memory.

The FindPattern function reads the process memory using ReadProcessMemory and searches for the pattern using a nested loop. The ParsePattern function converts the pattern string to a byte array, replacing wildcards with 0 values.


## Note
This module is intended for educational and research purposes only. Any misuse or unauthorized use is not encouraged.


## Contributing
If you find any issues with this module or have suggestions for improvements, feel free to open an issue or submit a pull request.


## License
This module is licensed under the MIT License.
