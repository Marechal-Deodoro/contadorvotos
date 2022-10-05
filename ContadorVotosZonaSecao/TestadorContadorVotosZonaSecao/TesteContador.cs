using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;

namespace TestadorContadorVotosZonaSecao
{
    public class Tests
    {
        string urlBase = "https://resultados.tse.jus.br/oficial/app/index.html#/eleicao;e=e544;uf=ba;ufbu=ba;mubu=33634;zn=0070;se=0024/dados-de-urna";
        string arquivoSaida = $"{DateTime.Now.ToString("yyyyMMddhhmmss")}-ApuracaoSomatorioVotacao.txt";
        string localPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
        public IWebDriver driver;

        [SetUp]
        public void Setup()
        {

            driver = new ChromeDriver(localPath + @"\drivers\");
            driver.Manage().Window.Maximize();

            
            driver.Url = urlBase;
            var timeout = 10000;
            var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeout));
            wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

            Thread.Sleep(1000);

            driver.FindElement(By.LinkText("Dados de Urna"));

            var comboCidade = driver.FindElement(By.ClassName("leading-tight"));
            comboCidade.Click();
            Thread.Sleep(1000);

            var combo00 = driver.FindElement(By.Id("mat-select-0"));
            combo00.Click();

            var combo00Estado = driver.FindElement(By.Id("mat-option-7"));
            combo00Estado.Click();

            var combo01 = driver.FindElement(By.Id("mat-select-2"));
            combo01.Click();
            Thread.Sleep(500);


            var combo01Ano = driver.FindElement(By.Id("mat-option-30"));
            combo01Ano.Click();
            Thread.Sleep(500);

            var comboTipoEleicao = driver.FindElement(By.Id("mat-select-4"));
            comboTipoEleicao.Click();
            Thread.Sleep(500);

            driver.FindElement(By.Id("mat-option-32")).Click();
            Thread.Sleep(500);


            var resultadoParametrosBusca01 = driver.FindElement(By.ClassName("bg-agrupamento-item"));
            resultadoParametrosBusca01.Click();
            Thread.Sleep(2000);
        }

        [Test]
        public void TesteColetaDadosTSE()
        {
            driver.FindElement(By.CssSelector("div.ml-2.mr-3.text-roxo.font-bold.leading-tight")).Click();
            Thread.Sleep(2000);

            driver.FindElement(By.Id("mat-option-37")).Click();
            Thread.Sleep(500);

            driver.FindElement(By.Id("mat-option-98")).Click();
            Thread.Sleep(500);

            driver.FindElement(By.CssSelector("ion-button.md.button.button-block.button-solid.ion-activatable.ion-focusable.hydrated")).Click();
            Thread.Sleep(1000);

            driver.FindElement(By.XPath("//div/span[contains(text(),'Zona')]")).Click();
            Thread.Sleep(500);

            IDictionary<int, int> candidatoVotos = new Dictionary<int, int>();
            var zonas = driver.FindElements(By.XPath("//mat-option/child::span[contains(text(),'Zona')]"));
            candidatoVotos = LoopingSecoes(candidatoVotos, zonas);
            StreamWriter streamEscritor = new StreamWriter(localPath + "\\" + arquivoSaida);
            foreach (var item in candidatoVotos)
            {
                streamEscritor.WriteLine($"Numero Candidato {item.Key} - Votos Candidato {item.Value}");
            }
            streamEscritor.Close();
            Assert.IsNotEmpty(candidatoVotos);
        }

        private IDictionary<int, int> LoopingSecoes(IDictionary<int, int> candidatoVotos, ReadOnlyCollection<IWebElement> zonas)
        {
            for (int i = 0; i < zonas.Count; i++)
            {
                IWebElement zona;
                if (i == 0)
                {
                    zona = driver.FindElement(By.XPath("//div/child::span[contains(text(),'Zona')]"));
                }
                else if (i == 1)
                {
                    zona = driver.FindElement(By.XPath($"//div/mat-option[position()={i + 1}]"));
                }
                else
                {
                    zona = driver.FindElement(By.XPath("//div/span[contains(text(),'Zona')]"));
                    zona.Click();
                    zona = driver.FindElement(By.XPath($"//div/mat-option[position()={i + 1}]"));
                }

                if (zona.Text.Split(" ").Length == 1) continue;

                zona.Click();
                Thread.Sleep(500);
                driver.FindElement(By.XPath("//div/child::span[contains(text(),'Seção')]")).Click();
                var comboBoxSecaoTodosItems = driver.FindElements(By.XPath("//div/child::span[contains(text(),'Seção')]"));

                candidatoVotos = ExtraiDadosdaTabeladePresidentes(candidatoVotos, comboBoxSecaoTodosItems);
            }
            return candidatoVotos;
        }

        private IDictionary<int, int> ExtraiDadosdaTabeladePresidentes(IDictionary<int, int> candidatoVotos, ReadOnlyCollection<IWebElement> comboBoxSecaoTodosItems)
        {
            for (int j = 0; j < comboBoxSecaoTodosItems.Count; j++)
            {
                if (j > 1) PreencheComboBoxSecao(j);
                comboBoxSecaoTodosItems = driver.FindElements(By.CssSelector("div > mat-option"));
                comboBoxSecaoTodosItems[j].Click();
                Thread.Sleep(100);
                var botaoPesquisar = driver.FindElement(By.CssSelector("div > button.tracking-tight"));
                if (j > 0)
                {
                    botaoPesquisar.Click();
                    Thread.Sleep(5000);

                    var totalPresidentesNaTabelaPorZonaSecao = driver.FindElements(By.XPath("//h1[contains(text(),'Presidente')]/parent::div/following-sibling::div//div[contains(@class,'row')]"));

                    for (int k = 0; k < totalPresidentesNaTabelaPorZonaSecao.Count; k++)
                    {
                        var tudo = totalPresidentesNaTabelaPorZonaSecao[k].Text.Split("\r\n");

                        if (!string.IsNullOrEmpty(tudo[0]))
                        {
                            var identificacaoCandidato = Convert.ToInt16(tudo[0]);
                            var totalVotosCandidato = Convert.ToInt32(tudo[2]);
                            if (candidatoVotos.ContainsKey(identificacaoCandidato))
                            {
                                candidatoVotos[identificacaoCandidato] += totalVotosCandidato;
                            }
                            else
                            {
                                candidatoVotos[identificacaoCandidato] = totalVotosCandidato;
                            }
                        }
                    }
                }
            }
            return candidatoVotos;
        }

        private IWebElement PreencheComboBoxSecao(int i)
        {
            IWebElement comboBoxSecao;
            if (i == 0)
            {
                comboBoxSecao = driver.FindElement(By.Id("mat-select-12"));
            }
            else
            {
                comboBoxSecao = driver.FindElement(By.XPath("//span[contains(text(),'Seção ')]"));
            }
            comboBoxSecao.Click();
            Thread.Sleep(500);
            return comboBoxSecao;
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}