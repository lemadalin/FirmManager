# FirmManager

FirmManager is a web application that allows business owners to run their website with a business mindset. It's easier than ever to manage all the customers, products and orders that your business has, and also visualize data as interactive graphics.

## Technologies used
*Frontend:*
* HTML/CSS/JavaScript
* SASS
* Bootstrap
* jQuery
* D3.js
* Gulp.js
* AJAX, JSON
* Google Maps API

*Backend:*
* ASP.NET Core 2.0
* Entity Framework
* LINQ
* Microsoft SQL Server / LocalDB
* Razor Pages

*Authentication:*
* ASP.NET Identity

## Getting Started

### Prerequisites
.NET Core 2.0 or later, Visual Studio 2017, Microsoft SQL Server / LocalDB.

### Description

#### Login page

The application begins with the login page, which is the only resource that guests have access to, unless they log in. After successful authentication, the guest becomes a user with full access to all resources.

#### Left menu

The navigation menu is on the left, as a sidebar menu. It has all the pages that the user can access.

### Customers page

The user can choose to manage customers. On this page, he has an overview of all the customers in the database, which are displayed in a paginated table. He can search for any customer, and with the help of an autocomplete function, it becomes easier. New customers can be added, edited or removed, and also every customer has a profile page that shows more details about him, as well as his order history. If the user wants to create a new customer, he can write only a part of his address and then he can choose the full address from an autocompleted list.

### Products page

If the user chooses to manage the products, he can view all the products in the database. He can also add, edit, remove or see more details about a certain product. An autocomplete function for searching a product and the pagination for the product table are present as well.

### Orders page

The orders page offers the same features of the customers page and products page. Also, when the user wants to see more details about a certain order, he can also view on a map where the order needs to be delivered to. There are 4 warehouses all accross the country, and the user can see what is the nearest warehouse to the destination and how much time does it take for the delivery to be made. Also, every order has a status (either Production, Shipped or Canceled). If a new order is created or the status of a certain order gets changed, the customer gets notified via email about the an order has been registered in his name or that his order has changed. 

### Statistics page

The statistics page has a few interesting graphics based on the data from the database:
* Number of customers from each state
* Number of orders that have been made each month, in the last two years
* Number of sold units from each product category
* Total sales from each product category

## Author

**Madalin Bartos**



