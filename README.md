# ILSpy Mobile

Versão mínima de um descompilador .NET para Android usando o motor oficial **ICSharpCode.Decompiler** (o mesmo do ILSpy).

## O que faz

- Permite selecionar um ficheiro `.dll` ou `.exe`
- Descompila o assembly completo para C# legível
- Mostra o resultado numa área de texto

## Como gerar o APK automaticamente

1. Crie um repositório no GitHub
2. Envie todo o conteúdo desta pasta para a branch `main`
3. Vá à aba **Actions** do repositório
4. O workflow `Compilar ILSpy Mobile APK` irá executar automaticamente
5. Quando terminar (ficará verde), descarregue o artefacto **ilspy-mobile-apk**

Também pode executar o workflow manualmente (botão "Run workflow").

## Requisitos para build local (opcional)

- .NET 8 SDK
- Workload Android: `dotnet workload install android`
- Android SDK / emulador (para testar)

```bash
dotnet restore
dotnet build -f net8.0-android
dotnet publish -c Release -f net8.0-android /p:AndroidPackageFormat=apk
```

## Limitações desta versão mínima

- Descompila o módulo inteiro (não tem navegação por tipos/métodos como o ILSpy desktop)
- Interface muito simples
- Sem suporte a PDB / símbolos de debug avançados
- Não resolve assemblies de referência externos (apenas o que está dentro do ficheiro escolhido)

## Motor utilizado

```xml
<PackageReference Include="ICSharpCode.Decompiler" Version="11.1.0.9782" />
```

Este é o mesmo motor open-source usado pelo ILSpy oficial.
