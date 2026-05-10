# Spec: Storybasic — Comando `background`

O comando `background` controla a imagem de fundo do leitor. Pode ser usado como shorthand com argumento direto ou como bloco com sub-comandos.

## Sintaxe

```
background [imageName [animation [flashColor]]]
    [sub-comandos]
```

## Formas de Uso

### Shorthand com imagem

```
background floresta
```
Exibe `floresta.jpg` na posição `Left`, sem animação.

```
background floresta fadein
```
Exibe a imagem com animação fade-in de 500 ms.

```
background floresta flash
background floresta flash white
background floresta flash #ff0000
```
Exibe a imagem com efeito de flash. A cor é opcional (padrão: `white`).

### Bloco com sub-comandos

```
background
right floresta
scroll
pause
color black fadein
pause 500
color white
pause
```

Os sub-comandos são executados em sequência dentro do bloco. Um `DismissNode` (`BackgroundState.Empty`) é registrado automaticamente para restaurar o estado ao sair do bloco.

### Bloco combinado (argumento + sub-comandos)

```
background floresta
color black
pause
```

O argumento cria um `BackgroundNode` inicial e os sub-comandos são adicionados a seguir no mesmo bloco.

## Animações de Transição

Disponíveis tanto no argumento de `background` quanto no de `color`.

| Keyword | Efeito | Duração |
|---|---|---|
| *(ausente)* | Substituição imediata | — |
| `fadein` | `opacity 0→1`, easing `ease` | 500 ms |
| `zoom` | `background-size 115%→100%`, easing `ease` | 500 ms |
| `dissolve` | `opacity 0→1`, easing `linear` | 500 ms |
| `flash` | Overlay colorido que aparece e desfade | 500 ms |

Keywords são **case-insensitive** (`fadein`, `FadeIn` e `FADEIN` são equivalentes).

### Flash com cor

```
background floresta flash white
background floresta flash #ff0000
color black flash red
```

Cores válidas: nomes CSS (`white`, `black`, `red`, `blue`, `gray`, etc.) e hexadecimal de 6 dígitos (`#rrggbb`). Lista completa de nomes válidos na spec de `IColorArgumentParser`.

Se a cor for omitida, o flash usa `white`.

A cor do flash é inválida para animações que não sejam `flash`:

```
background floresta fadein white  ← ERRO
```

## Sub-comandos

### `right imageName`

```
right floresta
```

Exibe `floresta.jpg` na posição `Right`. Dispara `IBackgroundChangeEvent` com `Position = Right`.

### `left imageName`

```
left floresta
```

Exibe `floresta.jpg` na posição `Left`. Dispara `IBackgroundChangeEvent` com `Position = Left`.

### `color colorName [animation [flashColor]]`

```
color black
color black fadein
color white zoom
color black flash white
color black flash #ff0000
```

Define um fundo de cor sólida. Suporta as mesmas animações que `background`. Dispara `IBackgroundChangeEvent` com `Type = Color` e `Position = Undefined`.

Cores válidas: nomes CSS e hexadecimal de 6 dígitos.

### `scroll`

```
scroll
```

Alterna a posição horizontal da imagem de fundo entre `Left` e `Right`. Dispara `IBackgroundScrollEvent` com o novo `Position`. A animação de scroll dura 1000 ms no componente `Background`.

O scroll deve ser adicionado **após** o `right` ou `left` para que a imagem já esteja posicionada antes do scroll ocorrer. Como os handlers são awaited sequencialmente, o scroll só inicia após qualquer animação de entrada anterior ter concluído.

### `pause` / `pause {ms}`

Ponto de pausa dentro do bloco. Interrompe a navegação e aguarda o próximo `MoveNextAsync()`.

```
color black
pause
color white
pause
```

Com duração fixa (em milissegundos):

```
color black
pause 500
color white
pause
```

## Eventos Disparados

| Evento | Disparado por | Payload |
|---|---|---|
| `IBackgroundChangeEvent` | `background`, `right`, `left`, `color` | `State` (tipo/nome/posição), `Animation`, `FlashColor` |
| `IBackgroundScrollEvent` | `scroll` | `Position` (destino) |

### Comportamento no backward

Na navegação backward, `IBackgroundChangeEvent` é sempre disparado com `Animation = None` e `FlashColor = null`. O estado restaurado é o que existia imediatamente antes do nó que está sendo desfeito.

O `scroll` não tem backward próprio — ao retroceder, o `ScrollNode` é executado novamente no sentido inverso (toggle da posição).

## Erros de Parse

| Situação | Mensagem |
|---|---|
| `background` sem argumento e sem sub-comandos | `"O comando 'background' espera um parâmetro ou um ou mais comandos adicionais."` |
| Mais de 3 tokens no argumento de `background` | `"Muitos argumentos para o comando 'background'."` |
| Mais de 3 tokens no argumento de `color` | `"Muitos argumentos para o comando 'color'."` |
| Keyword de animação inválida | `"O texto '{keyword}' não é uma animação válida. As animações disponíveis são: fadein, zoom, dissolve, flash."` |
| Cor de flash em animação não-flash | `"A cor do flash só pode ser especificada para a animação 'flash'."` |
| Nome de imagem inválido | Mensagem do `IImageArgumentParser` |
| Cor inválida | `"O texto '{value}' não representa uma cor válida."` |

## Estrutura Interna de Parsing

```
BackgroundNodeParser (IBackgroundNodeParser)
  ├── IBackgroundArgumentParser
  │     ├── IImageArgumentParser — valida nome da imagem
  │     └── IColorArgumentParser — valida cor do flash
  └── ElementParser + sub-parsers:
        ├── IBackgroundLeftNodeParser
        ├── IBackgroundRightNodeParser
        ├── IBackgroundColorNodeParser
        │     └── IBackgroundColorArgumentParser
        │           └── IColorArgumentParser — valida cor e cor do flash
        ├── IBackgroundScrollNodeParser
        └── IPauseNodeParser
```

**Nodes gerados:**

| Node | Criado por | Efeito |
|---|---|---|
| `BackgroundNode(state, animation, flashColor)` | `background`, `right`, `left`, `color` | Muda imagem ou cor de fundo |
| `BackgroundScrollNode` | `scroll` | Dispara evento de scroll |
| `BlockNode` | `background` (bloco) | Agrupa os nós do bloco |
| `BackgroundNode(Empty)` *(DismissNode)* | `background` (bloco) | Restaura estado vazio ao sair do bloco |

## Modelo de Estado

```csharp
BackgroundState(string Name, BackgroundType Type, BackgroundPosition Position)
```

| `BackgroundType` | Descrição |
|---|---|
| `Undefined` | Fundo não definido (estado inicial / vazio) |
| `Image` | Imagem `.jpg` |
| `Color` | Cor CSS sólida |

| `BackgroundPosition` | Descrição |
|---|---|
| `Undefined` | Sem posição (usado para `Color`) |
| `Left` | Imagem alinhada à esquerda |
| `Right` | Imagem alinhada à direita |
