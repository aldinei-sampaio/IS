# Contos Interativos (IS)

## Introdução

Contos Interativos (ou Interactive Stories - IS) é uma aplicação para leitura de livros digitais interativos.

É dividido em duas partes principais: biblioteca e leitor.

## Biblioteca

A biblioteca é a primeira UI que é exibida ao usuário ao iniciar a aplicação.

Tem as seguintes funcionalidades:
- Exibe os livros disponíveis na biblioteca do aplicativo (nome e imagem da capa), agrupados por categoria
- Quando o usuário selecionar um livro:
  - Exibe detalhes sobre o mesmo
    - sinopse
    - quantidade de capítulos
    - caso o usuário já tenha iniciado a leitura, em qual capítulo ele parou
    - lista de troféus que o usuário adquiriu durante a leitura do livro
  - Permite que o usuário inicie a leitura do livro selecionado ou retome a mesma, caso ela tenha sido iniciada e interrompida
  - Caso o usuário já tenha iniciado ou concluído a leitura, permitir reiniciar o progresso a partir do início do capítulo atual ou do primeiro capítulo

Ao iniciar a biblioteca, caso a última sessão tenha encerrado no meio da leitura de um livro (ex: o usuário fechou o navegador com a aplicação no modo leitor), o sistema deve automaticamente abrir os detalhes do livro, de forma que ele possa retomar a leitura rapidamente.

Não haverá música ou efeitos sonoros na biblioteca.

## Leitor

O leitor é a UI mais complexa da aplicação, que deve apresentar a história do livro como se fosse uma história em quadrinhos, com balões de narrativa, pensamento e diálogo. Ao apresentar um balão de fala, o leitor deve apresentar tanbém a imagem e o nome do personagem que está falando. A imagem do personagem pode ter diversas variantes, com diferentes expressões faciais (normal, surpreso, triste, alegre e com raiva).

Durante a leitura, existe sempre um personagem que é considerado o protagonista, cujo nome e imagem que devem aparecer do lado esquerdo da tela. Os demais personagens devem ser exibidos do lado direito, gerando uma clara diferenciação que facilita identificar o protagonista. O protagonista pode mudar durante a história, então ele não será sempre o mesmo.

Em certas partes da história o usuário terá a oportunidade de escolher o que o protagonista irá dizer ou fazer. Essas escolhas poderão levar o usuário a ganhar troféus, que representam façanhas que o usuário conseguiu realizar graças às suas escolhas. O leitor deve permitir visualizar a lista de troféus adquiridos no capítulo atual. O usuário não perde os troféus adquiridos caso o progresso do capítulo ou mesmo do livro inteiro seja reiniciado.

De acordo com o que estiver definido no storyboard do livro, o leitor deverá tocar certas faixas de música durante a leitura. O usuário deverá ter opção para ativar ou desativar a música de acordo com sua preferência. O storyboard irá determinar qual faixa deve ser tocada e em qual momento.

Ao terminar um capítulo, a aplicação deve exibir novamente os detalhes do livro. O usuário então poderá usar a opção de continuar a leitura para ler o próimo capítulo.

O leitor possui diversos componentes. O storyboard irá controlar quando cada um deles deve ser exibido, ocultado, ou quando alguma animação deve ser executada.

### Imagem de fundo

Uma imagem que deve ficar por trás de outros componentes (caso tenha algum visível). A imagem original deve ser redimensionada tendo como por base a altura da área visível. Poderá ocorrer de a imagem ficar com largura maior do que a da área visível. Nesses casos será possível que, de acordo com certos comandos do storyboard, a imagem deva ser rolada de um lado para o outro.

Em alguns casos, o fundo pode ser apenas uma cor específica, sem uma imagem .jpg associada.

Quando houver transição de cena, poderá ser executada uma animação enquanto a UI faz a transição de uma imagem de fundo para outra.

### Balão de tutorial

