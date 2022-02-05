using System.Xml;

namespace IS.Reading.Parsing;

/// <remarks>
/// <code>
/// storybasic 1.0
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
///     b 
///       icon heart
///       text Vamos para casa
///     end
///     if (dinheiro > 100)
///       c 
///         icon heart
///         text Que tal a gente ir comer alguma coisa?
///       end
///     else
///       c 
///         disabled
///         text (Você não tem dinheiro suficiente)
///       end
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
public class StbDocumentReader : IDocumentReader
{
    private readonly DocumentLineReader reader;

    private Memory<char>? currentLine;

    public StbDocumentReader(TextReader textReader)  
        => this.reader = new DocumentLineReader(textReader);

    public int CurrentLineIndex => reader.CurrentLineIndex;

    public void Dispose() => reader.Dispose();

    public async Task<bool> ReadAsync()
    {
        ElementName = string.Empty;

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
            ElementName = span.ToString();
            Argument = string.Empty;
        }
        else
        {
            ElementName = currentLine[..n].ToString();
            Argument = currentLine[(n + 1)..].ToString();
        }
    }


    public IDocumentReader ReadSubtree()
        => new SubReader(reader, ElementName, Argument);

    public bool AtEnd { get; private set; }

    public string ElementName { get; private set; } = string.Empty;

    public string Argument { get; private set; } = string.Empty;

    private class SubReader : IDocumentReader
    {
        private readonly DocumentLineReader reader;

        private Memory<char>? currentLine;

        public SubReader(DocumentLineReader reader, string elementName, string argument)
        { 
            this.reader = reader;
            this.ElementName = elementName;
            this.Argument = argument;
        }

        public int CurrentLineIndex => reader.CurrentLineIndex;

        public void Dispose()
        {
        }

        public async Task<bool> ReadAsync()
        {
            ElementName = string.Empty;
            Argument = string.Empty;

            currentLine = await reader.ReadLineAsync();
            if (currentLine is null)
            {
                AtEnd = true;
                return false;
            }

            return ReadLine(currentLine.Value);
        }

        private static bool IsEnd(ReadOnlySpan<char> span)
        {
            if (span.Length != 3)
                return false;

            if (span[0] != 'E' && span[0] != 'e')
                return false;

            if (span[1] != 'n' && span[1] != 'n')
                return false;

            if (span[2] != 'd' && span[2] != 'd')
                return false;

            return true;
        }

        private bool ReadLine(Memory<char> currentLine)
        {
            var span = currentLine.Span;

            if (IsEnd(span))
                return false;

            var n = span.IndexOf(' ');
            if (n == -1)
            {
                ElementName = span.ToString();
                Argument = string.Empty;
            }
            else
            {
                ElementName = currentLine[..n].ToString();
                Argument = currentLine[(n + 1)..].ToString();
            }
            return true;
        }

        public IDocumentReader ReadSubtree()
            => new SubReader(reader, ElementName, Argument);

        public bool AtEnd { get; private set; }

        public string ElementName { get; private set; } = string.Empty;

        public string Argument { get; private set; } = string.Empty;
    }
}

