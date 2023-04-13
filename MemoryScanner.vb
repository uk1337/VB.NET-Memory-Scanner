Imports System.Runtime.InteropServices
Imports System.Text

Module MemoryScanner

    Private Const PROCESS_VM_READ As Integer = &H10

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function OpenProcess(ByVal dwDesiredAccess As Integer, ByVal bInheritHandle As Boolean, ByVal dwProcessId As Integer) As IntPtr
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function ReadProcessMemory(ByVal hProcess As IntPtr, ByVal lpBaseAddress As IntPtr, <Out()> ByVal lpBuffer() As Byte, ByVal dwSize As Integer, ByRef lpNumberOfBytesRead As Integer) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Function CloseHandle(ByVal hObject As IntPtr) As Boolean
    End Function

    Public Function ScanMemory(ByVal processName As String, ByVal pattern As String) As List(Of IntPtr)
        Dim processId As Integer = Process.GetProcessesByName(processName)(0).Id
        Dim processHandle As IntPtr = OpenProcess(PROCESS_VM_READ, False, processId)

        Dim bytesRead As Integer
        Dim buffer(1024) As Byte
        Dim address As IntPtr = IntPtr.Zero
        Dim addresses As New List(Of IntPtr)

        While address <> IntPtr.MaxValue
            address = FindPattern(processHandle, address, buffer, pattern, bytesRead)
            If address <> IntPtr.Zero Then
                addresses.Add(address)
            End If
        End While

        CloseHandle(processHandle)

        Return addresses
    End Function

    Private Function FindPattern(ByVal processHandle As IntPtr, ByVal startAddress As IntPtr, ByRef buffer() As Byte, ByVal pattern As String, ByRef bytesRead As Integer) As IntPtr
        Dim patternBytes As Byte() = ParsePattern(pattern)
        Dim endAddress As IntPtr = IntPtr.Zero

        While startAddress < endAddress
            If ReadProcessMemory(processHandle, startAddress, buffer, buffer.Length, bytesRead) Then
                For i As Integer = 0 To bytesRead - patternBytes.Length
                    Dim found As Boolean = True

                    For j As Integer = 0 To patternBytes.Length - 1
                        If patternBytes(j) = &HFF OrElse patternBytes(j) = buffer(i + j) Then
                            Continue For
                        Else
                            found = False
                            Exit For
                        End If
                    Next

                    If found Then
                        Return startAddress + i
                    End If
                Next

                startAddress += buffer.Length
            Else
                Exit While
            End If
        End While

        Return IntPtr.Zero
    End Function

    Private Function ParsePattern(ByVal pattern As String) As Byte()
        Dim patternBytes As New List(Of Byte)

        For i As Integer = 0 To pattern.Length - 1 Step 2
            If pattern(i) = "?"c OrElse pattern(i + 1) = "?"c Then
                patternBytes.Add(&HFF)
            Else
                patternBytes.Add(Convert.ToByte(pattern.Substring(i, 2), 16))
            End If
        Next

        Return patternBytes.ToArray()
    End Function

End Module
