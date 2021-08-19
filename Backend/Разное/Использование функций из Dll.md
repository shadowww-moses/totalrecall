Допустим, есть библиотека `MyPerfect.dll`, в которой есть функция `Start` и мы хотим вызвать ее.

Для этого нужно загрузить dll в память. Это можно сделать через атрибут `DllImport`. Но чтобы заставить систему искать dll в произвольном месте, в котором нам нужно, а не в обычных местах вроде `ProgramFiles`, нужно вызвать метод `SetDllDirectoryA`, который находится в стандартной виндовой библиотеке `kernel32.dll`.

Вот ее-то как раз можно без всяких проблем загрузить через `ImportDll`, а дальше все просто:

```c#
public class Utility
{
	[DllImport("MyPerfect.dll", CallingConvention = CallingConvention.Cdecl)]
	private static extern int Start(string path);

	[DllImport("kernel32.dll")]
	private static extern int SetDllDirectoryA(string path);

	public void StartUtility()
	{
		var path = @"C:\foo\bar";

		SetDllDirectoryA(path);

		Start(somePathNoMatter);
}
```

