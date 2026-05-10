# Spec: Biblioteca

A Biblioteca é a UI inicial da aplicação, responsável por listar os livros disponíveis e permitir ao usuário iniciar ou retomar uma leitura.

## Páginas e Componentes

### `Library` — página raiz (`/`)

Página inicial da aplicação. Carrega em paralelo a lista de categorias e de livros via `IAssetManager`, agrupa os livros por categoria e renderiza uma grade de `BookCard` por categoria.

### `BookDetails` — `/book/{id}`

Exibe os detalhes de um livro selecionado:

- Imagem de capa como background de página
- Título, sinopse e tags
- Número de capítulos lançados e indicador de publicação completa

Controles disponíveis:

| Controle | Ação |
|---|---|
| Fechar | Volta para `Library` |
| Ativar/desativar música | Estado local da página |
| Iniciar leitura | Navega para `/book/{id}/read/1` |

### `BookCard` — componente

Cartão exibido na grade da biblioteca com imagem thumbnail e título. Emite `OnSelected` ao ser clicado, o que dispara a navegação para `BookDetails`.

## Modelo de Dados

Os arquivos ficam em `wwwroot/assets/books/`.

### `categories.json`

Lista ordenada de categorias exibidas na biblioteca.

```json
[
  { "id": "short_story", "title": "Contos" },
  { "id": "test",        "title": "Teste"  }
]
```

### `books.json`

Lista de todos os livros disponíveis na aplicação.

```json
[
  {
    "id": "enchanted_valley",
    "title": "O Vale Encantado",
    "categories": ["short_story"]
  }
]
```

Um livro pode pertencer a múltiplas categorias.

### `{bookId}/details.json`

Detalhes estáticos de cada livro, carregados pela página `BookDetails`.

```json
{
  "title": "O Vale Encantado",
  "synopsis": "Sinopse do livro...",
  "tags": ["Romance", "Fantasia"],
  "releasedChapters": 1,
  "fullReleased": false
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `title` | `string` | Título do livro |
| `synopsis` | `string` | Sinopse exibida em `BookDetails` |
| `tags` | `string[]` | Etiquetas (gênero, universo, etc.) |
| `releasedChapters` | `int` | Quantidade de capítulos publicados |
| `fullReleased` | `bool` | `true` quando a publicação está completa |

## Assets Visuais por Livro

| Arquivo | Uso |
|---|---|
| `thumbnail.png` | Imagem exibida no `BookCard` na grade da biblioteca |
| `cover.png` | Background da página `BookDetails` |

## Fluxo de Navegação

```
/ (Library)
  → /book/{id} (BookDetails)
      → /book/{id}/read/{chapter} (BookReader)
          → /book/{id} (ao terminar o capítulo ou clicar em Home)
```

## Dependências

| Interface | Responsabilidade |
|---|---|
| `IAssetManager` | Carrega categorias, livros, detalhes e URLs de assets |
| `NavigationManager` | Roteamento entre páginas |
