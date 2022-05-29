using Microsoft.Win32;
using ToDataUri;

var filePath = args.Any() ? args[0] : string.Empty;

const string KEY_NAME = "ToDataUri";
var supportedTypes = new Dictionary<string, string>()
{
    [".jpg"] = "image/jpeg",
    [".png"] = "image/png",
    [".gif"] = "image/gif"
};


var progPath = Environment.ProcessPath;
var cmdRegValue = @$"""{progPath}"" ""%1""";
var shellRegPath = @"Software\Classes\*\shell";
var menuReg = Registry.CurrentUser.OpenSubKey(@$"{shellRegPath}\{KEY_NAME}", true);
var cmdReg = Registry.CurrentUser.OpenSubKey(@$"{shellRegPath}\{KEY_NAME}\command", true);
using (var w = new WinFormUtil())
{
    if (filePath == "--unregister")
    {
        if (menuReg != null)
        {
            Registry.CurrentUser.OpenSubKey(@$"Software\Classes\*\shell", true)!.DeleteSubKeyTree(KEY_NAME);
            w.ShowInfo("Unregistered");
        }
        return;
    }
    else if (menuReg == null || cmdReg?.GetValue(string.Empty)?.ToString() != cmdRegValue)
    {
        if (menuReg == null)
        {
            menuReg = Registry.CurrentUser.CreateSubKey(@$"{shellRegPath}\{KEY_NAME}");
            menuReg.SetValue(string.Empty, "Convert To Data Uri");
        }
        if (cmdReg == null)
            cmdReg = menuReg.CreateSubKey("command");
        cmdReg.SetValue(string.Empty, cmdRegValue);
        w.ShowInfo("Registered");
    }
    if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
    {
        try
        {
            var ext = Path.GetExtension(filePath);
            if (!supportedTypes.ContainsKey(ext))
            {
                w.ShowInfo($"Unsupported file type - {ext}");
                return;
            }
            var fileContent = System.IO.File.ReadAllBytes(filePath);
            var base64 = Convert.ToBase64String(fileContent);
            var dataUri = $"data:{supportedTypes[ext]};base64,{base64}";

            //Call Clipboard API with STA thread
            var thread = new Thread(() => { Clipboard.SetText(dataUri); })
            {
                IsBackground = false,
                Priority = ThreadPriority.Normal
            };
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();
            w.ShowInfo("Data Uri copied");
        }
        catch (Exception ex)
        {
            w.Alert(ex.ToString());
        }
    }
}