using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Specflow_UIAutomation_Demo.Pages
{
    /// <summary>
    /// Page Object class representing the Products page of the online shop.
    /// </summary>
    public class ProductsPage
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        /// <summary>
        /// Initializes a new instance of the ProductsPage class.
        /// </summary>
        /// <param name="webDriver">The WebDriver instance to use for browser interactions</param>
        public ProductsPage(IWebDriver webDriver)
        {
            driver = webDriver;
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Element Locators
        private By productsTitle = By.ClassName("title");
        private By inventoryList = By.ClassName("inventory_list");
        private By inventoryItems = By.ClassName("inventory_item");
        private By productSort = By.ClassName("product_sort_container");
        private By shoppingCart = By.ClassName("shopping_cart_link");
        private By cartBadge = By.ClassName("shopping_cart_badge");
        private By backToProductsBtn = By.Id("back-to-products");

        // Dynamic locators
        private string addToCartButtonFormat = "add-to-cart-{0}";
        private string removeButtonFormat = "remove-{0}";
        private string productNameFormat = "//div[text()='{0}']";
        private string productPriceFormat = "//div[text()='{0}']/ancestor::div[@class='inventory_item']//div[@class='inventory_item_price']";

        /// <summary>
        /// Converts a product name to its corresponding ID format.
        /// Example: "Sauce Labs Backpack" becomes "sauce-labs-backpack"
        /// </summary>
        private string ConvertToProductId(string productName)
        {
            // Convert to lowercase
            string id = productName.ToLower();
            // Replace spaces with hyphens
            id = Regex.Replace(id, @"\s+", "-");
            return id;
        }

        /// <summary>
        /// Adds a product to the cart using the product name.
        /// Automatically converts the product name to the correct ID format.
        /// </summary>
        /// <param name="productName">The display name of the product (e.g., "Sauce Labs Backpack")</param>
        public void AddProductToCart(string productName)
        {
            string productId = ConvertToProductId(productName);
            var addButton = driver.FindElement(By.Id(string.Format(addToCartButtonFormat, productId)));
            addButton.Click();
        }

        public void RemoveProductFromCart(string productName)
        {
            string productId = ConvertToProductId(productName);
            var removeButton = driver.FindElement(By.Id(string.Format(removeButtonFormat, productId)));
            removeButton.Click();
        }

        public bool IsProductPageDisplayed()
        {
            return wait.Until(d => d.FindElement(productsTitle).Displayed);
        }

        public void SortProducts(string sortOption)
        {
            var sortDropdown = new SelectElement(driver.FindElement(productSort));
            sortDropdown.SelectByText(sortOption);
        }

        public List<string> GetAllProductNames()
        {
            var products = driver.FindElements(inventoryItems);
            return products.Select(p => p.FindElement(By.ClassName("inventory_item_name")).Text).ToList();
        }

        public string GetProductImageUrl(string productName)
        {
            try
            {
                // Find the product name element
                var productElement = driver.FindElement(By.XPath(string.Format(productNameFormat, productName)));
                // Move up to the product container
                var productContainer = productElement.FindElement(By.XPath("./ancestor::div[@class='inventory_item']"));
                // Find the first image inside the product container
                var imgElement = productContainer.FindElement(By.CssSelector("img"));
                // Return the src attribute value
                return imgElement.GetAttribute("src") ?? string.Empty;
            }
            catch (NoSuchElementException)
            {
                return string.Empty;
            }
        }

        public double GetProductPrice(string productName)
        {
            var priceElement = driver.FindElement(By.XPath(string.Format(productPriceFormat, productName)));
            string priceText = priceElement.Text.Replace("$", "");
            return double.Parse(priceText);
        }

        public void ClickOnProduct(string productName)
        {
            var productElement = driver.FindElement(By.XPath(string.Format(productNameFormat, productName)));
            productElement.Click();
        }

        public void OpenShoppingCart()
        {
            driver.FindElement(shoppingCart).Click();
        }

        public int GetCartItemCount()
        {
            try
            {
                var badge = driver.FindElement(cartBadge);
                return int.Parse(badge.Text);
            }
            catch (NoSuchElementException)
            {
                return 0;
            }
        }

        public bool IsProductAvailable(string productName)
        {
            try
            {
                return driver.FindElement(By.XPath(string.Format(productNameFormat, productName))).Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public string GetProductDescription(string productName)
        {
            var productElement = driver.FindElement(By.XPath(string.Format(productNameFormat, productName)));
            var productContainer = productElement.FindElement(By.XPath("./ancestor::div[@class='inventory_item']"));
            return productContainer.FindElement(By.ClassName("inventory_item_desc")).Text;
        }

        public bool IsAddToCartButtonEnabled(string productName)
        {
            string productId = ConvertToProductId(productName);
            var addButton = driver.FindElement(By.Id(string.Format(addToCartButtonFormat, productId)));
            return addButton.Enabled;
        }

        public void WaitForProductListToLoad()
        {
            wait.Until(d => d.FindElement(inventoryList).Displayed);
        }

        public Dictionary<string, double> GetAllProductsAndPrices()
        {
            var products = driver.FindElements(inventoryItems);
            var productPrices = new Dictionary<string, double>();

            foreach (var product in products)
            {
                string name = product.FindElement(By.ClassName("inventory_item_name")).Text;
                string priceText = product.FindElement(By.ClassName("inventory_item_price")).Text.Replace("$", "");
                double price = double.Parse(priceText);
                productPrices.Add(name, price);
            }

            return productPrices;
        }
    }
}
