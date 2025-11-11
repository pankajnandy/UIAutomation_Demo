using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using Specflow_UIAutomation_Demo.Pages;
using Specflow_UIAutomation_Demo.Utilities;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using System.Collections.Generic;
using System.Linq;

namespace Specflow_UIAutomation_Demo.StepDefinitions
{
    [Binding]
    public class OnlineShopStepDefinitions
    {
        private readonly IWebDriver _driver;
        private readonly Helper _helper;
        private readonly LoginPage _loginPage;
        private readonly ProductsPage _productsPage;
        private readonly YourCartPage _cartPage;
        private readonly CheckoutOverviewPage _checkoutOverviewPage;
        private readonly CheckoutInfoPage _checkoutInfoPage;
        private readonly CheckoutCompletePage _checkoutCompletePage;
        private readonly ScenarioContext _scenarioContext;

        public OnlineShopStepDefinitions(IWebDriver driver, ScenarioContext scenarioContext)
        {
            _driver = driver;
            _helper = new Helper();
            _loginPage = new LoginPage(driver);
            _productsPage = new ProductsPage(driver);
            _cartPage = new YourCartPage(driver);
            _checkoutOverviewPage = new CheckoutOverviewPage(driver);
            _checkoutInfoPage = new CheckoutInfoPage(driver);
            _checkoutCompletePage = new CheckoutCompletePage(driver);
            _scenarioContext = scenarioContext;
        }

        [Given(@"the user is on the login page")]
        public async Task GivenTheUserIsOnTheLoginPage()
        {
            var environmentInfo = await _helper.GetEnvironmentDataAsync();
            _driver.Navigate().GoToUrl(environmentInfo.Url);
        }

        [When(@"the user enters username ""(.*)""")]
        public void WhenTheUserEntersUsername(string username)
        {
            _loginPage.EnterUsername(username);
        }

        [When(@"the user enters password ""(.*)""")]
        public void WhenTheUserEntersPassword(string password)
        {
            _loginPage.EnterPassword(password);
        }

        [When(@"the user clicks the login button")]
        public void WhenTheUserClicksTheLoginButton()
        {
            _loginPage.ClickLoginButton();
        }

        [Then(@"the user should be able see ""(.*)""")]
        public void ThenTheUserShouldBeAbleSee(string result)
        {
            if (result == "Success")
            {
                Assert.IsTrue(_productsPage.IsProductPageDisplayed(), 
                    "Products page should be displayed after successful login");
            }
            else if (result == "Error")
            {
                Assert.IsTrue(_loginPage.HasError(), 
                    "Error message should be displayed");
            }
        }

        [Then(@"the system should show ""(.*)""")]
        public void ThenTheSystemShouldShow(string description)
        {
            if (_loginPage.HasError())
            {
                StringAssert.Contains(_loginPage.GetErrorMessage(), description,
                    $"Error message should contain: {description}");
            }
            else
            {
                Assert.IsTrue(_productsPage.IsProductPageDisplayed(),
                    $"LogIn is success, Products page should be displayed. {description}");
            }
        }

        [Then(@"the user should see ""(.*)""")]
        public void ThenTheUserShouldSee(string productsDisplay)
        {
            switch (productsDisplay)
            {
                case "All products":
                    //Product Page is displayed
                    Assert.IsTrue(_productsPage.IsProductPageDisplayed());

                    break;
                    
                case "Products with issues":
                    //Product Page is displayed
                    Assert.IsTrue(_productsPage.IsProductPageDisplayed());
                    
                    // Additional checks for problem user could be added here
                    break;
                    
                case "No access":
                case "Access denied":
                    Assert.IsTrue(_loginPage.HasError());
                    break;
            }
        }

        [Then(@"the system should validate ""(.*)""")]
        public void ThenTheSystemShouldValidate(string accessBehavior)
        {
            switch (accessBehavior)
            {
                case "User can view all products and their correct images":
                    Assert.IsTrue(_productsPage.IsProductPageDisplayed());
                    var productNames = _productsPage.GetAllProductNames();
                    foreach (var productName in productNames)
                    {
                        var imageUrl = _productsPage.GetProductImageUrl(productName);
                        Assert.IsFalse(imageUrl.Contains("sl-404"),
                            $"Product {productName} image should load for user.");
                    }
                    break;

                    
                case "User cannot access the products page":
                    Assert.IsTrue(_loginPage.HasError());
                    StringAssert.Contains(_loginPage.GetErrorMessage(), "locked out");
                    break;
                    
                case "User is redirected back to login page":
                    Assert.IsTrue(_loginPage.HasError());
                    break;
            }
        }

