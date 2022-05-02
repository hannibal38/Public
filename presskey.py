#pip install pywin32
import pyautogui, win32gui,win32con,win32api

#Input caption of console.
handle=win32gui.FindWindow(None,"제목 없음 - Windows 메모장")
#win32gui.PostMessage(handle,win32con.WM_CLOSE,0,0)
print(handle)
edit = win32gui.GetDlgItem(handle, 0xF)


size=pyautogui.size()

win_x1,win_y1,win_x2,win_y2=win32gui.GetWindowRect(handle)
print("size: " + str(size))
print(str(win_x1) + "," + str(win_y1) + "," + str(win_x2) + "," + str(win_y2))
print("MousePosition Before: " + str(pyautogui.position()))
pyautogui.moveTo(win_x1,win_y1)
pyautogui.moveRel(100,100)
print("MousePosition After: " + str(pyautogui.position()))


# Press quit
#win32api.SendMessage(edit, win32con.WM_CHAR, ord('q'), 0)
#win32api.Sleep(100)
#win32api.SendMessage(edit, win32con.WM_CHAR, ord('u'), 0)
#win32api.Sleep(100)
#win32api.SendMessage(edit, win32con.WM_CHAR, ord('i'), 0)
#win32api.Sleep(100)
#win32api.SendMessage(edit, win32con.WM_CHAR, ord('t'), 0)
#win32api.Sleep(100)
#
# Press Enter
#win32api.PostMessage(edit, win32con.WM_KEYDOWN,win32con.VK_RETURN, 0)
#win32api.Sleep(100)
#win32api.PostMessage(edit, win32con.WM_KEYUP,win32con.VK_RETURN,0)
#win32api.Sl eep(100)