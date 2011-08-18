using System;
using System.Collections.Generic;
using System.Text;

namespace Canguro.Utility
{
    class NativeHelperMethods
    {
        internal static void WaitInMainThread(int milliseconds)
        {
            System.Windows.Forms.Application.DoEvents();

            // Para evitar que este ciclo se coma todo el cpu, porque
            // DoEvents sólo pasa el control al MessageLoop de la aplicación
            // para que cheque si hay mensajes y en caso contrario regresa
            // inmediatamente, lo que hace que este ciclo se convierta en un
            // while que consume el 100% del cpu nomás por esperar
            // (Exactamente como el PropertyGrid cuando despliega un DropDown)
            // La llamada a la rutina de abajo duerme al hilo hasta que ocurra
            // cualquier evento, con lo que se arregla el problema
            NativeMethods.MsgWaitForMultipleObjects(0, IntPtr.Zero, true, milliseconds, 0xff);
        }

        internal static bool AppStillIdle
        {
            get
            {
                NativeMethods.Message msg;
                return !NativeMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }
    }
}
