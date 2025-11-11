using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specflow_UIAutomation_Demo.Pages
{
    public class CheckoutCompletePage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        /// <summary>
        /// Initializes a new instance of the CheckoutCompletePage class.
        /// This page confirms the successful completion of an order.
        /// </summary>
        public CheckoutCompletePage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Element Locators
        private By completeHeader = By.ClassName("complete-header");
        private By completeText = By.ClassName("complete-text");
        private By backHomeButton = By.Id("back-to-products");
        private By ponyExpressImage = By.ClassName("pony_express");


        // Page Methods
        public bool IsCheckoutCompletePageDisplayed()
        {
            return wait.Until(d => d.FindElement(completeHeader).Displayed);
        }

        public string GetConfirmationHeader()
        {
            return driver.FindElement(completeHeader).Text;
        }

        public string GetConfirmationMessage()
        {
            return driver.FindElement(completeText).Text;
        }

        public void ClickBackHome()
        {
            driver.FindElement(backHomeButton).Click();
        }

        public bool IsConfirmationImageDisplayed()
        {
            try
            {
                return driver.FindElement(ponyExpressImage).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public bool ValidateOrderSuccess()
        {
            return IsCheckoutCompletePageDisplayed() &&
                   GetConfirmationHeader().Contains("Thank you for your order!") &&
                   IsConfirmationImageDisplayed();
        }
    }
}
