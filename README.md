# Contador de Votos
## Requisitos
1. .Net 3.1  
2. Conexão com a internet  
3. Chrome Driver na versão major do instalado na máquina que irá rodar. Download da versão compatível com o Chrome clique [aqui](https://chromedriver.chromium.org/downloads)  

## Como Executar
1. Colocar na pasta [TestadorContadorVotosZonaSecao\drivers]() o executável do Chrome Driver.  
2. Rodar a suite de testes do Visual Studio ou o commando de testes `dotnet test TestadorContadorVotosSecao.csproj`, dentro da pasta `TestadorContadorVotosZonaSecao` da solução.  
3. Quando o teste finalizar, irá criar um arquivo com o nome contendo a **data** e **hora** mais **-ApuracaoSomatorioVotacao** no nome do arquivo e a extensão do mesmo será **.txt** na pasta de testes, contendo o sumário da votação de todas as zonas e respectivas seções da cidade.  
4. Para validar o resultado do arquivo gerado, verificar a cidade no [mapa](https://especiaisg1.globo/politica/eleicoes/2022/mapas/mapa-da-apuracao-no-brasil-presidente/1-turno/) ou nesse outro [mapa](https://infograficos.valor.globo.com/especial/o-mapa-da-votacao-presidencial.html)  
###### Observação
1. Durante a execução do teste, evite redimencionar a tela, pois irá afetar a renderização dos componentes da página, o que pode acarretar em erros durante os testes.  
2. O teste completo dura aproximadamente 20 minutos.  
3. O código fonte do teste está com os dados da cidade de Barreiras - BA. Caso queira fazer para outra cidade, necessário alterar o teste automatizado para obter os dados da localidade desejada.  

###### Fontes/Referências/Agradecimentos/Outros
1. [TSE](https://resultados.tse.jus.br/)  
