using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Specflow_UIAutomation_Demo.Pages
{
    public class YourCartPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        
        /// <summary>
        /// Initializes a new instance of the YourCartPage class.
        /// This page represents the shopping cart where users can review and modify their selected items.
        /// </summary>
        public YourCartPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Element Locators
        private By cartTitle = By.ClassName("title");
        private By cartList = By.ClassName("cart_list");
        private By cartItems = By.ClassName("cart_item");
        private By continueShoppingButton = By.Id("continue-shopping");
        private By checkoutButton = By.Id("checkout");
        private By cartQuantity = By.ClassName("cart_quantity");
        private By removeButton = By.ClassName("cart_button");
        private By itemPrice = By.ClassName("inventory_item_price");
        
        // Dynamic locators
        private string removeItemFormat = "remove-{0}";
        private string itemNameFormat = "//div[text()='{0}']";

        // Page Methods
        public bool IsCartPageDisplayed()
        {
            return wait.Until(d => d.FindElement(cartTitle).Displayed);
        }

        //Get list of item names in the cart
        public List<string> GetCartItems()
        {
            var items = driver.FindElements(cartItems);
            return items.Select(item => item.FindElement(By.ClassName("inventory_item_name")).Text).ToList();
        }

        //Get list of IWebElements representing items in the cart
        public List<IWebElement> GetCarWebElements()
        {
            IWebElement ele = driver.FindElement(cartList);
            return new List<IWebElement>(ele.FindElements(cartItems));
        }

        //Get price of a specific item in the cart
        public double GetItemPrice(string itemName)
        {
            var priceElement = driver.FindElement(By.ClassName(".inventory_item_price"));
            string priceText = priceElement.Text.Replace("$", "");
            return double.Parse(priceText);
        }

        public int GetItemQuantity(string itemName)
        {
            var item = driver.FindElement(By.XPath(string.Format(itemNameFormat, itemName)));
            var quantityElement = item.FindElement(By.XPath("./ancestor::div[@class='cart_item']"))
                                    .FindElement(cartQuantity);
            return int.Parse(quantityElement.Text);
        }

        public void RemoveItem(string itemId)
        {
            var removeBtn = driver.FindElement(By.Id(string.Format(removeItemFormat, itemId)));
            removeBtn.Click();
        }

        public void ClickContinueShopping()
        {
            driver.FindElement(continueShoppingButton).Click();
        }

        public void ClickCheckout()
        {
            driver.FindElement(checkoutButton).Click();
        }

        public double GetCartTotal()
        {
            var prices = driver.FindElements(itemPrice)
                             .Select(e => double.Parse(e.Text.Replace("$", "")));
            return prices.Sum();
        }

        public bool IsItemInCart(string itemName)
        {
            try
            {
                return driver.FindElement(By.XPath(string.Format(itemNameFormat, itemName))).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        
    }
}