        [Given(@"the user is logged in as ""(.*)""")]
        public async Task GivenTheUserIsLoggedInAs(string userType)
        {
            var environmentInfo = await _helper.GetEnvironmentDataAsync();
            _driver.Navigate().GoToUrl(environmentInfo.Url);
            _loginPage.EnterUsername(userType);
            _loginPage.EnterPassword("secret_sauce");
            _loginPage.ClickLoginButton();
        }

        [When(@"the user navigates to the products page")]
        public void WhenTheUserNavigatesToTheProductsPage()
        {
            _productsPage.WaitForProductListToLoad();
            Assert.IsTrue(_productsPage.IsProductPageDisplayed(), 
                "Products page should be displayed");
        }

        [When(@"sorts products by ""(.*)""")]
        public void WhenSortsProductsBy(string sortOption)
        {
            _productsPage.SortProducts(sortOption);
            // Store the sort option for later verification
            _scenarioContext["SortOption"] = sortOption;
        }

        [Then(@"the products should be sorted by price in descending order")]
        public void ThenTheProductsShouldBeSortedByPriceInDescendingOrder()
        {
            // Get all products and their prices
            var productPrices = _productsPage.GetAllProductsAndPrices();

            // Create a list of actual prices from the dictionary values
            List<double> actualPrices = new List<double>();
            foreach (var price in productPrices.Values)
            {
                actualPrices.Add(price);
            }

            // Create a list of expected prices sorted in descending order
            List<double> expectedPrices = new List<double>(productPrices.Values);
            expectedPrices.Sort();     // Sorts in ascending order
            expectedPrices.Reverse();  // Reverses to descending order


            // Compare the lists
            CollectionAssert.AreEqual(expectedPrices, actualPrices, 
                "Products are not sorted by price in descending order");

            // Additional verification that prices are actually different
            Assert.IsTrue(actualPrices.Distinct().Count() > 1, 
                "There should be products with different prices to verify sorting");
        }

        [When(@"adds ""(.*)"" to the cart")]
        public void WhenAddsToTheCart(string productName)
        {
            _productsPage.AddProductToCart(productName);
        }

        [When(@"adds all products to the cart")]
        public void WhenAddsAllProductsToTheCart()
        {
            Helper.WaitForPageToLoad(_driver);
            //Thread.Sleep(1000); // Temporary wait to ensure page is loaded
            var allProducts = _productsPage.GetAllProductNames();
            foreach (var product in allProducts)
            {
                _productsPage.AddProductToCart(product);
            }
            // Store the count for later verification
            _scenarioContext["TotalProducts"] = allProducts;
        }

        [When(@"the user adds these products to cart")]
        public void WhenTheUserAddsTheseProductsToCart(Table table)
        {
            var products = table.Rows.Select(row => row["Product Name"]).ToList();
            foreach (var product in products)
            {
                _productsPage.AddProductToCart(product);
            }
        }

        [When(@"the user removes ""(.*)"" from cart")]
        public void WhenTheUserRemovesFromCart(string productName)
        {
            _productsPage.RemoveProductFromCart(productName);
        }

        [Then(@"the cart count should be ""(.*)""")]
        public void ThenTheCartCountShouldBe(string expectedCount)
        {
            var actualCount = _productsPage.GetCartItemCount();
            Assert.AreEqual(int.Parse(expectedCount), actualCount, 
                $"Cart count should be {expectedCount}");
        }

        [Then(@"the cart count should match total products count")]
        public void ThenTheCartCountShouldMatchTotalProductsCount()
        {
            _scenarioContext.TryGetValue("TotalProducts", out List<string> totalProducts);
            var expectedCount = totalProducts.Count();
            var actualCount = _productsPage.GetCartItemCount();
            Assert.AreEqual(expectedCount, actualCount, 
                "Cart count should match the total number of products");
        }

        [Then(@"the cart should contain ""(.*)""")]
        public void ThenTheCartShouldContain(string productName)
        {
            _productsPage.OpenShoppingCart();
            Assert.IsTrue(_cartPage.IsItemInCart(productName), 
                $"Product {productName} should be in the cart");
        }

