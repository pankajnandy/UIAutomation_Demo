using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Specflow_UIAutomation_Demo.Pages
{
    public class CheckoutOverviewPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        /// <summary>
        /// Initializes a new instance of the CheckoutOverviewPage class.
        /// This page shows the order summary and final details before purchase completion.
        /// </summary>
        public CheckoutOverviewPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Element Locators
        private By finishButton = By.Id("finish");
        private By cancelButton = By.Id("cancel");
        private By cartItems = By.ClassName("cart_item");
        private By subtotalLabel = By.ClassName("summary_subtotal_label");
        private By taxLabel = By.ClassName("summary_tax_label");
        private By totalLabel = By.ClassName("summary_total_label");
        private By itemPrice = By.ClassName("inventory_item_price");

        // Page Methods
        public bool IsCheckoutOverviewPageDisplayed()
        {
            return wait.Until(d => d.FindElement(finishButton).Displayed);
        }

        public void ClickFinish()
        {
            driver.FindElement(finishButton).Click();
        }

        public void ClickCancel()
        {
            driver.FindElement(cancelButton).Click();
        }

        // Get list of item names in the checkout overview
        public List<string> GetOrderItems()
        {
            var items = driver.FindElements(cartItems);
            return items.Select(item => item.FindElement(By.ClassName("inventory_item_name")).Text).ToList();
        }

        //get subtotal, tax, and total amounts
        public double GetSubtotal()
        {
            string subtotalText = driver.FindElement(subtotalLabel).Text;
            return ExtractPrice(subtotalText);
        }

        public double GetTax()
        {
            string taxText = driver.FindElement(taxLabel).Text;
            return ExtractPrice(taxText);
        }

        public double GetTotal()
        {
            string totalText = driver.FindElement(totalLabel).Text;
            return ExtractPrice(totalText);
        }

        /// Verify if a specific item is in the order
        public bool VerifyItemInOrder(string itemName)
        {
            try
            {
                var items = GetOrderItems();
                return items.Contains(itemName);
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private double ExtractPrice(string priceText)
        {
            return double.Parse(priceText.Split('$')[1].Trim());
        }

    }
}
