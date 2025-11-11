using BoDi;
using LivingDoc.SpecFlowPlugin;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Specflow_UIAutomation_Demo.Utilities;
using TechTalk.SpecFlow;

namespace Specflow_UIAutomation_Demo.Hooks
{
    [Binding]
    public sealed class Hooks
    {
        // For additional details on SpecFlow hooks see http://go.specflow.org/doc-hooks
        private readonly IObjectContainer _objectContainer;
        private IWebDriver driver;

        public Hooks(IObjectContainer container)
        {
            _objectContainer = container;
        }

        [BeforeFeature]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            Console.WriteLine($"Running feature...{featureContext.FeatureInfo.Title}");
        }

        [BeforeScenario]
        public void BeforeScenario(ScenarioContext scenarioContext)
        {// Get the browser name from the configuration
            string browser = Helper.GetAppConfig().Browser;

            switch (browser?.ToLower())
            {// Initialize the WebDriver based on the browser
                case "chrome":
                    driver = new ChromeDriver();
                    break;
                case "firefox":
                    driver = new FirefoxDriver();
                    break;
                default:
                    throw new NotSupportedException($"Browser is not supported.");
            }

            driver.Manage().Window.Maximize(); // Maximize the browser window
            _objectContainer.RegisterInstanceAs<IWebDriver>(driver);
        }

        [AfterScenario]
        public void AfterScenario()
        {// Resolve the WebDriver from the container
            var driver = _objectContainer.Resolve<IWebDriver>();
            // Quit the driver if it's not null
            if (driver != null)
            {
                try
                {
                    driver.Quit();
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error quitting the driver: {ex.Message}");
                }
            }
        }
    }
}