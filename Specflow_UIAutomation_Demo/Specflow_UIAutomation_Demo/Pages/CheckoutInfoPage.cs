using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specflow_UIAutomation_Demo.Pages
{
    public class CheckoutInfoPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        /// <summary>
        /// Initializes a new instance of the CheckoutInfoPage class.
        /// This page handles the customer information input during checkout.
        /// </summary>
        public CheckoutInfoPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Element Locators
        private By firstNameInput = By.Id("first-name");
        private By lastNameInput = By.Id("last-name");
        private By postalCodeInput = By.Id("postal-code");
        private By continueButton = By.Id("continue");
        private By cancelButton = By.Id("cancel");
        private By errorMessage = By.CssSelector("[data-test='error']");

        // Page Methods
        public bool IsCheckoutInfoPageDisplayed()
        {
            return wait.Until(d => d.FindElement(firstNameInput).Displayed);
        }

        public void EnterFirstName(string firstName)
        {
            driver.FindElement(firstNameInput).SendKeys(firstName);
        }

        public void EnterLastName(string lastName)
        {
            driver.FindElement(lastNameInput).SendKeys(lastName);
        }

        public void EnterPostalCode(string postalCode)
        {
            driver.FindElement(postalCodeInput).SendKeys(postalCode);
        }

        public void ClickContinueBtn()
        {
            driver.FindElement(continueButton).Click();
        }

        public void ClickCancel()
        {
            driver.FindElement(cancelButton).Click();
        }

        public string GetErrorMessage()
        {
            return driver.FindElement(errorMessage).Text;
        }

        public bool HasError()
        {
            try
            {
                return driver.FindElement(errorMessage).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void FillCheckoutInfo(string firstName, string lastName, string postalCode)
        {
            EnterFirstName(firstName);
            EnterLastName(lastName);
            EnterPostalCode(postalCode);
        }
    }
}
