# Preferências de código

- Sempre usar file-based namespaces
- Usar primary constructors sempre que possível
- Quando há uma única implementação, a classe e a interface devem ficar no mesmo arquivo .cs
- O rootnamespace deve ser sempre vazio no csproj
- O namespace sempre deve ser equivalente à estrutura de pastas (namespace A.B.C deve ficar dentro da pasta A/B/C)
- Os warnings devem ser tratados como erros
- Classes com métodos de extensão para injeção de dependência devem ficar no namespace Microsoft.Extensions.DependendyInjection, e seus respectivos arquivos .cs devem ficar numa subpasta com o mesmo nome