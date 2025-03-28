using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

class WindowFocusMonitor
{
    private const uint EVENT_SYSTEM_FOREGROUND = 0x0003;
    private const uint WINEVENT_OUTOFCONTEXT = 0;
    private DateTime _lastTriggerTime = DateTime.MinValue;
    private readonly TimeSpan _debounceInterval = TimeSpan.FromMilliseconds(500);

    // Delegate para o callback do evento
    private delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);
    private WinEventDelegate _winEventDelegate;

    [DllImport("user32.dll")]
    private static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc,
        WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(IntPtr hWinEventHook);

    private IntPtr _hook;

    public Action OnFocusChanged;
    public void StartMonitoring()
    {
        _winEventDelegate = new WinEventDelegate(WinEventProc);
        _hook = SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero,
            _winEventDelegate, 0, 0, WINEVENT_OUTOFCONTEXT);
    }

    private void WinEventProc(IntPtr hWinEventHook, uint eventType,
        IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        var now = DateTime.Now;
        if ((now - _lastTriggerTime) < _debounceInterval)
        {
            return; // Ignora se foi disparado muito recentemente
        }

        _lastTriggerTime = now;
        // Aqui você pode disparar sua lógica de alteração de volume
        Debug.WriteLine("Foreground window changed");
        OnFocusChanged?.Invoke();
        // Exemplo: Chamar um método para ajustar volumes
    }

    public void StopMonitoring()
    {
        UnhookWinEvent(_hook);
    }
}
