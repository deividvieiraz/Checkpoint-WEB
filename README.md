1. Estrutura de Classes para o Usuário

A classe User representa o modelo de dados do usuário, contendo as informações essenciais que serão utilizadas no sistema. Esta estrutura inclui o ID único do usuário, nome, e-mail e a data do último acesso. Esses dados são cruciais para o gerenciamento de sessões e a personalização da experiência do usuário. A classe foi projetada de forma simples e eficiente para facilitar a integração com o banco de dados e com o cache, garantindo acesso rápido às informações quando necessário.

2. Conexão com MySQL e Redis

Conexão com MySQL: O acesso ao banco de dados MySQL é feito através de uma camada de repositório utilizando o Dapper. O MySQL esta rodando em um container no Docker. O repositório é responsável por gerenciar as consultas SQL para buscar as informações do usuário no banco de dados. Esse modelo permite um acesso simples e direto aos dados persistentes do usuário.

Conexão com Redis: A comunicação com o Redis é realizada utilizando a biblioteca StackExchange.Redis. O Redis é utilizado como um cache em memória para armazenar as sessões ativas dos usuários. Quando o usuário faz login, as informações do usuário são armazenadas no Redis com um tempo de expiração definido (geralmente 15 minutos). A conexão com o Redis é otimizada para garantir o melhor desempenho e reduzir a latência, armazenando os dados de sessão e evitando consultas repetidas ao banco de dados.

3. Lógica de Verificação de Cache e Fallback para o Banco de Dados

O fluxo de foi projetado para otimizar o desempenho do sistema, verificando primeiro o cache (Redis) antes de fazer uma consulta ao banco de dados:

Verificação no Redis: Ao tentar buscar os usuários, o sistema verifica se os dados da sessão estão presentes no Redis. Se os dados do usuário já estão armazenados no cache e não expiraram, eles são retornados diretamente, reduzindo o tempo de resposta e a carga no banco de dados.

Fallback para o MySQL: Caso os dados não sejam encontrados no Redis (se a sessão expirou ou nunca foi armazenada), o sistema consulta o banco de dados MySQL para buscar as informações dos usuários. Após recuperar os dados do banco, o sistema armazena essas informações no Redis para futuras consultas, mantendo a consistência e o desempenho nas próximas requisições.

4. Tratamento de Exceções e Boas Práticas

Tratamento de Exceções: Para garantir a estabilidade do sistema, todas as interações com o Redis e o MySQL são protegidas por blocos de tratamento de exceções. Caso ocorram falhas na comunicação com qualquer um dos serviços, o sistema captura a exceção e retorna uma mensagem genérica para o usuário, sem expor detalhes técnicos. Isso garante uma experiência de usuário sem interrupções e facilita a manutenção do sistema.

Política de Expiração de Cache: As sessões armazenadas no Redis têm uma expiração definida (15 minutos). Essa política garante que os dados no cache sejam mantidos atualizados e que o sistema não consuma recursos desnecessários armazenando informações antigas.
