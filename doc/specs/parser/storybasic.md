# Spec: Storybasic — Visão Geral

Storybasic é a linguagem de script usada nos arquivos `.stbasic` para definir o storyboard de cada capítulo de um livro.

## Formato do Arquivo

- Extensão: `.stbasic`
- Encoding: UTF-8
- Primeira linha não em branco obrigatória: `' Storybasic 1.0`

Exemplo mínimo:

```
' Storybasic 1.0
background floresta
pause
```

## Sintaxe Geral

Cada linha não em branco contém um comando seguido de um argumento opcional:

```
[comando] [argumento]
```

- Linhas em branco são ignoradas
- Linhas começando com `'` são comentários e também são ignoradas
- Argumentos com espaços internos são separados de acordo com as regras de cada comando

Alguns comandos aceitam **sub-comandos** em linhas indentadas que formam um bloco:

```
background
right floresta
color black
scroll
pause
```

## Modelo de Navegação

O storyboard é uma árvore de nós construída durante o parse. A navegação percorre essa árvore nó a nó.

### Forward

`storyboard.MoveNextAsync()` avança a narrativa, executando nós em sequência. Nós com `ChildBlock` fazem a navegação descer para o bloco filho. A navegação para em nós do tipo `IPauseNode` e aguarda a próxima chamada.

### Backward

`storyboard.MovePreviousAsync()` percorre a pilha de nós visitados (`BackwardStack`) em ordem inversa. Cada nó tem seu `EnterAsync(context, savedState)` chamado com o estado salvo durante o avanço, permitindo a restauração do estado anterior sem reproduzir animações.

### DismissNodes

Blocos podem registrar um `DismissNode` via `IParsingContext.RegisterDismissNode(node)`. Ao sair do bloco (fim de escopo), o motor executa o DismissNode para restaurar o estado padrão. Exemplo: o bloco `background` registra `BackgroundNode(BackgroundState.Empty)` como DismissNode.

## Comandos Implementados

| Comando | Argumento | Descrição |
|---|---|---|
| `background` | `[imageName [animation [flashColor]]]` | Imagem de fundo ou bloco com sub-comandos |
| `color` | `colorName [animation [flashColor]]` | Cor sólida de fundo (sub-comando de `background`) |
| `right` | `imageName` | Imagem de fundo alinhada à direita (sub-comando de `background`) |
| `left` | `imageName` | Imagem de fundo alinhada à esquerda (sub-comando de `background`) |
| `scroll` | — | Scroll horizontal do fundo (sub-comando de `background`) |
| `pause` | — | Pausa aguardando interação do usuário |
| `pause` | `{ms}` | Pausa temporizada em milissegundos |
| `music` | `trackName` | Controla a faixa de música ativa |
| `person` | — | Configuração e exibição de personagem (bloco) |
| `speech` | `texto` | Balão de fala |
| `thought` | `texto` | Balão de pensamento |
| `narration` | `texto` | Balão de narração |
| `tutorial` | `texto` | Balão de tutorial/instrução |
| `if` | `condição` | Controle de fluxo condicional |
| `while` | `condição` | Repetição condicional |
| `set` | `variável = valor` | Atribuição de variáveis |
| `input` | — | Entrada de texto do usuário (bloco) |

## Arquitetura de Parsing

```
StoryboardParser
  └── RootBlockParser
        └── INodeParser (por comando)
              ├── IDocumentReader     — linha atual e argumento
              ├── IParsingContext     — contexto, erros, registro de DismissNodes
              └── IParentParsingContext — destino dos nós gerados
```

Parsers de argumento encapsulam validação e conversão de tokens individuais. Internamente usam `ReadOnlySpan<char>` com `stackalloc Range[]` para o split, evitando alocações de array:

| Parser | Responsabilidade |
|---|---|
| `IImageArgumentParser` | Valida nomes de imagem |
| `IColorArgumentParser` | Valida cores CSS (nomes e hex) |
| `IBackgroundArgumentParser` | Parse de `imageName [animation [flashColor]]` |
| `IBackgroundColorArgumentParser` | Parse de `colorName [animation [flashColor]]` |
| `IIntegerArgumentParser` | Parse de inteiros (ex: duração do `pause`) |

## Animações Suportadas

Aplicáveis aos comandos `background` e `color`:

| Keyword | Tipo |
|---|---|
| `fadein` | `BackgroundAnimation.FadeIn` |
| `zoom` | `BackgroundAnimation.Zoom` |
| `dissolve` | `BackgroundAnimation.Dissolve` |
| `flash` | `BackgroundAnimation.Flash` |

Keywords são **case-insensitive**.

## Localização dos Arquivos

Os arquivos `.stbasic` ficam em:

```
wwwroot/assets/books/{bookId}/chapters/{chapter}.stbasic
```

Onde `{chapter}` é o número do capítulo (ex: `1.stbasic`, `2.stbasic`).