        [Then(@"the cart should not contain ""(.*)""")]
        public void ThenTheCartShouldNotContain(string productName)
        {
            _productsPage.OpenShoppingCart();
            Assert.IsFalse(_cartPage.IsItemInCart(productName), 
                $"Product {productName} should not be in the cart");
        }

        [Then(@"all products should be in the cart")]
        public void ThenAllProductsShouldBeInTheCart()
        {
            _scenarioContext.TryGetValue("TotalProducts", out List<string> totalProducts);
            _productsPage.OpenShoppingCart();
            var cartItems = _cartPage.GetCartItems();
            
            CollectionAssert.AreEquivalent(totalProducts, cartItems, 
                "All products should be present in the cart");
        }

        [When(@"the user proceeds to checkout")]
        public void WhenTheUserProceedsToCheckout()
        {
            _productsPage.OpenShoppingCart();
            var cartTotal = _cartPage.GetCartTotal();
            _cartPage.ClickCheckout();
            // Store the current state for verification
            _scenarioContext["CheckoutStarted"] = true;
            _scenarioContext["CartTotal"] = cartTotal;
        }

        [When(@"enters the following shipping information")]
        public void WhenEntersTheFollowingShippingInformation(Table table)
        {
            var shippingInfo = table.Rows.ToDictionary(row => row["Field"], row => row["Value"]);

            // Store shipping info for later verification if needed
            _scenarioContext["ShippingInfo"] = shippingInfo;

            _checkoutInfoPage.EnterFirstName(shippingInfo["First Name"]);
            _checkoutInfoPage.EnterLastName(shippingInfo["Last Name"]);
            _checkoutInfoPage.EnterPostalCode(shippingInfo["Zip Code"]);
            _checkoutInfoPage.ClickContinueBtn();
        }



        [Then(@"the checkout confirmation should be displayed")]
        public void ThenTheCheckoutConfirmationShouldBeDisplayed()
        {
            // Verify that we went through the complete checkout flow
            Assert.IsTrue(_scenarioContext.TryGetValue("CheckoutStarted", out bool checkoutStarted) && checkoutStarted,
                "Checkout process should have been started");
            Assert.IsTrue(_scenarioContext.TryGetValue("PurchaseCompleted", out bool purchaseCompleted) && purchaseCompleted,
                "Purchase should have been completed");

            // Additional verification of confirmation message
            StringAssert.Contains(_checkoutCompletePage.GetConfirmationMessage(), "Your order has been dispatched, and will arrive just as fast as the pony can get there!",
                "Order confirmation should show thank you message");

            Assert.IsTrue(_checkoutCompletePage.IsCheckoutCompletePageDisplayed(),
                "Order confirmation page should be displayed");

            Assert.IsTrue(_checkoutCompletePage.ValidateOrderSuccess(),
                "Order is successfully completed");
        }


        [Then(@"the cart should be empty")]
        public void ThenTheCartShouldBeEmpty()
        {
            _checkoutCompletePage.ClickBackHome();
            var cartCount = _productsPage.GetCartItemCount();
            Assert.AreEqual(0, cartCount, "Cart should be empty after successful purchase");
        }

        [Then(@"the total price should be the sum of all items")]
        public void ThenTheTotalPriceShouldBeTheSumOfAllItems()
        {
            _scenarioContext.TryGetValue("CartTotal", out double yourCartTotal);

            // Get subtotal, tax, and total from the checkout overview page
            var expectedSubTotal = _checkoutOverviewPage.GetSubtotal();
            var expectedTax = _checkoutOverviewPage.GetTax();
            var expectedTotal = _checkoutOverviewPage.GetTotal();
            var calculatedTotal = expectedSubTotal + expectedTax;

            // Verify the totals
            Assert.AreEqual(expectedSubTotal, yourCartTotal, 
                "Cart total should match the sum of all item prices");
            Assert.AreEqual(expectedTotal, calculatedTotal, 
                "Total price should be the sum of subtotal and tax");
        }


        [Then(@"completes the purchase")]
        public void ThenCompletesThePurchase()
        {
            _checkoutOverviewPage.ClickFinish();
            // Store purchase completion state for verification in confirmation step
            _scenarioContext["PurchaseCompleted"] = true;
        }

    }
}
