# RS Bookstore API

Welcome to the RS Bookstore API

## Description

This API interacts with a SQL database, and also interfaces with a client app to allow users to view and purchase books, and also manage their shopping carts.  
Administrators can also use the API to add, edit, and view inventory details.  

# This API also has an accompanying client app you can use to interface with the API. Download the client using this link: [Client App](https://github.com/SamTheCopy-ninja/C-Sharp-Bookstore-App-With-API.git)

## Features

1. **Add Book Endpoint:**

   * Endpoint to add a new book to the database.
   * Accepts input data including title, author, price, and quantity.
   * Validates input data and adds the book to the database.
   * Returns a success message if the book is added successfully.
   * Returns an error message if the request fails or data is invalid.

2. **Get Cart Items by User Endpoint:**

   * Endpoint to retrieve cart items for a specific user from the database.
   * Accepts user ID as input parameter.
   * Retrieves cart items associated with the provided user ID.
   * Returns the list of cart items for the user.
   * Returns an empty list if no cart items are found for the user.

3. **Get All Cart Items Endpoint:**

   * Endpoint to retrieve all cart items from the database.
   * Retrieves all cart items stored in the database.
   * Returns the list of all cart items.
   * Returns an empty list if no cart items are found in the database.

4. **Add Book to Database Functionality:**

   * Method to add a new book to the database.
   * Inserts book details including title, author, price, and quantity into the Books table.
   * Validates input data and ensures data integrity.
   * Handles exceptions and logs errors to the console.

5. **Get Cart Items by User from Database Functionality:**

   * Method to retrieve cart items for a specific user from the database.
   * Executes a SQL query to fetch cart items associated with the provided user ID.
   * Maps database results to C# objects representing cart items.
   * Handles exceptions and logs errors to the console.

6. **Get All Cart Items from Database Functionality:**

   * Method to retrieve all cart items from the database.
   * Executes a SQL query to fetch all cart items stored in the database.
   * Maps database results to C# objects representing cart items.
   * Handles exceptions and logs errors to the console.

## Installation

To install the API, download the .zip file or clone the repository:

```bash
# Clone the repository
git clone https://github.com/SamTheCopy-ninja/C-Sharp-Bookstore-API.git
```  

## Configuration  

- First begin by setting up your database in `Microsoft SSMS`:
  You will need the following tables:
```sql
-- Create Books Table
CREATE TABLE Books (
    BookId INT PRIMARY KEY IDENTITY(1,1),
    Title NVARCHAR(255) NOT NULL,
    Author NVARCHAR(255) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    Quantity INT NOT NULL
);

-- Create CartItems Table
CREATE TABLE CartItems (
    CartItemId INT PRIMARY KEY IDENTITY(1,1),
    BookId INT NOT NULL,
    Quantity INT NOT NULL,
    UserId NVARCHAR(100) NOT NULL,
    CONSTRAINT FK_CartItems_Books FOREIGN KEY (BookId) REFERENCES Books(BookId)
);
```
## - Next up, connect your database to the app

1. **Open Visual Studio:**
   - Launch Visual Studio on your machine.

2. **Open Server Explorer:**
   - In Visual Studio, go to the "View" menu.
   - Select "Server Explorer" from the dropdown.

3. **Connect to Database:**
   - Inside Server Explorer, right-click on "Data Connections."
   - Choose "Add Connection" from the context menu.

4. **Choose Data Source:**
   - In the "Add Connection" dialog, select "Microsoft SQL Server" as the data source.

5. **Configure Connection:**
   - Provide the necessary connection details:
     - **Server Name:** Select the name of your SQL Server from the dropdown menu, click `Refresh` if no options are currently listed.
     - **Authentication:** Choose the appropriate authentication method (Windows or SQL Server).
     - **Username and Password:** If using SQL Server authentication, provide your credentials.
     - **Tick the `Trust Server Certificate` Box**
     - **Database Name:** Select the name of your database from the dropdown menu.

6. **Test Connection:**
   - Click the "Test Connection" button to ensure that the connection details are correct.

7. **Connect:**
   - Click the "OK" button to establish the connection.

8. **View Database Objects:**
   - After successfully connecting, you should see your database listed under "Data Connections" in Server Explorer.
   - Expand the database node to view tables, views, and other database objects.

