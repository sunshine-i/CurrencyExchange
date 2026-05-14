# CurrencyExchange

**Course:** Network Application Development

**Author(s):** Sunshine Maree Ilagan

**Student ID(s):** 73656

---

## Project Description

CurrencyExchange is a desktop application that allows users to view live currency exchange rates and simulate currency trading using a virtual account balance. Exchange rates are fetched in real time from the **National Bank of Poland (NBP) public API**, with PLN as the base currency.

### Features

- **User accounts** — register and log in with a username and hashed password
- **Live exchange rates** — browse all NBP Table A currencies and their mid rates against PLN
- **Virtual top-up** — add funds to your account in any supported currency
- **Currency trading** — convert between any two currencies based on live rates, subject to available balance
- **Transaction history** — view a full log of all past top-ups and exchanges, including the rate applied

---

## Project Structure

```
CurrencyExchange/
├── CurrencyExchange.Service/       # WCF web service — fetches and serves exchange rates
├── CurrencyExchange.Client/        # WPF desktop application (MVVM)
├── CurrencyExchange.Database/      # Class library — database models and data access
|   └── Scripts/DatabaseSetup.sql   # SQL Server schema script
└── Documentation/                  # Documentation directory
    └── documentation.md            # Project documentation markdown
```

---

## Prerequisites

| Requirement | Details |
|---|---|
| Visual Studio | 2019 or later, with **.NET desktop development** and **ASP.NET and web development** workloads |
| .NET Framework | 4.8 |
| SQL Server | Any local instance (SQL Server Express / LocalDB is fine) |

---

## How to Run

### 1. Set up the database

Open **SQL Server Management Studio** (or any SQL client) and run the setup script:

```
CurrencyExchange.Database/Scripts/DatabaseSetup.sql
```

This creates the `CurrencyExchangeDb` database and its three tables (`Users`, `Balances`, `Transactions`).

### 2. Configure the connection string

Open `CurrencyExchange.Client/App.config` and update the `Server=` value to match your SQL Server instance:

```xml
<add name="CurrencyExchangeDb"
     connectionString="Server=(LocalDb)\MSSQLLocalDB;Database=CurrencyExchangeDb;Integrated Security=True;"
     providerName="System.Data.SqlClient"/>
```

### 3. Open the solution

Open `CurrencyExchange.slnx` in Visual Studio.

### 4. Configure multiple startup projects
 
To start both the service and the client together:
 
1. Right-click the **solution** in Solution Explorer and select **Configure Startup Projects...**
2. Select **Multiple startup projects**
3. Set both `CurrencyExchange.Service` and `CurrencyExchange.Client` to **Start**
4. Click **OK**.
Then press **F5** to launch both at once.
 
> **Note:** The default service port is `50916` — if it differs on your machine, update the endpoint in `CurrencyExchange.Client/App.config`:
>
> ```xml
> <endpoint address="http://localhost:50916/ExchangeService.svc" ... />
> ```

### 5. Use the application

1. **Register** a new account on the registration screen
2. **Log in** with your credentials
3. Use **Exchange Rates** to browse all current NBP rates
4. Use **Top Up** to add funds to your account in any currency
5. Use **Trade** to convert between currencies
6. Use **History** to review past transactions