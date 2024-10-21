# StockPortfolioTracker
## Table of Contents
- [Project Overview](#project-overview)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Installation](#installation)
- [Usage](#usage)
- [Database Diagram](#database-diagram)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Project Overview
The Stock Management Web Application is an ASP.NET-based application designed to help users manage their stock portfolio. The application provides a user-friendly interface for viewing stock prices, analyzing market trends, and making informed investment decisions.

## Features
- Portfolio management
- Stocks and transactions CRUD operations
- Stock analysis tools
- Search functionality for stocks
- Responsive design for mobile and desktop
- User authentication and authorization (To be implemented)
- Real-time stock price updates (To be implemented)
- Historical price charts (To be implemented)

## Technologies Used
- **Backend:** ASP.NET Core, Entity Framework Core
- **Frontend:** HTML, CSS, JavaScript, Bootstrap
- **Database:** SQL Server
- **Version Control:** Git
- **Deployment:** Azure

## Installation
1. **Clone the Repository:**

```bash
git clone https://github.com/yourusername/your-repo-name.git
```

2. **Navigate to the Project Directory:**

```bash
cd your-repo-name
```

3. **Install Dependencies:**

Ensure you have the .NET SDK installed, then run:

```bash
dotnet restore
```

4. **Set Up the Database:**

Update your database connection string in `appsettings.json`. Then, run the migrations to create the database schema:

```bash
dotnet ef database update
```

5. **Run the Application:**

Start the application with:

```bash
dotnet run
```
Access the application at `http://localhost:39304` if you use `iisSettings` profile.
Access the application at `http://localhost:5063` if you use `http` profile.
Access the application at `http://localhost:7072` if you use `https` profile.

## Usage
### Portfolio Dashboard
- View key portfolio metrics and total value.
- Track stocks with details including symbol, company name, total shares, current price, and total value.
- Review recent transactions, showing stock, quantity, transaction type, and price.
<img src="https://i.ibb.co/WnTHmZC/Portfolio.png" alt="Portfolio" border="0">

### Stocks List
- Access a list of stocks with their symbol, company name, current price, and last updated date.
- Perform actions like **Edit**, **Details**, or **Delete** for each stock.
<img src="https://i.ibb.co/bvwvCmc/Stocks-List.png" alt="Stocks-List" border="0">

### Transactions List
- Manage your transactions, including stock symbol, quantity, transaction type, price, status, and date.
- Options available to **Edit**, **Details**, or **Delete** transactions.
<img src="https://i.ibb.co/1QF1dvY/Transactions-List.png" alt="Transactions-List" border="0">

Easily navigate between sections using the application’s menu.

## Database Diagram
<img src="https://i.ibb.co/Rp0TNPB/Database.png" alt="Database" border="0">

## Configuration
### AppSettings
Configure the necessary settings in the `appsettings.json` file, including:

- Connection strings
- API keys (Not applicable currently)
- Application-specific settings (Not applicable currently)

## Contributing
Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Make your changes and commit them (`git commit -m 'Add some feature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Create a new Pull Request.

## License
This project is licensed under the MIT License.

## Contact
For inquiries, please reach out to:

Abderrahmane Grou - abderrahmane.grou@polymtl.ca
