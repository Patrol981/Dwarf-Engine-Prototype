using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Dwarf.Engine.Debugging;
public class Debugger {
  public const bool UseDebbuger = false;

  public static void OnDebugMessage(
    DebugSource source,     // Source of the debugging message.
    DebugType type,         // Type of the debugging message.
    int id,                 // ID associated with the message.
    DebugSeverity severity, // Severity of the message.
    int length,             // Length of the string in pMessage.
    IntPtr pMessage,        // Pointer to message string.
    IntPtr pUserParam
   ) {      // The pointer you gave to OpenGL, explained later.

      // In order to access the string pointed to by pMessage, you can use Marshal
      // class to copy its contents to a C# string without unsafe code. You can
      // also use the new function Marshal.PtrToStringUTF8 since .NET Core 1.1.
      string message = Marshal.PtrToStringAnsi(pMessage, length);

      // The rest of the function is up to you to implement, however a debug output
      // is always useful.
      Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);

      // Potentially, you may want to throw from the function for certain severity
      // messages.
      if (type == DebugType.DebugTypeError) {
        throw new Exception(message);
      }
  }

  public static DebugProc DebugMessageDelegate = OnDebugMessage;
}