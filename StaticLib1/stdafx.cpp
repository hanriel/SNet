// stdafx.cpp: исходный файл, содержащий только стандартные включаемые модули
// StaticLib1.pch будет использоваться в качестве предкомпилированного заголовка
// stdafx.obj будет содержать предварительно откомпилированные сведения о типе

#include "stdafx.h"


// TODO: Установите ссылки на любые требующиеся дополнительные заголовки в файле STDAFX.H
// , а не в данном файле

//Функция проверки того, что запущен единственный процесс программы
BOOL AreWeAlone(LPSTR szName) {
	HANDLE hMutex = CreateMutex(0, TRUE, szName);

	if (GetLastError() == ERROR_ALREADY_EXISTS) {
		CloseHandle(hMutex);
		return FALSE;
	}

	return TRUE;
}

//Функция проверяет, является ли пользователь администратором;
//Возвращает TRUE, если пользователь - администратор.
BOOL IsCurrentUserAdmin() {
	WCHAR wcsUserName[UNLEN + 1];
	DWORD dwUserNameLen = UNLEN + 1;

	if (GetUserNameW(wcsUserName, &dwUserNameLen) > 0) {
		USER_INFO_1 *userInfo;

		if (NetUserGetInfo(NULL, wcsUserName, 1, (BYTE**)&userInfo) == NERR_Success) {
			BOOL bReturn = (userInfo->usri1_priv == USER_PRIV_ADMIN);

			NetApiBufferFree(userInfo);
			return bReturn;
		}
	}

	return FALSE;
}

//Функция обработки сообщений окна помощи
INT_PTR CALLBACK AboutDialogProc(HWND hwndDlg, UINT uMsg, WPARAM wParam, LPARAM lParam) {
	switch (uMsg) {
	case WM_COMMAND:
		WORD loWord;

		loWord = LOWORD(wParam);
		if (loWord == IDOK || loWord == IDCANCEL) {
			SendMessage(hwndDlg, WM_CLOSE, 0, 0);
		}
		return TRUE;

	case WM_CLOSE:
		EndDialog(hwndDlg, 0);
		return TRUE;
	}
	return FALSE;
}