### Now, go to the `appsettings.json` file in the app, and replace the current `DefaultConnection` string with the string listed in your "Data Connections" from the Server Explorer.  


## Usage  
### Run the API app
The `/AddBook` endpoint allows users to add a book to the database. Click the `Try it out` button and add the following example data, then click `Execute`:  
```json
{
 "Title": "The Great Gatsby",
 "Author": "F. Scott Fitzgerald", 
 "Price": 12.99,
 "Quantity": 10
}
```

* `Title`: The title of the book (required).
* `Author`: The author of the book (required).
* `Price`: The price of the book (required).
* `Quantity`: The quantity of the book (required).

The API endpoint can return the following responses:

**Response**:

* Status Code: `200 OK`
   * Body: "Book added successfully"
* Status Code: `400 Bad Request`
   * Body: Error message if the request body is invalid.
* Status Code: `500 Internal Server Error`
   * Body: Error message if an error occurs while adding the book to the database.

The `/Books` endpoint returns a list of all the books in the database, it does not require any parameters.  
Click the `Try it out` button, then click `Execute`.  


The `/Books/AddToCart` endpoint adds a book to the cart based on the `Firebase` user ID of the current user, and also the book ID.  
This endpoint requires the user ID of an authenticated person who `registered using the client app`. It also requires the ID of the book from the database.  
Click the `Try it out` button, enter add the Firebase user ID and a book ID, then click `Execute`.  

The `/Cart` endpoint returns a list of all the cart items belonging to specific user. It requires a `Firebase` user ID of a registered user.  
Click the `Try it out` button, enter add the Firebase user ID, then click `Execute`.  

The `/Cart/RemoveFromCart/{cartItemId}` endpoint allows a user to delete a book from their cart.  
Click the `Try it out` button, enter cartItemID of an existing item, then click `Execute`.  

The `/EditBooks/AllBooks` endpoint returns a list of all the books in the database, it does not require any parameters.  
This is for the admin using the `client app`, allowing them to select a book to edit.  
Click the `Try it out` button, then click `Execute`.  

The `/EditBooks/Edit/{id}` endpoint returns a list of all the books in the database matching a specific ID, it requires a valid book ID for an existing book in the database.  
This is used in the `client app` to allow admins to edit the details for a specific book.  
Click the `Try it out` button, and add a book ID for an existing book, then click `Execute`.  

The `/EditBooks/Edit/` endpoint adds a book back to the database after an admin using the `client app` has finished updating the book details.  
This endpoint require a book ID and a quantity (to update the number of books available).  
Click the `Try it out` button, and add a book ID and a new quantity for an existing book, then click `Execute`.  

The `/Home` endpoint returns a list of all the books in the database, it does not require any parameters.  
This is used to display a few of the books on home page, for users on the `client app`.  
Click the `Try it out` button, then click `Execute`.  

The `/Home/Search` endpoint returns a list of all the books in the database, based on a search query.  
This endpoints requires a search term such as the `Author name` for a book.  
Click the `Try it out` button, enter a search term to query the list of books then click `Execute`.   

The `/ViewOrders` endpoint returns a list of all the books in a cart, for all customers.  
This allows admins using the `client app` to view all current customer orders.  
Click the `Try it out` button, then click `Execute`.  

Lastly, the `/ViewOrders/ByUser/{userId}` endpoint returns a list of all the books in a cart, for a specific customer based on their `Firebase` user ID.  
This allows admins using the `client app` to view all current customer orders.  
Click the `Try it out` button, then click `Execute`. 


## Authors  
- This app has been built by:
  `Samkelo Tshabalala`

  ## Technologies and Languages Used

- **Frontend:**
 - [Swagger](https://swagger.io/tools/swagger-ui/)

- **Backend:**
  - [C#](https://docs.microsoft.com/en-us/dotnet/csharp/) programming language

- **Database:**
  - [SQL Server](https://www.microsoft.com/en-us/sql-server/)

- **Frameworks and Libraries:**
  - [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
  - [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)

- **Development Environment:**
  - [Visual Studio](https://visualstudio.microsoft.com/)

- **Version Control:**
  - [Git](https://git-scm.com/)

- **Other Tools:**
  - [SSMS (SQL Server Management Studio)](https://docs.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