Um quadro para exibir texto de instruções ao usuário. Quando o balão é exibido, o leitor entra em estado de pausa, aguardando o usuário tocar em qualquer parte da área visível. Após essa interação, o sistema deve avançar para a próxima cena do storyboard.

O texto de instruções pode ter várias parágrafos, sendo que o storyboard irá mandar exibir um de cada vez. Haverá animações para a abertura e o fechamento do balão, mas não entre a mudança de texto (exceto quando o novo parágrafo tem mais ou menos linhas do que o anterior, situação no qual o balão deve ser redimensionado para acomodar melhor o novo parágrafo).

### Balão de narração

Tem funcionamento idêntico ao balão de tutorial, mas com uma identidade visual própria, para exibir um parágrafo de texto narrativo da história do livro (quando não se trata de uma fala ou pensamento de um personagem).

### Quadro de personagem

Esse é um quadro contendo vários componentes, com o objetivo de mostrar o nome, a imagem do personagem e um balão de fala ou pensamento.

Pode ser de dois tipos: protagonista ou coadjuvante. A diferença básica é que para protagonista os elementos devem se alinhar à esquerda da área de visão enquanto para coadjuvantes o alinhamento deve ser à direita.

Os elementos do quadro de personagem são:
- Janela: uma espécie de imagem de fundo para a imagem do personagem, alinhada à esquerda para protagonista e à direita para coadjuvante. Funciona como uma moldura, para que a imagem do personagem não fique "flutuando" sobre a imagem de fundo do leitor. A janela usa uma imagem .jpg diferente dependendo do humor do personagem.
- Foto: a imagem do personagem, exibida sobre a janela. Usa uma imagem .jpg diferente dependendo do humor do personagem. A imagem precisará ser espelhada horizontalmente quando se tratar de um personagem coadjuvante.
- Balão de fala: funcionamento idêntico aos balões de tutorial ou narração, com a diferença que devem aparecer logo abaixo da janela e não devem ocupar toda a área visível, devendo ser alinhado à esquerda para protagonista e à direita para coadjuvante.
- Balão de pensamento: idêntico ao balão de fala, mas com identidade visual própria para representar um pensamento ao invés de uma fala.
- Nome do personagem: o nome do personagem que está falando ou pensando deve aparecer como um título para o balão de fala ou pensamento.
- Recompensa: um pequeno texto (geralmente uma única palavra) podendo apresentar um pequeno ícone, que aparece junto com o balão, mas com uma animação que faz com que se mova e desapareça em poucos segundos. Serve para notificar ao usuário um efeito causado pela última opção que ele escolheu. Por exemplo, para indicar que o relacionamento do protagonista com outro personagem mudou para melhor ou para pior.
- Opções: até cinco botões que poderão aparecer abaixo do balão de fala ou pensamento. Esses botões contém opções, das quais o usuário precisa selecionar uma para que a história prossiga. Quando opções estiverem visíveis, o usuário não poderá prosseguir na história tocando em qualquer parte da área visível. Ele será obrigado a escolher uma das opções disponíveis.

### Quadro de troféu

Esse quadro deve aparecer na parte superior da área visível por alguns segundos e logo desaparecer. Serve para notificar ao usuário que ele ganhou um troféu. O quadro exibe um ícone de troféu, um título e uma curta descrição.

Ele vai aparecer sempre junto a um balão de narração, fala ou pensamento e deverá continuar na tela por todo seu tempo de duração padrão, mesmo que o usuário avance na história enquanto o quadro estiver aberto. Caso o usuário ganhe um outro troféu na sequência, o mesmo será exibido apenas depois que o primeiro sumir.

O quadro deverá desaparecer imediatamente caso o usuário saia do leitor.

### Botão Home

Um ícone de "home", transparente, que deve aparecer na parte superior esquerda da área visível por alguns segundos nas seguintes situações:
- Logo que o modo leitor é aberto
- Quando o usuário toca na parte superior da área visível
- Quando tem algum balão aberto (mas sem opções) por mais do que 5 segundos

