Feature: OnlineShop 

To ensure users can Login successfully ,make purchases through the online shop, 
validating key functionalities such as browsing,
adding items to cart, checking out, and receiving confirmation.

@login
Scenario Outline: User attempts to login with different credentials
    Given the user is on the login page
    When the user enters username "<Username>"
    And the user enters password "<Password>"
    * the user clicks the login button
    Then the user should be able see "<Result>"
    And the system should show "<Description>"

    Examples:
        | Username                | Password       | Result  | Description                                           |
        | standard_user           | secret_sauce   | Success | User successfully logged in and sees products page    |
        | locked_out_user         | secret_sauce   | Error   | Sorry, this user has been locked out                 |
        | problem_user            | secret_sauce   | Success | User logged in but cannot see  product data image   |
        |                         | secret_sauce   | Error   | Username is required                                 |
        | standard_user           |                | Error   | Password is required                                 |
        | invalid_user            | invalid_pass   | Error   | Username and password do not match any user          |
	    | performance_glitch_user | secret_sauce   | Success | User logged in but may experience performance issues |


@login @products
Scenario Outline: Verify Product Page load and product image load with user roles
    Given the user is on the login page
    When the user enters username "<Username>"
    And the user enters password "<Password>"
    * the user clicks the login button
    Then the user should see "<ProductsDisplay>"
    And the system should validate "<ExpectedBehavior>"

    Examples:
        | Username                | Password     | ProductsDisplay      | ExpectedBehavior                                    |
        | standard_user           | secret_sauce | All products         | User can view all products and their correct images |
        | problem_user            | secret_sauce | Products with issues | User can view all products and their correct images |
        | locked_out_user         | secret_sauce | No access            | User cannot access the products page                |
        | invalid_user            | secret_sauce | Access denied        | User is redirected back to login page               |
        | performance_glitch_user | secret_sauce | All products         | User can view all products and their correct images         |

@login @products @sort
Scenario: Standard user can sort products
    Given the user is logged in as "standard_user"
    When the user navigates to the products page
    And sorts products by "Price (high to low)"
    Then the products should be sorted by price in descending order

@login @products @cart
Scenario: User can add single product to cart
    Given the user is logged in as "standard_user"
    When the user navigates to the products page
    And adds "Sauce Labs Backpack" to the cart
    Then the cart count should be "1"
    And the cart should contain "Sauce Labs Backpack"

@login @products @cart
Scenario: User can add all products to cart
    Given the user is logged in as "standard_user"
    When the user navigates to the products page
    And adds all products to the cart
    Then the cart count should match total products count
    And all products should be in the cart

@login @products @cart
Scenario: User can remove product from cart
    Given the user is logged in as "standard_user"
    When the user navigates to the products page
    And the user adds these products to cart
        | Product Name          |
        | Sauce Labs Backpack   |
        | Sauce Labs Bike Light |
    Then the cart count should be "2"
    When the user removes "Sauce Labs Backpack" from cart
    Then the cart count should be "1"
    And the cart should contain "Sauce Labs Bike Light"
    * the cart should not contain "Sauce Labs Backpack"

@login @products @cart @checkout
Scenario: Standard user can complete checkout process
    Given the user is logged in as "standard_user"
    When the user navigates to the products page
    And adds "Sauce Labs Backpack" to the cart
    * the user proceeds to checkout
    * enters the following shipping information
        | Field       | Value           |
        | First Name | Pankaj           |
        | Last Name  | Nandy            |
        | Zip Code   | SE185BA          |
    Then completes the purchase
    Then the checkout confirmation should be displayed
    * the cart should be empty

@login @products @cart @checkout
Scenario: Standard user can complete checkout process with multiple items
    Given the user is logged in as "standard_user"
    When the user navigates to the products page
    And the user adds these products to cart
        | Product Name          |
        | Sauce Labs Backpack   |
        | Sauce Labs Bike Light |
    Then the cart count should be "2"
    When the user proceeds to checkout
    When enters the following shipping information
        | Field      | Value    |
        | First Name | John     |
        | Last Name  | Doe      |
        | Zip Code   | 12345    |
    Then the total price should be the sum of all items    
    Then completes the purchase
    Then the checkout confirmation should be displayed
    And the cart should be empty