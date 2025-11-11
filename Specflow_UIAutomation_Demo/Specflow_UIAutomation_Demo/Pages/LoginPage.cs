using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specflow_UIAutomation_Demo.Pages
{
    public class LoginPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        public LoginPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Element locators
        By username = By.CssSelector("#user-name");
        By password = By.CssSelector("#password");
        By loginBtn = By.CssSelector("#login-button");
        By errorMessage = By.CssSelector("[data-test='error']");


        // Page Methods
        public void EnterUsername(string user)
        {
            driver.FindElement(username).SendKeys(user);
        }

        public void EnterPassword(string pass)
        {
            driver.FindElement(password).SendKeys(pass);
        }

        public void ClickLoginButton()
        {
            driver.FindElement(loginBtn).Click();
        }

        /// <summary>
        /// Checks if there is an error message displayed on the login page.
        /// </summary>
        /// <returns>True if an error message is displayed, false otherwise.</returns>
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

        /// <summary>
        /// Gets the error message text if present.
        /// </summary>
        /// <returns>The error message text or empty string if no error is present.</returns>
        public string GetErrorMessage()
        {
            return HasError() ? driver.FindElement(errorMessage).Text : string.Empty;
        }
    }
}