Se o usuário tocar nesse botão, o leitor deve ser encerrado e o sistema deve voltar a exibir os detalhes do livro, no modo biblioteca

### Botão Troféus

Um ícone de troféu, transparente, que deve aparecer na parte superior direita da área visível por alguns segundos, junto com o botão Home. Os dois botões sempre devem aparecer e desaparecer juntos.

Se o usuário tocar nesse botão, o leitor deve exibir um diálogo mostrando a lista de troféus adquirida pelo usuário no capítulo atual, bem como os que ele não adquiriu ainda. Para os troféis não adquiridos, o nome e a descrição devem mostrar pontos de interrogação (ex: ??????)

### Botão Música

Um ícone transparente para permitir ativar ou desativar a música

### Botão "clique aqui"

Um ícone que deve aparecer na parte inferior da tela quando tem algum balão aberto (mas sem opções) por mais do que 5 segundos. Serve para indicar ao usuário que a aplicação está aguardando ele tocar em qualquer parte da tela para continuar a história.

### Entrada de texto

Em certas situações (como sempre, isso é controlado pelo storyboard) será necessário abrir um pequeno diálogo para que o usuário possa digitar algum texto. Isso será usado para renomar um personagem ou para solicitar alguma informação necessária para prosseguir na história. O diálogo deverá exibir uma mensagem e um input, e poderá ter validação REGEX ou por quantidade de caracteres (o diálogo precisará ser capaz de exibir uma pequena mensagem de erro). Ao informar um texto e pressionar ENTER, o diálogo será fechado e a história prosseguirá seu curso normal, sendo que o storyboard irá decidir o que fazer com o texto que o usuário informou.

## Sobre a UI

A UI deve ser projetada primariamente para ser acessada por dispositivos móveis, como smartphones. A área de visão deve se adequar aos tamanhos de tela mais comuns nesse tipo de dispositivo, sempre no padrão retrato (o recurso de rotacionamento de tela, comum do dispositivo, deve ser desativado, se possível) sendo que para dispositivos com tela mais larga, a proporção deve ser mantida incluindo um fundo escuro à esquerda e à direita da área de visão, de forma que fique centralizada.

## Estrutura dos arquivos de recurso

Sobre os livros a exibir na biblioteca, os dados ficarão em arquivos estáticos na pasta /assets/books:
- categories.json: lista das categorias que devem aparecer da biblioteca
- books.json: lista dos livros disponíveis

A pasta /assets/books terá uma subpasta para cada livro disponível no books.json. Essa pasta deve conter:
- thumbnail.png: miniatura da capa do livro a ser exibida na lista de livros da biblioteca
- cover.png: imagem grande da capa do livro, a ser exibida como imagem de fundo na tela de propriedades do livro
- details.json: detalhes estáticos sobre o livro (sinopse, quantidade de capítulos publicados, se o livro já foi totalmente publicado ou se ainda serão lançados mais capítulos, etc)
- trophies.json: a lista de todos os troféus de cada capítulo do livro

A pasta do livro terá as seguintes subpastas:
- chapters: conterá arquivos com o storyboard de cada capítulo individual. O nome dos arquivos deverá ser o número do capítulo com extensão .stbasic. O .stbasic é um arquivo texto contendo comandos na liguagem "Storybasic", que foi projetada exclusivamente para esta aplicação. Um dos módulos da aplicação é um parser para arquivos Storybasic.
- char: imagens de personagens
- window: imagens de janela
- background: imagens de fundo
- music: arquivos de música (mp3)

## Stack

A aplicação será implementada em dotnet, com frontend em Blazor webassembly, que será publicado como um site estático, sem um backend ou integração com qualquer api.

A aplicação deverá ter um Storage para armazenar o progresso do usuário em cada um dos livros que ele iniciou, bem como a lista dos troféus adquiridos. Neste primeiro momento, o armazenamento precisa ser feito no LocalStore do navegador. Futuramente poderemos incluir um mecanismo de autenticação e um backend para salvar os dados em um banco de dados, mas isso é uma ideia para o futuro.
