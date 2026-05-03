using System.Xml;

namespace IS.Reading.Parsing;

/// <remarks>
/// <code>
/// ' storybasic 1.0
///
/// set john = 'John'
/// set jane = 'Jane'
/// set tutorial = 'Tutorial'
/// set narration = 'Narrador'
/// 
/// background
/// left forest
/// scroll
/// 
/// tutorial
/// title Tutorial
/// - Esta é uma obra de ficção.
/// - Qualquer semelhança entre locais, acontecimentos ou pessoas da vida real terá sido mera coincidência.
/// 
/// narration
/// title Título do Livro
/// - Capítulo ??: A Introdução
/// 
/// narration
/// title Narrador
/// - Muito tempo atrás, numa galáxia muito, muito distante...
/// 
/// while a == null
///   set a = 1
///   
///   @ john
///   # happy
///   - Olá!
///   - Como estão as coisas?
///   
///   @ jane
///   - Sem comentários.
///   
///   @ john
///   # happy
///   - Ruim assim?
///   # surprised
///   - O que houve?
///   
///   @ jane
///   - ...
///   
///   @ john
///   # surprised
///   thought
///   - Nunca vi ela assim antes. Deve ser sério.
///   
///   speech
///   # normal
///   - Sabe que pode falar comigo, não é?
///   - Sempre estarei aqui por você.
/// 
///   @ jane
///   # surprised
///   - Deixa pra lá, {john}...
///   
///   @ john
///   title {john}
///   if (empatia > 10)
///     # normal
///   else
///     # angry
///   end
///   thought
///   - Eu vou dizer...
/// 
///   ? selection
///     timeout 8
///     default d
///     
///     if (empatia > 10)
///       a Tudo bem, eu entendo, deixa pra lá.
///     else
///       a Se é isso que você quer...
///     end
///     b Vamos para casa
///     icon heart
///     if (dinheiro > 100)
///       c Que tal a gente ir comer alguma coisa?
///       icon heart
///     else
///       c (Você não tem dinheiro suficiente)
///       disabled
///     end
///     d ...
///   end
///   
///   if selection == 'a'
///     thought
///     - Será que isso está correto?
///     # sad
///     - Não sei se deveria prosseguir com isso...
///   end
///   
///   if selection == 'b'
///     # angry
///     thought 
///     - Isso não está indo conforme o previsto.
///   end
/// end
/// </code>
/// </remarks>
public sealed class DocumentReader : IDocumentReader
{
    private readonly IDocumentLineReader reader;

    private Memory<char>? currentLine;

    public DocumentReader(IDocumentLineReader reader)  
        => this.reader = reader;

    public int CurrentLineIndex => reader.CurrentLineIndex;

    public void Dispose() => reader.Dispose();

    public async Task<bool> ReadAsync()
    {
        Command = string.Empty;

        currentLine = await reader.ReadLineAsync();
        if (currentLine is null)
        {
            AtEnd = true;
            return false;
        }

        ReadLine(currentLine.Value);

        return true;
    }

    private void ReadLine(Memory<char> currentLine)
    {
        var span = currentLine.Span;

        var n = span.IndexOf(' ');
        if (n == -1)
        {
            Command = span.ToString();
            Argument = string.Empty;
        }
        else
        {
            Command = currentLine[..n].ToString();
            Argument = currentLine[(n + 1)..].ToString();
        }
    }

    public bool AtEnd { get; private set; }

    public string Command { get; private set; } = string.Empty;

    public string Argument { get; private set; } = string.Empty;
}

