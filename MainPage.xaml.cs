using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;

namespace ILSpyMobile;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnPickClicked(object sender, EventArgs e)
    {
        try
        {
            StatusLabel.Text = "A selecionar ficheiro...";
            BtnPick.IsEnabled = false;

            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.Android, new[] { "application/octet-stream", "application/x-msdownload", "*/*" } },
                { DevicePlatform.WinUI, new[] { ".dll", ".exe" } },
                { DevicePlatform.iOS, new[] { "public.data" } },
                { DevicePlatform.MacCatalyst, new[] { "dll", "exe" } }
            });

            var options = new PickOptions
            {
                PickerTitle = "Selecione um assembly .NET (.dll ou .exe)",
                FileTypes = customFileType
            };

            var result = await FilePicker.Default.PickAsync(options);

            if (result == null)
            {
                StatusLabel.Text = "Nenhum ficheiro selecionado.";
                return;
            }

            StatusLabel.Text = $"A descompilar: {result.FileName}...";
            CodeEditor.Text = "A processar...";

            // Lê o ficheiro para memória (importante no Android por causa de content:// URIs)
            using var stream = await result.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Descompila
            string decompiledCode = await Task.Run(() => DecompileAssembly(memoryStream, result.FileName));

            CodeEditor.Text = decompiledCode;
            StatusLabel.Text = $"Descompilado com sucesso: {result.FileName}";
        }
        catch (Exception ex)
        {
            CodeEditor.Text = $"Erro ao descompilar:\n\n{ex.Message}\n\n{ex.StackTrace}";
            StatusLabel.Text = "Erro durante a descompilação.";
        }
        finally
        {
            BtnPick.IsEnabled = true;
        }
    }

    private static string DecompileAssembly(Stream assemblyStream, string fileName)
    {
        // Cria um PEFile a partir do stream
        using var peFile = new PEFile(fileName, assemblyStream);

        var settings = new DecompilerSettings
        {
            ThrowOnAssemblyResolveErrors = false,
            UseDebugSymbols = true
        };

        var resolver = new UniversalAssemblyResolver(fileName, false, peFile.DetectTargetFrameworkId());
        var decompiler = new CSharpDecompiler(peFile, resolver, settings);

        // Descompila o módulo inteiro para C#
        return decompiler.DecompileWholeModuleAsString();
    }
}
