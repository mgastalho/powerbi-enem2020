# powerbi-enem2020

Problema: Explorar e analisar os dados do ENEM.
Perguntas para serem respondidas:

- Qual o total de inscritos?
- Qual a média de nota geral?
- Qual a média de nota por assunto?
- Qual o total de homens e mulheres inscritos?
- Qual o total de inscritos por etnia?
- Qual UF com maior nota e inscritos?
- Qual a média por tipo de escola?
- Quantidade de treineiros?

Para isso, realizei o download dos dados em https://download.inep.gov.br/microdados/microdados_enem_2020.zip

OBJETIVO: Colocar os dados do CSV em uma base de dados SQL SERVER.

Eu precisava analisar os dados que eu tinha em mãos, mas meu computador não suporta abrir um CSV tão grande. Eu precisava criar a tabela que iria receber esses dados.
No SSI, criei uma tarefa de execução de Script e configurei alguns parâmetros:
- Tipo de arquivo a ser lido: CSV
- Localização do arquivo: Pasta onde o arquivo está localizado
- Localização da pasta de log: Pasta para onde o log da execução do script será rodado
- Total de registros: Total de registros a serem lidos. Como eu precisava analisar inicialmente os dados para saber o que eu tinha, criei esse parâmetro para ler e inserir uma quantidade limitada de registros. Se informado valor, ele limita a quantidade informada, se não, ele lê o arquivo inteiro.

Após os registros no banco de dados, pude realizar a construção da minha base de dados. 
Executei esse pedaço via notebook no AZURE DATA STUDIO

Assim que todos os meus dados estavam na base de dados, finalmente pude ler os registros no Power BI e construir o relatório para responder as perguntas iniciais.

Link para o painel do power bi:
https://bit.ly/PBIenem2020
