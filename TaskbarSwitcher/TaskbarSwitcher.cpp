#include "stdafx.h"
#define APPLICATION_INSTANCE_MUTEX_NAME _TEXT("{5420dce9-dbfc-43d7-b882-2a26b30b034b}")

APPBARDATA msgData;

WINDOWPLACEMENT placement;

HWND maxWindow;

POINT cursor;

bool tryShowBar;

bool IsCursorOverTaskbar();

BOOL CALLBACK EnumWindowsProc(HWND hwnd, LPARAM lParam)
{
    if (IsWindowVisible(hwnd) == FALSE)
        return TRUE;
    GetWindowPlacement(hwnd, &placement);
    if (placement.showCmd != SW_MAXIMIZE)
        return TRUE;
    maxWindow = hwnd;
    return FALSE;
}


int main()
{
    //https://stackoverflow.com/questions/171213/how-to-block-running-two-instances-of-the-same-program
    HANDLE hMutexOneInstance(CreateMutex(NULL, TRUE, APPLICATION_INSTANCE_MUTEX_NAME));
    if (hMutexOneInstance == NULL || GetLastError() == ERROR_ALREADY_EXISTS) {
        if (hMutexOneInstance) {
            ReleaseMutex(hMutexOneInstance);
            CloseHandle(hMutexOneInstance);
        }
        return 0;
    }
    tryShowBar = true;
    msgData.cbSize = sizeof(APPBARDATA);
    placement.length = sizeof(WINDOWPLACEMENT);
    SHAppBarMessage(ABM_GETTASKBARPOS, &msgData);
    while (true) {
        while (IsCursorOverTaskbar())
            Sleep(250);
        EnumWindows(EnumWindowsProc, NULL);
        if (maxWindow == NULL) {
            if (tryShowBar == false) {
                Sleep(375);
                continue;
            }
            tryShowBar = false;
            msgData.lParam = ABS_ALWAYSONTOP;
            SHAppBarMessage(ABM_SETSTATE, &msgData);
            Sleep(500);
            continue;
        }
        SHAppBarMessage(ABM_GETTASKBARPOS, &msgData);
        msgData.lParam = ABS_AUTOHIDE;
        SHAppBarMessage(ABM_SETSTATE, &msgData);
        do {
            Sleep(500);
            if (IsWindowVisible(maxWindow) == FALSE)
                break;
            GetWindowPlacement(maxWindow, &placement);
        } while (placement.showCmd == SW_MAXIMIZE);
        tryShowBar = true;
        maxWindow = NULL;
    }
    return 0;
}

inline bool IsCursorOverTaskbar()
{
    GetCursorPos(&cursor);
    switch (msgData.uEdge)
    {
    case ABE_BOTTOM:
        if (cursor.y >= msgData.rc.top)
            return true;
        break;
    case ABE_LEFT:
        if (cursor.x <= msgData.rc.right)
            return true;
        break;
    case ABE_TOP:
        if (cursor.y <= msgData.rc.bottom)
            return true;
        break;
    default:
        if (cursor.x >= msgData.rc.left)
            return true;
        break;
    }
    return false;
}
